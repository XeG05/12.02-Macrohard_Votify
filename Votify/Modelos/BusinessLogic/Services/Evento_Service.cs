using Votify.Auth;
using Votify.Modelos.BusinessLogic.DTOs;
using Votify.Modelos.BusinessLogic.Factory;
using Votify.Modelos.BusinessLogic.Interfaces;
using Votify.Modelos.Persistence.Interfaces;

namespace Votify.Modelos.BusinessLogic.Services
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

        public async Task<EntradaEvento> EntrarEvento(string correo, long eventoId)
        {
            var usuario = (await _usuarioRepo
                .FindAsync(u => u.Correo == correo))
                .FirstOrDefault()
                ?? throw new Exception("Usuario no encontrado");

            var evento = await _eventoRepo.GetByIdAsync(eventoId)
                ?? throw new Exception("Evento no encontrado");

            var tipoRol = await DetectarRol(correo, eventoId);

            EventoCreator creator = tipoRol switch
            {
                "ADMIN" => new EventoAdministradorCreator(_usuarioRepo,
                    _eventoRepo, _proyectoRepo,
                    _participanteRepo, _grupoRepo, _categoriumRepo, _votoRepo),

                "PARTICIPANTE" => new EventoParticipanteCreator(_participanteRepo,
            _grupoRepo,
            _proyectoRepo,
            _categoriumRepo,
            _usuarioRepo),

                "VOTANTE" => new EventoVotanteCreator(_eventoRepo,
            _proyectoRepo,
            _participanteRepo,
            _grupoRepo,
            _categoriumRepo,
            _rolRepo,
            _criterioRepo,
            _votoRepo,
            _comentarioRepo,
            _usuarioRepo),
                /*
                // Jurado puede reutilizar VotanteCreator o tener el suyo propio
                "JURADO" => new EventoVotanteCreator(),*/

                _ =>
                throw new Exception(
                    $"No tienes ningún rol asignado en el evento {eventoId}")
            };

            var eventoBase = creator.CrearEvento(evento, usuario);
            return new EntradaEvento(eventoBase, tipoRol);
        }

        private async Task<string> DetectarRol(string correo, long idEvento)
        {
            var admin = await _adminRepo
                .FindAsync(a => a.Correo == correo && a.Id == idEvento);
            if (admin.Any()) return "ADMIN";

            var jurado = await _juradoRepo
                .FindAsync(j => j.Correo == correo && j.Id == idEvento);
            if (jurado.Any()) return "JURADO";

            var participante = await _participanteRepo
                .FindAsync(p => p.Correo == correo && p.Id == idEvento);
            if (participante.Any()) return "PARTICIPANTE";

            var votante = await _votanteRepo
                .FindAsync(v => v.Correo == correo && v.Id == idEvento);
            if (votante.Any()) return "VOTANTE";

            throw new Exception("No tienes ningún rol asignado en este evento");
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
            }

            if (dto.TieneGrupos)
            {
                foreach (var grupoDto in dto.Grupos)
                {
                    var grupo = new Grupo
                    {
                        Nombre = grupoDto.Nombre,
                        HojaRutaTexto = null
                    };
                    await _grupoRepo.AddAsync(grupo);
                    // grupo.Id disponible aquí

                    foreach (var correo in grupoDto.CorreosParticipantes
                                                   .Where(c => !string.IsNullOrWhiteSpace(c)))
                    {
                        await ProcesarParticipante(correo, grupo.Id, evento, usuario);
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
