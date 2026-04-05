using Votify.LógicaNegocio.Domain;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Factory;
using Votify.LógicaNegocio.Interfaces;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Services
{
    public class Evento_Service : Service<Evento>, I_Evento_Service
    {
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
        private readonly EventoFactory _eventoFactory;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        public Evento_Service(IRepository<Evento> eventoRepo, 
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
            EventoFactory eventoFactory,
            IEmailService emailService,
            IConfiguration config) : base(eventoRepo)
        {
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
            _eventoFactory = eventoFactory;
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

            var roles = await _rolRepo.FindAsync(r => r.CorreoUsuario == correo);
            var participacionIds = roles
                .Select(r => r.IdEvento)
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

        public async Task<EntradaEvento> EntrarEvento(string correo, long eventoId)
        {
            var usuario = (await _usuarioRepo
                .FindAsync(u => u.Correo == correo))
                .FirstOrDefault()
                ?? throw new Exception("Usuario no encontrado");

            var evento = await _eventoRepo.GetByIdAsync(eventoId)
                ?? throw new Exception("Evento no encontrado");

            var rol = await ObtenerRol(correo, eventoId);

            var eventoBase = _eventoFactory.CrearEvento(evento, usuario, rol);
            return new EntradaEvento(eventoBase, rol);
        }

        private async Task<Rol> ObtenerRol(string correo, long idEvento)
        {
            var rol = (await _rolRepo
                .FindAsync(r => r.CorreoUsuario == correo && r.IdEvento == idEvento))
                .FirstOrDefault();

            if (rol != null)
                return rol;

            await AsignarVotanteAsync(correo, idEvento);

            return (await _rolRepo
                .FindAsync(r => r.CorreoUsuario == correo && r.IdEvento == idEvento))
                .FirstOrDefault()
                ?? throw new Exception("Error al asignar rol de votante");
        }

        private async Task AsignarVotanteAsync(string correo, long idEvento)
        {
            var usuario = (await _usuarioRepo.FindAsync(u => u.Correo == correo)).First();
            var evento = await _eventoRepo.GetByIdAsync(idEvento)
                ?? throw new Exception("Evento no encontrado");

            var rolExistente = await _rolRepo.FindAsync(r => r.CorreoUsuario == correo && r.IdEvento == idEvento);

            Rol rol;
            if (!rolExistente.Any())
            {
                rol = new Rol
                {
                    CorreoUsuario = correo,
                    IdEvento = idEvento,
                    CorreoUsuarioNavigation = usuario,
                    IdEventoNavigation = evento
                };
                await _rolRepo.AddAsync(rol);
            }
            else
            {
                rol = rolExistente.First();
            }

            var nuevoVotante = new Votante
            {
                Correo = correo,
                Id = idEvento,
                Rol = rol
            };
            await _votanteRepo.AddAsync(nuevoVotante);
        }

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

            await _adminRepo.AddAsync(admin);

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
    }
    
}
