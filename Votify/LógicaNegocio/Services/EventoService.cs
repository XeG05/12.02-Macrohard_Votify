using Microsoft.EntityFrameworkCore;
using Votify.LógicaNegocio.Domain;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Factory;
using Votify.LógicaNegocio.Factory.Roles;
using Votify.LógicaNegocio.Interfaces;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Services
{
    public class EventoService : Service<Evento>, IEventoService
    {
        private readonly AdministradorCreator _adminCreator;
        private readonly ParticipanteCreator _participanteCreator;
        private readonly VotanteCreator _votanteCreator;
        private readonly IRepository<Evento> _eventoRepo;
        private readonly IRepository<Usuario> _usuarioRepo;
        private readonly IRepository<Proyecto> _proyectoRepo;
        private readonly IRepository<Participante> _participanteRepo;
        private readonly IRepository<Grupo> _grupoRepo;
        private readonly IRepository<Categorium> _categoriumRepo;
        private readonly IRepository<Administrador> _adminRepo;
        private readonly IRepository<Jurado> _juradoRepo;
        private readonly IRepository<Votante> _votanteRepo;
        private readonly IRepository<Rol> _rolRepo;
        private readonly IRepository<Criterio> _criterioRepo;
        private readonly IRepository<Voto> _votoRepo;
        private readonly IRepository<Comentario> _comentarioRepo;
        private readonly IRepository<InvitacionPendiente> _invitacionRepo;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        public EventoService(AdministradorCreator adminC, 
            ParticipanteCreator participanteC, 
            VotanteCreator votanteC,
            IRepository<Evento> eventoRepo,
            IRepository<Usuario> usuarioRepo,
            IRepository<Proyecto> proyectoRepo,
            IRepository<Participante> participanteRepo,
            IRepository<Grupo> grupoRepo,
            IRepository<Categorium> categoriumRepo,
            IRepository<Administrador> adminRepo,
            IRepository<Jurado> juradoRepo,
            IRepository<Votante> votanteRepo,
            IRepository<Rol> rolRepo,
            IRepository<Criterio> criterioRepo,
            IRepository<Voto> votoRepo,
            IRepository<Comentario> comentarioRepo,
            IRepository<InvitacionPendiente> invitacionRepo,
            IEmailService emailService,
            IConfiguration config) : base(eventoRepo)
        {
            _adminCreator = adminC;
            _participanteCreator = participanteC;
            _votanteCreator = votanteC;
            _eventoRepo = eventoRepo;
            _usuarioRepo = usuarioRepo;
            _proyectoRepo = proyectoRepo;
            _participanteRepo = participanteRepo;
            _grupoRepo = grupoRepo;
            _categoriumRepo = categoriumRepo;
            _adminRepo = adminRepo;
            _juradoRepo = juradoRepo;
            _votanteRepo = votanteRepo;
            _rolRepo = rolRepo;
            _criterioRepo = criterioRepo;
            _votoRepo = votoRepo;
            _comentarioRepo = comentarioRepo;
            _invitacionRepo = invitacionRepo;
            _emailService = emailService;
            _config = config;
        }

        public async Task<List<Evento>> GetEventosCreados(string correo)
        {
            var admins = await _adminRepo.FindAsync(a => a.Correo == correo);
            var eventIds = admins.Select(a => a.Id).ToHashSet();
            var todos = await _eventoRepo.GetAllAsync();
            return todos.Where(e => eventIds.Contains(e.Id)).ToList();
        }

        public async Task<List<Evento>> GetEventosParticipacion(string correo)
        {
            var admins = await _adminRepo.FindAsync(a => a.Correo == correo);
            var adminEventIds = admins.Select(a => a.Id).ToHashSet();

            var participantes = await _participanteRepo.FindAsync(p => p.Correo == correo);
            var jurados = await _juradoRepo.FindAsync(j => j.Correo == correo);

            var participacionIds = participantes.Select(p => p.Id)
                .Concat(jurados.Select(j => j.Id))
                .Where(id => !adminEventIds.Contains(id))
                .ToHashSet();

            var todos = await _eventoRepo.GetAllAsync();
            return todos.Where(e => participacionIds.Contains(e.Id)).ToList();
        }

        public async Task<List<Evento>> GetEventosDisponibles(string correo)
        {
            var eventosCreados = await GetEventosCreados(correo);
            var eventosParticipacion = await GetEventosParticipacion(correo);
            var eventosExcluidos = new HashSet<long>(
                eventosCreados.Select(e => e.Id)
                .Concat(eventosParticipacion.Select(e => e.Id)));
            var todos = await _eventoRepo.GetAllAsync();
            return todos.Where(e => !eventosExcluidos.Contains(e.Id)).ToList();
        }

        public async Task<List<(Evento Evento, string RolTexto, Grupo? Grupo)>> GetParticipacionesDetalladas(string correo)
        {
            var admins = await _adminRepo.FindAsync(a => a.Correo == correo);
            var adminEventIds = admins.Select(a => a.Id).ToHashSet();

            var participantes = await _participanteRepo.FindAsync(p => p.Correo == correo);
            var jurados = await _juradoRepo.FindAsync(j => j.Correo == correo);

            var res = new List<(Evento, string, Grupo?)>();
            var todos = await _eventoRepo.GetAllAsync();

            foreach (var j in jurados)
            {
                if (adminEventIds.Contains(j.Id)) continue;
                var ev = todos.FirstOrDefault(e => e.Id == j.Id);
                if (ev != null)
                    res.Add((ev, "Jurado", null));
            }

            foreach (var p in participantes)
            {
                if (adminEventIds.Contains(p.Id)) continue;
                var ev = todos.FirstOrDefault(e => e.Id == p.Id);
                var gr = await _grupoRepo.GetByIdAsync(p.IdGrupo);
                if (ev != null)
                    res.Add((ev, "Participante", gr));
            }

            return res;
        }

        public async Task<EntradaEventoDto> EntrarEvento(string correo, long eventoId, long? grupoId = null)
        {
            var usuario = (await _usuarioRepo
                .FindAsync(u => u.Correo == correo))
                .FirstOrDefault()
                ?? throw new Exception("Usuario no encontrado");

            var evento = await _eventoRepo.GetByIdAsync(eventoId)
                ?? throw new Exception("Evento no encontrado");

            var rol = await ObtenerRol(correo, eventoId);

            return new EntradaEventoDto(evento, rol);
        }

        private async Task<Rol> ObtenerRol(string correo, long idEvento)
        {
            var rol = (await _rolRepo
                .FindAsync(r => r.CorreoUsuario == correo && r.IdEvento == idEvento))
                .FirstOrDefault();

            if (rol == null)
            {
                await AsignarVotanteAsync(correo, idEvento);

                rol = (await _rolRepo
                    .FindAsync(r => r.CorreoUsuario == correo && r.IdEvento == idEvento))
                    .FirstOrDefault()
                    ?? throw new Exception("Error al asignar rol de votante");
            }

            // Cargar explícitamente las propiedades de navegación si no están cargadas
            rol.Administrador ??= (await _adminRepo.FindAsync(a => a.Correo == correo && a.Id == idEvento)).FirstOrDefault();
            rol.Participantes = (await _participanteRepo.FindAsync(p => p.Correo == correo && p.Id == idEvento)).ToList();
            rol.Votante ??= (await _votanteRepo.FindAsync(v => v.Correo == correo && v.Id == idEvento)).FirstOrDefault();
            rol.Jurado ??= (await _juradoRepo.FindAsync(j => j.Correo == correo && j.Id == idEvento)).FirstOrDefault();

            return rol;
        }

        private Task AsignarVotanteAsync(string correo, long idEvento)
        => _votanteCreator.AsignarAsync(correo, idEvento);

        public async Task<Evento> CrearEvento(string correoCreador, EventoCreacionDto dto)
        {
            var (esValido, error) = dto.Validar();
            if (!esValido) throw new Exception(error);

            var usuario = (await _usuarioRepo.FindAsync(u => u.Correo == correoCreador))
                .FirstOrDefault()
                ?? throw new Exception("Usuario no encontrado");

            var evento = new Evento(
                dto.Nombre,
                dto.Descripcion,
                dto.FechaFinInscripciones!.Value,
                dto.FechaFinEntrega!.Value,
                dto.FechaFinVotaciones!.Value,
                dto.TieneGrupos,
                dto.GruposCompletos,
                dto.NumParticipantes
            );

            await _eventoRepo.AddAsync(evento);

            var rol = new Rol
            {
                CorreoUsuario = correoCreador,
                IdEvento = evento.Id,
                CorreoUsuarioNavigation = usuario,
                IdEventoNavigation = evento
            };

            await _rolRepo.AddAsync(rol);


            var admin = new Administrador
            {
                Correo = correoCreador,
                Id = evento.Id,
                Rol = rol
            };

            await _adminCreator.AsignarAsync(correoCreador, evento.Id);

            foreach (var catDto in dto.Categorias)
            {
                var categoria = new Categorium
                {
                    Nombre = catDto.Nombre,
                    Descripcion = catDto.Descripcion,
                    IdEvento = evento.Id,
                    PonderacionPopular = catDto.PonderacionPopular,
                    PonderacionExperto = catDto.PonderacionExperto
                };
                await _categoriumRepo.AddAsync(categoria);

                foreach (var critDto in catDto.Criterios)
                {
                    var criterio = new Criterio
                    {
                        Nombre = critDto.Nombre,
                        Descripcion = critDto.Descripcion,
                        IdCategoria = categoria.Id,
                        Ponderacion = critDto.Ponderacion
                    };
                    await _criterioRepo.AddAsync(criterio);
                }


                if (dto.TieneGrupos)
                {
                    foreach (var grupoDto in catDto.Grupos)
                    {
                        var grupo = new Grupo
                        {
                            Nombre = grupoDto.Nombre,
                            IdCategoria = categoria.Id,
                            HojaRutaTexto = null
                        };
                        await _grupoRepo.AddAsync(grupo);

                        foreach (var correo in grupoDto.CorreosParticipantes
                                                       .Where(c => !string.IsNullOrWhiteSpace(c)))
                        {
                            await ProcesarParticipante(correo, grupo.Id, evento, usuario);
                        }
                    }
                }
            }

            return evento;
        }

        private async Task ProcesarParticipante(string correoInvitado, long idGrupo,
                                          Evento evento, Usuario creador)
        {
            if (string.IsNullOrWhiteSpace(correoInvitado)) return;

            // Evitar duplicados de invitación
            var invExistente = await _invitacionRepo.FindAsync(
                i => i.CorreoDestino == correoInvitado &&
                     i.IdEvento == evento.Id &&
                     i.IdGrupo == idGrupo &&
                     !i.Aceptada);

            if (invExistente.Any()) return;

            // Siempre crear invitación pendiente, exista o no el usuario
            var token = Guid.NewGuid().ToString();
            var invitacion = new InvitacionPendiente
            {
                Token = token,
                CorreoDestino = correoInvitado,
                IdGrupo = idGrupo,
                IdEvento = evento.Id,
                Aceptada = false,
                FechaExpiracion = DateTime.UtcNow.AddDays(7),
                CorreoCreador = creador.Correo
            };
            await _invitacionRepo.AddAsync(invitacion);

            var urlBase = _config["App:Url"] ?? "https://localhost:7001";
            var urlAceptacion = $"{urlBase}/aceptar-invitacion/{token}";

            await _emailService.EnviarInvitacionAsync(
                correoInvitado,
                evento.Nombre,
                creador.Nombre,
                urlAceptacion);
        }

        //devuelve el proyecto que corresponde a la id de categoria y de grupo, y si no existe, devuelve null.
        public async Task<Proyecto> RecuperarProyecto(long idProyecto)
        {
            Proyecto? pr = await _proyectoRepo.AsQueryable().Include(p => p.Imagenes).FirstOrDefaultAsync(p => p.Id == idProyecto);
            return pr;
        }

        public async Task<List<Usuario>> ParticipantesGrupo(long idGrupo)
        {
            List<Participante> participantes = await _participanteRepo.FindAsync(p => p.IdGrupo == idGrupo);
            List<Usuario> usuarios = new List<Usuario>();
            foreach (var participante in participantes)
            {
                var usuario = await _usuarioRepo.FindAsync(u => u.Correo == participante.Correo);
                if (usuario != null && usuario.Any())
                {
                    usuarios.Add(usuario.First());
                }
            }
            return usuarios;
        }

        public async Task<List<Proyecto>> ListaProyectos(long idEvento)
        {
            return await _proyectoRepo.FindAsync(p => p.IdGrupoNavigation.IdCategoriaNavigation.IdEvento == idEvento);
        }

        public async Task<Grupo?> GrupoProyecto(long idGrupo)
        {
            return await _grupoRepo.GetByIdAsync(idGrupo);
        }

        public async Task<List<Categorium>> CategoriasDelEvento(long idEvento)
        {
            return await _categoriumRepo.AsQueryable()
                .Where(c => c.IdEvento == idEvento)
                .Include(c => c.Criterios)
                .ToListAsync();
        }

        public async Task ComentarProyecto(long idEvento, string correo, long idProyecto, string comentario)
        {
            Proyecto pr = await _proyectoRepo.GetByIdAsync(idProyecto) ?? throw new Exception("Proyecto no encontrado.");

            Rol rol = (await _rolRepo.FindAsync(r => r.IdEvento == idEvento && r.CorreoUsuario == correo)).FirstOrDefault() ?? 
                throw new Exception("Rol no encontrado para el usuario en este evento.");

            Comentario comentario1 = new Comentario
            (
                idEvento, 
                correo,
                comentario,
                pr.Id, 
                pr,
                rol
            );

            
            pr.Comentarios ??= new List<Comentario>();
            rol.Comentarios ??= new List<Comentario>();

            pr.Comentarios.Add(comentario1);
            rol.Comentarios.Add(comentario1);
            await _comentarioRepo.AddAsync(comentario1);
            await _proyectoRepo.UpdateAsync(pr);
            await _rolRepo.UpdateAsync(rol);
        }

        public async Task<Comentario?> ObtenerComentarioUsuario(long idProyecto, string correo)
        {
            var proyecto = await _proyectoRepo.GetByIdAsync(idProyecto);
            if (proyecto == null) return null;

            // Buscamos el comentario basándonos en el Id del proyecto y el correo del usuario (el IdProyectoGru en la BD significa Id del Proyecto)
            var comentarios = await _comentarioRepo.FindAsync(c => c.IdProyectoGru == proyecto.Id && c.CorreoRol == correo);
            return comentarios.FirstOrDefault();
        }

        public async Task<(int VotosTotales, bool HaVotado)> ObtenerInfoVotoSimple(long idEvento, long idProyecto, string correo)
        {
            var proyectoActual = await _proyectoRepo.GetByIdAsync(idProyecto);
            if (proyectoActual == null) throw new Exception("Proyecto no encontrado");
            var grupoActual = await _grupoRepo.GetByIdAsync(proyectoActual.IdGrupo);
            if (grupoActual == null) throw new Exception("Grupo no encontrado");
            long idCategoriaActual = grupoActual.IdCategoria;

            var todosLosVotos = await _votoRepo.FindAsync(v => v.IdRol == idEvento && v.CorreoRol == correo);

            var proyectosVotados = todosLosVotos.Select(v => v.IdProyectoGru).Distinct().ToList();
            int proyectosVotadosCount = 0;
            foreach (var pid in proyectosVotados)
            {
                var p = await _proyectoRepo.GetByIdAsync(pid);
                if (p != null)
                {
                    var g = await _grupoRepo.GetByIdAsync(p.IdGrupo);
                    if (g != null && g.IdCategoria == idCategoriaActual)
                    {
                        proyectosVotadosCount++;
                    }
                }
            }

            bool haVotado = todosLosVotos.Any(v => v.IdProyectoGru == idProyecto);

            return (proyectosVotadosCount, haVotado);
        }

        public async Task VotarProyectoSimple(long idEvento, long idProyecto, string correo)
        {
            var info = await ObtenerInfoVotoSimple(idEvento, idProyecto, correo);
            if (info.HaVotado) throw new Exception("Ya has votado por este proyecto.");
            if (info.VotosTotales >= 3) throw new Exception("Límite de votos alcanzado para esta categoría.");

            Proyecto pr = await _proyectoRepo.GetByIdAsync(idProyecto);
            if (pr == null) throw new Exception("Proyecto no encontrado.");

            Grupo grupo = await _grupoRepo.GetByIdAsync(pr.IdGrupo);
            var criterios = await _criterioRepo.FindAsync(c => c.IdCategoria == grupo.IdCategoria);
            Criterio criterioSeleccionado = criterios.FirstOrDefault();
            if (criterioSeleccionado == null) throw new Exception("No existe criterio para emitir un voto en esta categoría.");

            Rol rol = (await _rolRepo.FindAsync(r => r.IdEvento == idEvento && r.CorreoUsuario == correo)).FirstOrDefault();
            if (rol == null) throw new Exception("Rol no encontrado para el usuario en este evento.");

            Voto nuevoVoto = new Voto(
                criterioSeleccionado.Id,
                1,
                correo,
                idProyecto,
                criterioSeleccionado,
                pr,
                rol
            );

            nuevoVoto.IdRol = idEvento;

            pr.Votos ??= new List<Voto>();
            rol.Votos ??= new List<Voto>();

            pr.Votos.Add(nuevoVoto);
            rol.Votos.Add(nuevoVoto);
            await _votoRepo.AddAsync(nuevoVoto);
            await _proyectoRepo.UpdateAsync(pr);
            await _rolRepo.UpdateAsync(rol);
        }
    }
}
