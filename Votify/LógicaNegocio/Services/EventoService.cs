using Microsoft.EntityFrameworkCore;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Exceptions;
using Votify.LógicaNegocio.Factory.Roles;
using Votify.LógicaNegocio.Interfaces;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Services
{
    public class EventoService : Service<Evento>, IEventoService
    {
        private readonly AdministradorCreator _adminCreator;
        private readonly VotanteCreator _votanteCreator;
        private readonly IRepository<Evento> _eventoRepo;
        private readonly IRepository<Usuario> _usuarioRepo;
        private readonly IRepository<Proyecto> _proyectoRepo;
        private readonly IRepository<Participante> _participanteRepo;
        private readonly IRepository<Grupo> _grupoRepo;
        private readonly IRepository<Categorium> _categoriumRepo;
        private readonly IRepository<Administrador> _adminRepo;
        private readonly IRepository<Jurado> _juradoRepo;
        private readonly IRepository<Rol> _rolRepo;
        private readonly IRepository<Criterio> _criterioRepo;
        private readonly IRepository<Voto> _votoRepo;
        private readonly IRepository<Comentario> _comentarioRepo;
        private readonly IRepository<InvitacionPendiente> _invitacionRepo;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _uow;
        public EventoService(AdministradorCreator adminC, 
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
            IConfiguration config,
            IUnitOfWork uow) : base(eventoRepo, uow)
        {
            _adminCreator = adminC;
            _votanteCreator = votanteC;
            _eventoRepo = eventoRepo;
            _usuarioRepo = usuarioRepo;
            _proyectoRepo = proyectoRepo;
            _participanteRepo = participanteRepo;
            _grupoRepo = grupoRepo;
            _categoriumRepo = categoriumRepo;
            _adminRepo = adminRepo;
            _juradoRepo = juradoRepo;
            _rolRepo = rolRepo;
            _criterioRepo = criterioRepo;
            _votoRepo = votoRepo;
            _comentarioRepo = comentarioRepo;
            _invitacionRepo = invitacionRepo;
            _emailService = emailService;
            _config = config;
            _uow = uow;
        }

        public async Task<List<Evento>> GetEventosCreados(string correo)
        {
            var admins = await _adminRepo.FindAsync(a => a.CorreoUsuario == correo);
            var eventIds = admins.Select(a => a.IdEvento).ToHashSet();
            var todos = await _eventoRepo.GetAllAsync();
            return todos.Where(e => eventIds.Contains(e.Id)).Where(e => e.Estado != "Cerrado").ToList();
        }

        public async Task<List<Evento>> GetEventosParticipacion(string correo)
        {
            var admins = await _adminRepo.FindAsync(a => a.CorreoUsuario == correo);
            var adminEventIds = admins.Select(a => a.IdEvento).ToHashSet();

            var participantes = await _participanteRepo.FindAsync(p => p.CorreoUsuario == correo);

            var participacionIds = participantes
                .Select(p => p.IdEvento)
                .Where(id => !adminEventIds.Contains(id))
                .ToHashSet();

            var todos = await _eventoRepo.GetAllAsync();
            return todos.Where(e => participacionIds.Contains(e.Id)).Where(e => e.Estado != "Cerrado").ToList();
        }

        public async Task<List<Evento>> GetEventosDisponibles(string correo)
        {
            var eventosCreados = await GetEventosCreados(correo);
            var eventosParticipacion = await GetEventosParticipacion(correo);
            var eventosJurado = await GetEventosJurados(correo);

            var eventosExcluidos = new HashSet<long>(
                eventosCreados.Select(e => e.Id)
                .Concat(eventosParticipacion.Select(e => e.Id))
                .Concat(eventosJurado.Select(e => e.Id))
            );

            var todos = await _eventoRepo.GetAllAsync();
            return todos.Where(e => !eventosExcluidos.Contains(e.Id)).Where(e => e.Estado != "Cerrado").ToList();
        }

        public async Task<List<Evento>> GetEventosJurados(string correo)
        {
            var admins = await _adminRepo.FindAsync(a => a.CorreoUsuario == correo);
            var adminEventIds = admins.Select(a => a.IdEvento).ToHashSet();

            var jurados = await _juradoRepo.FindAsync(j => j.CorreoUsuario == correo);

            var juradoIds = jurados
                .Select(j => j.IdEvento)
                .Where(id => !adminEventIds.Contains(id))
                .ToHashSet();

            var todos = await _eventoRepo.GetAllAsync();
            return todos.Where(e => juradoIds.Contains(e.Id)).Where(e => e.Estado != "Cerrado").ToList();
        }



        public async Task<EntradaEventoDto> EntrarEvento(string correo, long eventoId, long? grupoId = null)
        {
            var usuario = (await _usuarioRepo
                .FindAsync(u => u.Correo == correo))
                .FirstOrDefault()
                ?? throw new NotFoundException("Usuario no encontrado");

            var evento = await _eventoRepo.GetByIdAsync(eventoId)
                ?? throw new NotFoundException("Evento no encontrado");

            var rol = await ObtenerRol(correo, eventoId);
            await _uow.SaveChangesAsync();
            return new EntradaEventoDto(evento, rol);
        }

        private async Task<Rol> ObtenerRol(string correo, long idEvento)
        {
            var rol = (await _rolRepo
                .FindAsync(r => r.CorreoUsuario == correo && r.IdEvento == idEvento))
                .FirstOrDefault();

            return rol ?? await _votanteCreator.CrearAsync(correo, idEvento);
        }

        private void ValidarCorreosDuplicados(EventoCreacionDto dto)
        {
            if (!dto.TieneGrupos) return;

            var duplicados = dto.Categorias
                .SelectMany(c => c.Grupos)
                .SelectMany(g => g.CorreosParticipantes)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .GroupBy(c => c)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicados.Any())
                throw new ConflictException($"Correos duplicados entre grupos: {string.Join(", ", duplicados)}");
        }

        private Evento CrearEntidadEvento(EventoCreacionDto dto) => new Evento(
            dto.Nombre,
            dto.Descripcion,
            dto.FechaFinInscripciones!.Value,
            dto.FechaFinEntrega!.Value,
            dto.FechaFinVotaciones!.Value,
            dto.FechaFinExperta!.Value,
            dto.TipoComentario,
            dto.ParticipantesVotan,
            dto.TipoVotacion,
            dto.NumVotarProyectos,
            dto.PuntosARepartir,
            dto.TieneGrupos,
            dto.GruposCompletos,
            dto.NumParticipantes
        );

        private async Task CrearCategoriaConCriteriosYGrupos(CategoriaDto catDto, Evento evento, Usuario creador)
        {
            var categoria = new Categorium
            {
                Nombre = catDto.Nombre,
                Descripcion = catDto.Descripcion,
                IdEvento = evento.Id,
                PonderacionPopular = catDto.PonderacionPopular,
                PonderacionExperto = catDto.PonderacionExperto,
                CriteriosSonPorcentaje = catDto.CriteriosSonPorcentaje
            };
            await _categoriumRepo.AddAsync(categoria);

            foreach (var critDto in catDto.Criterios)
                await _criterioRepo.AddAsync(new Criterio
                {
                    Nombre = critDto.Nombre,
                    Descripcion = critDto.Descripcion,
                    IdCategoria = categoria.Id,
                    Ponderacion = critDto.Ponderacion
                });

            if (evento.TieneGrupos)
                foreach (var grupoDto in catDto.Grupos)
                    await CrearGrupoConParticipantes(grupoDto, categoria.Id, evento, creador);
        }

        private async Task CrearGrupoConParticipantes(GrupoDto grupoDto, long idCategoria, Evento evento, Usuario creador)
        {
            var grupo = new Grupo
            {
                Nombre = grupoDto.Nombre,
                IdCategoria = idCategoria,
                HojaRutaTexto = null
            };
            await _grupoRepo.AddAsync(grupo);

            foreach (var correo in grupoDto.CorreosParticipantes.Where(c => !string.IsNullOrWhiteSpace(c)))
                await ProcesarParticipante(correo, grupo.Id, evento, creador);
        }

        public async Task<Evento> CrearEvento(string correoCreador, EventoCreacionDto dto)
        {
            var (esValido, error) = dto.Validar();
            if (!esValido) throw new ValidationException(error);

            var usuario = (await _usuarioRepo.FindAsync(u => u.Correo == correoCreador))
                .FirstOrDefault()
                ?? throw new NotFoundException("Usuario no encontrado");

            ValidarCorreosDuplicados(dto);

            var evento = CrearEntidadEvento(dto);
            await _eventoRepo.AddAsync(evento);
            await _adminCreator.CrearAsync(correoCreador, evento.Id);

            foreach (var catDto in dto.Categorias)
                await CrearCategoriaConCriteriosYGrupos(catDto, evento, usuario);

            await _uow.SaveChangesAsync();
            return evento;
        }

        private async Task ProcesarParticipante(string correoInvitado, long idGrupo,
                                          Evento evento, Usuario creador)
        {
            if (string.IsNullOrWhiteSpace(correoInvitado)) return;

            var invExistente = await _invitacionRepo.FindAsync(
                i => i.CorreoDestino == correoInvitado &&
                     i.IdEvento == evento.Id &&
                     i.IdGrupo == idGrupo &&
                     !i.Aceptada);

            if (invExistente.Any()) return;

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
                var usuario = await _usuarioRepo.FindAsync(u => u.Correo == participante.CorreoUsuario);
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

        public async Task ComentarProyecto(long idEvento, string correo, long idProyecto, long idCriterio, string comentario)
        {
            Proyecto pr = await _proyectoRepo.GetByIdAsync(idProyecto) ?? throw new NotFoundException("Proyecto no encontrado.");

            Rol rol = (await _rolRepo.FindAsync(r => r.IdEvento == idEvento && r.CorreoUsuario == correo)).FirstOrDefault() ?? 
                throw new NotFoundException("Rol no encontrado para el usuario en este evento.");

            Criterio criterio = await _criterioRepo.GetByIdAsync(idCriterio) ?? throw new NotFoundException("Criterio no encontrado.");

            var existente = (await _comentarioRepo.FindAsync(c =>
                c.IdProyectoGru == idProyecto &&
                c.CorreoRol == correo &&
                c.IdRol == idEvento &&
                c.IdCriterio == idCriterio)).FirstOrDefault();

            if (existente != null)
            {
                existente.Contenido = comentario;
                await _comentarioRepo.UpdateAsync(existente);
            }
            else
            {
                var nuevo = new Comentario(idEvento, correo, comentario, pr.Id, idCriterio, pr, rol, criterio);
                await _comentarioRepo.AddAsync(nuevo);
            }

            await _uow.SaveChangesAsync();
        }

        public async Task<List<ComentarioDto>> ObtenerComentariosUsuario(long idProyecto, string correo)
        {
            var comentarios = await _comentarioRepo.FindAsync(
                c => c.IdProyectoGru == idProyecto && c.CorreoRol == correo);

            // Buscamos el comentario basándonos en el Id del proyecto y el correo del usuario (el IdProyectoGru en la BD significa Id del Proyecto)
            return comentarios.Select(c => new ComentarioDto
            {
                IdCriterio = c.IdCriterio,
                Autor = correo,
                Contenido = c.Contenido
            }).ToList();
        }

        public async Task<List<CriterioVotoInfoDto>> ObtenerInfoVotoPorCriterios(long idEvento, long idProyecto, string correo)
        {
            var evento = await _eventoRepo.GetByIdAsync(idEvento)
                ?? throw new NotFoundException("Evento no encontrado");

            var proyectoActual = await _proyectoRepo.GetByIdAsync(idProyecto) 
                ?? throw new NotFoundException("Proyecto no encontrado");

            var grupoActual = await _grupoRepo.GetByIdAsync(proyectoActual.IdGrupo)
                ?? throw new Exception("Grupo no encontrado");

            long idCategoriaActual = grupoActual.IdCategoria;

            var criterios = await _criterioRepo.FindAsync(c => c.IdCategoria == idCategoriaActual);
            var todosLosVotosUsuario = await _votoRepo.FindAsync(v => v.IdRol == idEvento && v.CorreoRol == correo);

            var resultado = new List<CriterioVotoInfoDto>();

            foreach (var criterio in criterios)
            {
                var votosCriterio = todosLosVotosUsuario
                    .Where(v => v.IdCriterio == criterio.Id)
                    .ToList();

                var dto = new CriterioVotoInfoDto { IdCriterio = criterio.Id };

                switch (evento.TipoVotacion)
                {
                    case TipoVotacion.Simple:
                        var proyectosVotadosEnCategoria = new HashSet<long>();
                        foreach (var voto in votosCriterio)
                        {
                            var proyectoVotado = await _proyectoRepo.GetByIdAsync(voto.IdProyectoGru);
                            if (proyectoVotado == null) continue;
                            var grupoVotado = await _grupoRepo.GetByIdAsync(proyectoVotado.IdGrupo);
                            if (grupoVotado?.IdCategoria == idCategoriaActual)
                                proyectosVotadosEnCategoria.Add(voto.IdProyectoGru);
                        }
                        dto.HaVotado = votosCriterio.Any(v => v.IdProyectoGru == idProyecto);
                        dto.VotosRestantes = Math.Max(0, evento.NumVotarProyectos - proyectosVotadosEnCategoria.Count);
                        break;

                    case TipoVotacion.Nota:
                        var votoNota = votosCriterio.FirstOrDefault(v => v.IdProyectoGru == idProyecto);
                        dto.HaVotado = votoNota != null;
                        dto.NotaDada = votoNota?.Nota;
                        break;

                    case TipoVotacion.Puntos:
                        var votoPuntos = votosCriterio.FirstOrDefault(v => v.IdProyectoGru == idProyecto);
                        var puntosUsadosCriterio = votosCriterio.Sum(v => (int)(v.Nota ?? 0));
                        dto.HaVotado = votoPuntos != null && votoPuntos.Nota > 0;
                        dto.PuntosAsignados = votoPuntos?.Nota ?? 0;
                        dto.PuntosRestantes = Math.Max(0, evento.PuntosARepartir - puntosUsadosCriterio);
                        break;
                }

                resultado.Add(dto);
            }

            return resultado;
        }

        public async Task VotarProyecto(long idEvento, long idProyecto, long idCriterio, int? valor, string correo)
        {
            var evento = await _eventoRepo.GetByIdAsync(idEvento)
                ?? throw new NotFoundException("Evento no encontrado.");

            if (evento.TipoComentario == TipoComentario.Obligatorio)
            {
                var comentario = (await _comentarioRepo.FindAsync(c =>
                    c.IdRol == idEvento &&
                    c.CorreoRol == correo &&
                    c.IdProyectoGru == idProyecto &&
                    c.IdCriterio == idCriterio)).FirstOrDefault();

                if (comentario == null || string.IsNullOrWhiteSpace(comentario.Contenido))
                    throw new ValidationException("Debes enviar un comentario antes de votar en este criterio.");
            }

            switch (evento.TipoVotacion)
            {
                case TipoVotacion.Simple:
                    await VotarSimple(evento, idProyecto, idCriterio, correo);
                    break;
                case TipoVotacion.Nota:
                    if (valor == null) throw new ValidationException("Se requiere una nota para este tipo de votación.");
                    await VotarNota(evento, idProyecto, idCriterio, valor.Value, correo);
                    break;
                case TipoVotacion.Puntos:
                    if (valor == null) throw new ValidationException("Se requieren puntos para este tipo de votación.");
                    await VotarPuntos(evento, idProyecto, idCriterio, valor.Value, correo);
                    break;
            }
        }

        public async Task VotarSimple(Evento evento, long idProyecto, long idCriterio, string correo)
        {
            Proyecto proyecto = await _proyectoRepo.GetByIdAsync(idProyecto)
                ?? throw new NotFoundException("Proyecto no encontrado.");

            Grupo grupo = await _grupoRepo.GetByIdAsync(proyecto.IdGrupo)
                ?? throw new NotFoundException("Grupo no encontrado.");

            Criterio criterio = await _criterioRepo.GetByIdAsync(idCriterio)
                ?? throw new NotFoundException("Criterio no encontrado.");

            if (criterio.IdCategoria != grupo.IdCategoria)
                throw new ValidationException("El criterio no pertenece a la categoría del proyecto.");

            var votosUsuario = await _votoRepo.FindAsync(v =>
                v.IdRol == evento.Id &&
                v.CorreoRol == correo &&
                v.IdCriterio == idCriterio);

            if (votosUsuario.Any(v => v.IdProyectoGru == idProyecto))
                throw new ConflictException("Ya has votado este proyecto en este criterio.");

            var proyectosVotadosEnCategoria = new HashSet<long>();

            foreach (var voto in votosUsuario)
            {
                var proyectoVotado = await _proyectoRepo.GetByIdAsync(voto.IdProyectoGru);
                if (proyectoVotado == null) continue;

                var grupoVotado = await _grupoRepo.GetByIdAsync(proyectoVotado.IdGrupo);
                if (grupoVotado?.IdCategoria == grupo.IdCategoria)
                    proyectosVotadosEnCategoria.Add(voto.IdProyectoGru);
            }

            if (proyectosVotadosEnCategoria.Count >= evento.NumVotarProyectos)
                throw new ConflictException($"Límite de {evento.NumVotarProyectos} votos alcanzado para este criterio.");

            Rol rol = (await _rolRepo.FindAsync(r => r.IdEvento == evento.Id && r.CorreoUsuario == correo)).FirstOrDefault()
                ?? throw new NotFoundException("Rol no encontrado para el usuario en este evento.");

            var nuevoVoto = new Voto(criterio.Id, 1, correo, idProyecto, criterio, proyecto, rol);
            nuevoVoto.IdRol = evento.Id;

            proyecto.Votos ??= new List<Voto>();
            rol.Votos ??= new List<Voto>();

            proyecto.Votos.Add(nuevoVoto);
            rol.Votos.Add(nuevoVoto);

            await _votoRepo.AddAsync(nuevoVoto);
            await _proyectoRepo.UpdateAsync(proyecto);
            await _rolRepo.UpdateAsync(rol);
            await _uow.SaveChangesAsync();
        }

        private async Task VotarNota(Evento evento, long idProyecto, long idCriterio, int nota, string correo)
        {
            if (nota <= 0)
                throw new ValidationException("La nota debe ser mayor que 0.");

            Proyecto proyecto = await _proyectoRepo.GetByIdAsync(idProyecto)
                ?? throw new NotFoundException("Proyecto no encontrado.");

            Grupo grupo = await _grupoRepo.GetByIdAsync(proyecto.IdGrupo)
                ?? throw new NotFoundException("Grupo no encontrado.");

            Criterio criterio = await _criterioRepo.GetByIdAsync(idCriterio)
                ?? throw new NotFoundException("Criterio no encontrado.");

            if (criterio.IdCategoria != grupo.IdCategoria)
                throw new ValidationException("El criterio no pertenece a la categoría del proyecto.");

            Rol rol = (await _rolRepo.FindAsync(r => r.IdEvento == evento.Id && r.CorreoUsuario == correo)).FirstOrDefault()
                ?? throw new NotFoundException("Rol no encontrado para el usuario en este evento.");

            var votoExistente = (await _votoRepo.FindAsync(v =>
                v.IdRol == evento.Id &&
                v.CorreoRol == correo &&
                v.IdCriterio == idCriterio &&
                v.IdProyectoGru == idProyecto)).FirstOrDefault();

            if (votoExistente != null)
            {
                votoExistente.Nota = nota;
                await _votoRepo.UpdateAsync(votoExistente);
            }
            else
            {
                var nuevoVoto = new Voto(criterio.Id, nota, correo, idProyecto, criterio, proyecto, rol);
                nuevoVoto.IdRol = evento.Id;
                proyecto.Votos ??= new List<Voto>();
                rol.Votos ??= new List<Voto>();
                proyecto.Votos.Add(nuevoVoto);
                rol.Votos.Add(nuevoVoto);
                await _votoRepo.AddAsync(nuevoVoto);
                await _proyectoRepo.UpdateAsync(proyecto);
                await _rolRepo.UpdateAsync(rol);
            }

            await _uow.SaveChangesAsync();
        }

        private async Task VotarPuntos(Evento evento, long idProyecto, long idCriterio, int puntos, string correo)
        {
            if (puntos < 0)
                throw new ValidationException("Los puntos no pueden ser negativos.");

            Proyecto proyecto = await _proyectoRepo.GetByIdAsync(idProyecto)
                ?? throw new NotFoundException("Proyecto no encontrado.");

            Grupo grupo = await _grupoRepo.GetByIdAsync(proyecto.IdGrupo)
                ?? throw new NotFoundException("Grupo no encontrado.");

            Criterio criterio = await _criterioRepo.GetByIdAsync(idCriterio)
                ?? throw new NotFoundException("Criterio no encontrado.");

            if (criterio.IdCategoria != grupo.IdCategoria)
                throw new ValidationException("El criterio no pertenece a la categoría del proyecto.");

            Rol rol = (await _rolRepo.FindAsync(r => r.IdEvento == evento.Id && r.CorreoUsuario == correo)).FirstOrDefault()
                ?? throw new NotFoundException("Rol no encontrado para el usuario en este evento.");

            var todosVotosCriterio = await _votoRepo.FindAsync(v =>
                v.IdRol == evento.Id &&
                v.CorreoRol == correo &&
                v.IdCriterio == idCriterio);

            var votoExistente = todosVotosCriterio.FirstOrDefault(v => v.IdProyectoGru == idProyecto);

            var puntosUsadosOtrosProyectos = todosVotosCriterio
                .Where(v => v.IdProyectoGru != idProyecto)
                .Sum(v => (int)(v.Nota ?? 0));

            if (puntosUsadosOtrosProyectos + puntos > evento.PuntosARepartir)
                throw new ConflictException($"Superas el límite de {evento.PuntosARepartir} puntos para este criterio.");

            if (votoExistente != null)
            {
                votoExistente.Nota = puntos;
                await _votoRepo.UpdateAsync(votoExistente);
            }
            else if (puntos > 0)
            {
                var nuevoVoto = new Voto(criterio.Id, puntos, correo, idProyecto, criterio, proyecto, rol);
                nuevoVoto.IdRol = evento.Id;
                proyecto.Votos ??= new List<Voto>();
                rol.Votos ??= new List<Voto>();
                proyecto.Votos.Add(nuevoVoto);
                rol.Votos.Add(nuevoVoto);
                await _votoRepo.AddAsync(nuevoVoto);
                await _proyectoRepo.UpdateAsync(proyecto);
                await _rolRepo.UpdateAsync(rol);
            }

            await _uow.SaveChangesAsync();
        }
    }
}
