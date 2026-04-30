using Microsoft.EntityFrameworkCore;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Exceptions;
using Votify.LógicaNegocio.Interfaces;
using Votify.Persistencia;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;


namespace Votify.LógicaNegocio.Services
{
    public class EventoAdministradorService : IEventoAdministradorService
    {
        private readonly IRepository<Evento> _eventoRepo;
        private readonly IRepository<Categorium> _categoriumRepo;
        private readonly IRepository<Grupo> _grupoRepo;
        private readonly IRepository<Proyecto> _proyectoRepo;
        private readonly IRepository<Voto> _votoRepo;
        private readonly IRepository<Criterio> _criterioRepo;
        private readonly IRepository<Participante> _participanteRepo;
        private readonly IRepository<InvitacionPendiente> _invitacionRepo;
        private readonly IRepository<Rol> _rolRepo;
        private readonly IRepository<Administrador> _administradorRepo;
        private readonly IRepository<Usuario> _usuarioRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        private readonly VotifyContext _context;
        public EventoAdministradorService(
            IRepository<Evento> eventoRepo,
            IRepository<Categorium> categoriumRepo,
            IRepository<Grupo> grupoRepo,
            IRepository<Proyecto> proyectoRepo,
            IRepository<Voto> votoRepo,
            IRepository<Criterio> criterioRepo,
            IRepository<Participante> participanteRepo,
            IRepository<InvitacionPendiente> invitacionRepo,
            IRepository<Rol> rolRepo,
            IRepository<Administrador> administradorRepo,
            IRepository<Usuario> usuarioRepo,
        IUnitOfWork unitOfWork,
            IEmailService emailService,
            IConfiguration config,
        VotifyContext context)
        {
            _eventoRepo = eventoRepo;
            _categoriumRepo = categoriumRepo;
            _grupoRepo = grupoRepo;
            _proyectoRepo = proyectoRepo;
            _votoRepo = votoRepo;
            _criterioRepo = criterioRepo;
            _participanteRepo = participanteRepo;
            _invitacionRepo = invitacionRepo;
            _rolRepo = rolRepo;
            _administradorRepo = administradorRepo;
            _usuarioRepo = usuarioRepo;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _config = config;
            _context = context;
            
        }

        public async Task CambiarNombre(long idEvento, string nuevoNombre)
        {
            var evento = await _eventoRepo.GetByIdAsync(idEvento)
                ?? throw new NotFoundException("Evento no encontrado.");
            evento.Nombre = nuevoNombre;
            await _eventoRepo.UpdateAsync(evento);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CambiarDescripcion(long idEvento, string nuevaDescripcion)
        {
            var evento = await _eventoRepo.GetByIdAsync(idEvento)
                ?? throw new NotFoundException("Evento no encontrado.");
            evento.Descripcion = nuevaDescripcion;
            await _eventoRepo.UpdateAsync(evento);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<Categorium>> VerCategorias(long idEvento)
        => await _categoriumRepo.FindAsync(c => c.IdEvento == idEvento);

        public async Task<List<Grupo>> VerGrupo(long idCategoria)
        {
            return await _grupoRepo.FindAsync(g => g.IdCategoria == idCategoria);
        }



        public async Task<List<Proyecto>> VerProyectosCategoria(long idCategoria)
        {
            var grupos = await _grupoRepo.FindAsync(g => g.IdCategoria == idCategoria);

            var idsGrupos = grupos.Select(g => g.Id).ToList();

            var proyectos = await _proyectoRepo.FindAsync(p => idsGrupos.Contains(p.IdGrupo));

            return proyectos;
        }
        public async Task<int> ContarVotosProyecto(long idProyecto)
        {
            var votos = await _votoRepo.FindAsync(v => v.IdProyectoGru == idProyecto);
            return votos.Count;
        }

        public async Task<List<ProyectoResultadosDto>> ObtenerResultadosCategoria(long idCategoria)
        {
            var categoria = await _categoriumRepo.GetByIdAsync(idCategoria)
                ?? throw new NotFoundException("Categoría no encontrada.");

            var evento = await _eventoRepo.GetByIdAsync(categoria.IdEvento)
                ?? throw new NotFoundException("Evento no encontrado.");

            var grupos = await _grupoRepo.FindAsync(g => g.IdCategoria == idCategoria);
            var criterios = await _criterioRepo.FindAsync(c => c.IdCategoria == idCategoria);

            var resultado = new List<ProyectoResultadosDto>();

            foreach (var g in grupos)
            {
                var proyecto = (await _proyectoRepo.FindAsync(p => p.IdGrupo == g.Id)).FirstOrDefault();

                if (proyecto == null)
                    continue;

                var votosProyecto = await _votoRepo.FindAsync(v => v.IdProyectoGru == proyecto.Id);

                double puntuacion = 0;

                var resultadosCriterios = new List<ResultadoCriterioDto>();

                foreach (var criterio in criterios)
                {
                    var votosCriterio = votosProyecto.Where(v => v.IdCriterio == criterio.Id).ToList();
                    double ponderacion = criterio.Ponderacion / 100.0;
                    double? mediaNota = null;
                    double? totalPuntos = null;

                    switch (evento.TipoVotacion)
                    {
                        case TipoVotacion.Simple:
                            puntuacion += ponderacion * votosCriterio.Count;
                            break;

                        case TipoVotacion.Nota:
                            if (votosCriterio.Any())
                            {
                                mediaNota = votosCriterio.Average(v => (double)(v.Nota ?? 0));
                                puntuacion += ponderacion * mediaNota.Value;
                            }
                            break;

                        case TipoVotacion.Puntos:
                            totalPuntos = votosCriterio.Sum(v => (double)(v.Nota ?? 0));
                            puntuacion += ponderacion * totalPuntos.Value;
                            break;
                    }

                    resultadosCriterios.Add(new ResultadoCriterioDto
                    {
                        NombreCriterio = criterio.Nombre,
                        Ponderacion = criterio.Ponderacion,
                        CantidadVotos = votosCriterio.Count,
                        MediaNota = mediaNota,
                        TotalPuntos = totalPuntos
                    });
                }

                resultado.Add(new ProyectoResultadosDto
                {
                    NombreProyecto = proyecto.Nombre,
                    NombreGrupo = g.Nombre,
                    CantidadVotos = votosProyecto.Count,
                    PuntuacionMedia = Math.Round(puntuacion, 2),
                    ResultadosPorCriterio = resultadosCriterios
                });
            }

            return resultado
                .OrderByDescending(r => r.PuntuacionMedia)
                .ToList();
        }

        public async Task<int> TotalVotosCategoria(long idCategoria)
        {
            var grupos = await _grupoRepo.FindAsync(g => g.IdCategoria == idCategoria);
            var idsGrupos = grupos.Select(g => g.Id).ToList();

            var proyectos = await _proyectoRepo.FindAsync(p => idsGrupos.Contains(p.IdGrupo));
            var idsProyectos = proyectos.Select(p => p.Id).ToList();

            var votos = await _votoRepo.FindAsync(v => idsProyectos.Contains(v.IdProyectoGru));

            return votos.Count;
        }

        public async Task<List<Proyecto>> VerProyectoDeGrupo(long idGrupo)
        {
            return await _proyectoRepo.FindAsync(p => p.IdGrupo == idGrupo);
        }

        public async Task<List<MiembroGrupoDto>> VerParticipantes(long idGrupo)
        {
            return await _context.Participantes
                .Include(p => p.CorreoUsuarioNavigation)
                .Where(p => p.IdGrupo == idGrupo)
                .Select(p => new MiembroGrupoDto
                {
                    Correo = p.CorreoUsuario,
                    Nombre = p.CorreoUsuarioNavigation.Nombre
                })
                .ToListAsync();
        }

        public async Task ActualizarParticipantesGrupo(long eventoId, GrupoDto dto)
        {
            var evento = await _eventoRepo.GetByIdAsync(eventoId);
            if (evento == null) throw new Exception("Evento no encontrado");

            ValidarFaseInscripcion(evento);

            var grupo = await _grupoRepo.GetByIdAsync(dto.Id);
            if (grupo == null) throw new Exception("Grupo no encontrado");

            var actuales = await _participanteRepo.FindAsync(p => p.IdGrupo == dto.Id);

            var correosActuales = actuales
            .Select(p => p.CorreoUsuario.ToLower())
            .ToHashSet();

            var nuevosCorreos = dto.CorreosParticipantes
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim().ToLower())
                .Distinct()
                .ToHashSet();

            // VALIDACIÓN GLOBAL (igual que ya tienes)
            var participantesEvento = await _participanteRepo.FindAsync(p => p.IdEvento == eventoId);

            foreach (var correo in nuevosCorreos)
            {
                var yaParticipa = participantesEvento.Any(p =>
                    p.CorreoUsuario.ToLower() == correo && p.IdGrupo != dto.Id);

                if (yaParticipa)
                    throw new Exception($"El usuario {correo} ya participa en otro grupo del evento.");
            }

            // ELIMINAR participantes existentes 
            foreach (var p in actuales)
            {
                var correoActual = p.CorreoUsuario.Trim().ToLower();

                if (!nuevosCorreos.Contains(correoActual))
                    await _participanteRepo.DeleteAsync(p);
            }

            // eliminar invitaciones pendientes antiguas
            var invitaciones = await _invitacionRepo.FindAsync(i =>
                i.IdGrupo == dto.Id &&
                i.IdEvento == eventoId &&
                !i.Aceptada);

            foreach (var inv in invitaciones)
            {
                if (!nuevosCorreos.Contains(inv.CorreoDestino.ToLower()))
                    await _invitacionRepo.DeleteAsync(inv);
            }

            var adminList = (await _administradorRepo.FindAsync(a => a.IdEvento == eventoId));
            var a = adminList[0];
            var creadorList = (await _usuarioRepo.FindAsync(u => u.Correo == a.CorreoUsuario));
            var creador = creadorList[0];
            //  AÑADIR INVITACIONES 
            foreach (var correo in nuevosCorreos)
            {
                if (!correosActuales.Contains(correo))
                {
                    await ProcesarParticipante(
                        correo,
                        dto.Id,
                        evento,
                        creador
                    );
                }
            }
            await _unitOfWork.SaveChangesAsync();
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
        public async Task AñadirGrupo(string nombreGrupo, long idCategoria)
        {
            var categoria = await _context.Categoria
                .FirstOrDefaultAsync(c => c.Id == idCategoria);

            if (categoria == null)
                throw new Exception("Categoría no encontrada");

            var evento = await _eventoRepo.GetByIdAsync(categoria.IdEvento);

            ValidarFaseInscripcion(evento);

            var grupo = new Grupo
            {
                Nombre = nombreGrupo,
                IdCategoria = idCategoria,
                HojaRutaTexto = ""
            };

            await _grupoRepo.AddAsync(grupo);
            await _unitOfWork.SaveChangesAsync();

            // crear proyecto asociado
            var proyecto = new Proyecto
            {
                Nombre = "Sin nombre",
                IdGrupo = grupo.Id,
                Archivo = Array.Empty<byte>()
            };

            await _proyectoRepo.AddAsync(proyecto);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task EliminarGrupo(long grupoId)
        {
            var grupo = await _grupoRepo.GetByIdAsync(grupoId);
            var categoria = await _categoriumRepo.GetByIdAsync(grupo.IdCategoria);
            var evento = await _eventoRepo.GetByIdAsync(categoria.IdEvento);
            var invitaciones = await _context.InvitacionesPendientes
            .Where(i => i.IdGrupo == grupoId)
            .ToListAsync();

            if (grupo == null)
                throw new Exception("Grupo no encontrado");


            if (evento == null)
                throw new Exception("Evento no encontrado");

            ValidarFaseInscripcion(evento);

            // eliminar dependencias
            _context.InvitacionesPendientes.RemoveRange(invitaciones);
            _context.Participantes.RemoveRange(grupo.Participantes);
            _context.Grupos.Remove(grupo);

            await _context.SaveChangesAsync();
        }

        public async Task ActualizarGrupo(long grupoId, GrupoDto dto)
        {
            var grupo = await _grupoRepo.GetByIdAsync(grupoId);
            if (grupo == null)
                throw new Exception("Grupo no encontrado");

            // validar fase
            var categoria = await _categoriumRepo.GetByIdAsync(grupo.IdCategoria);
            var evento = await _eventoRepo.GetByIdAsync(categoria.IdEvento);

            ValidarFaseInscripcion(evento);

            // actualizar nombre
            grupo.Nombre = dto.Nombre;

            await _grupoRepo.UpdateAsync(grupo);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task AceptarInvitacion(string token, string correoUsuario)
        {
            var invitacion = await _invitacionRepo
                .FindAsync(i => i.Token == token && !i.Aceptada);

            var inv = invitacion.FirstOrDefault();
            if (inv == null)
                throw new Exception("Invitación inválida o ya utilizada");

            if (inv.FechaExpiracion < DateTime.UtcNow)
                throw new Exception("La invitación ha expirado");

            if (!inv.CorreoDestino.Equals(correoUsuario, StringComparison.OrdinalIgnoreCase))
                throw new Exception("Esta invitación no es para tu cuenta");

            // crear participante
            await _participanteRepo.AddAsync(new Participante
            {
                CorreoUsuario = correoUsuario,
                IdGrupo = inv.IdGrupo,
                IdEvento = inv.IdEvento
            });

            inv.Aceptada = true;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> ContarInvitacionesPendientes(long idGrupo)
        {
            return await _context.InvitacionesPendientes
                .CountAsync(i => i.IdGrupo == idGrupo && !i.Aceptada);
        }

        public async Task ActualizarFechas(long eventoId,
        DateTime finInscripciones,
        DateTime finEntrega,
        DateTime finVotaciones,
        DateTime finExperta,
        DateTime finVotExperta)
        {
            DateOnly finInscripcionesOnly = DateOnly.FromDateTime(finInscripciones);
            DateOnly finEntregaOnly = DateOnly.FromDateTime(finEntrega);
            DateOnly finVotacionesOnly = DateOnly.FromDateTime(finVotaciones);
            DateOnly finExpertaOnly = DateOnly.FromDateTime(finExperta);
            DateOnly finVotExpertaOnly = DateOnly.FromDateTime(finVotExperta);

            var evento = await _eventoRepo.GetByIdAsync(eventoId);

            if (evento == null)
                throw new NotFoundException("Evento no encontrado");

            var hoy = DateOnly.FromDateTime(DateTime.UtcNow);

            //  Validar orden
            if (!(finInscripcionesOnly < finEntregaOnly && finEntregaOnly < finVotacionesOnly && finVotacionesOnly < finExpertaOnly && finExpertaOnly < finVotExpertaOnly))
                throw new ValidationException("Las fechas deben estar en orden lógico");

            /*//  No fechas pasadas por el momento para hacer pruebas
            if (finInscripcionesOnly < hoy || finEntregaOnly < hoy || finVotacionesOnly < hoy || finExpertaOnly < hoy || finVotExpertaOnly < hoy)
        throw new ValidationException("No puedes poner fechas en el pasado");*/ //POR EL MOMENTO DESHABILITADO PARA HACER PRUEBAS
            //  Restricción por fase
            var faseActual = ObtenerFaseActual(eventoId);

            // 🔹 INSCRIPCIONES → puedes modificar todo
            if (faseActual == FasesEvento.Inscripciones)
            {
                // no bloqueo nada
            }

            // 🔹 ENTREGAS → no puedes tocar inscripciones
            else if (faseActual == FasesEvento.Entregas)
            {
                if (finInscripcionesOnly != evento.FechaFinInscripciones)
                    throw new ValidationException("No puedes modificar una fase ya finalizada");
            }

            // 🔹 VOTACIONES → no puedes tocar inscripciones ni entregas
            else if (faseActual == FasesEvento.Votaciones)
            {
                if (finInscripcionesOnly != evento.FechaFinInscripciones ||
                    finEntregaOnly != evento.FechaFinEntrega)
                    throw new ValidationException("No puedes modificar una fase ya finalizada");
            }

            // 🔹 Evaluacion Experta → no puedes tocar inscripciones ni entregas ni votaciones
            else if (faseActual == FasesEvento.EvaluacionExperta)
            {
                if (finInscripcionesOnly != evento.FechaFinInscripciones ||
                    finEntregaOnly != evento.FechaFinEntrega ||
                    finVotacionesOnly != evento.FechaFinVotaciones)
                    throw new ValidationException("No puedes modificar una fase ya finalizada");
            }

            // 🔹 Votación Experta → solo puedes tocar votacion experta
            else if (faseActual == FasesEvento.VotacionExperta)
            {
                if (finInscripcionesOnly != evento.FechaFinInscripciones ||
                    finEntregaOnly != evento.FechaFinEntrega ||
                    finVotacionesOnly != evento.FechaFinVotaciones ||
                    finExpertaOnly != evento.FechaFinEvaluacionExperta)
                    throw new ValidationException("No puedes modificar una fase ya finalizada");
            }

            // 🔹 FINALIZADO → nada
            else
            {
                throw new ValidationException("El evento ya ha finalizado");
            }

            //  actualizar
            evento.FechaFinInscripciones = finInscripcionesOnly;
            evento.FechaFinEntrega = finEntregaOnly;
            evento.FechaFinVotaciones = finVotacionesOnly;
            evento.FechaFinEvaluacionExperta = finExpertaOnly;
            evento.FechaFinVotacionExperta = finVotExpertaOnly;

            await _eventoRepo.UpdateAsync(evento);
            await _unitOfWork.SaveChangesAsync();
        }

        private void ValidarFaseInscripcion(Evento evento)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Now);

            if (hoy > evento.FechaFinInscripciones)
                throw new Exception("No se pueden modificar participantes fuera de la fase de inscripción.");

            if (evento.Estado == "Pausado")
                throw new Exception("El evento está pausado.");

            if (evento.Estado == "Cerrado")
                throw new Exception("El evento está cerrado.");
        }


        public FasesEvento ObtenerFaseActual(long idEvento)
        {
            var evento = _eventoRepo.GetByIdAsync(idEvento).Result
                ?? throw new NotFoundException("Evento no encontrado");

            var hoy = DateOnly.FromDateTime(DateTime.Now);
            if (hoy <= evento.FechaFinInscripciones) return FasesEvento.Inscripciones;
            if (hoy <= evento.FechaFinEntrega) return FasesEvento.Entregas;
            if (hoy <= evento.FechaFinVotaciones) return FasesEvento.Votaciones;
            if (hoy <= evento.FechaFinEvaluacionExperta) return FasesEvento.EvaluacionExperta;
            if (hoy <= evento.FechaFinVotacionExperta) return FasesEvento.VotacionExperta;
            return FasesEvento.Resultados;
        }
        public async Task PausarEvento(long eventoId)
        {
            var evento = await _eventoRepo.GetByIdAsync(eventoId);
            if (evento == null) throw new Exception("Evento no encontrado");

            if (evento.Estado == "Cerrado")
                throw new Exception("No se puede pausar un evento cerrado");

            evento.Estado = "Pausado";
            await _eventoRepo.UpdateAsync(evento);
        }

        public async Task ReanudarEvento(long eventoId)
        {
            var evento = await _eventoRepo.GetByIdAsync(eventoId);
            if (evento == null) throw new Exception("Evento no encontrado");

            if (evento.Estado == "Cerrado")
                throw new Exception("No se puede reanudar un evento cerrado");

            evento.Estado = "Activo";
            await _eventoRepo.UpdateAsync(evento);
        }

        public async Task CerrarEvento(long eventoId)
        {
            var evento = await _eventoRepo.GetByIdAsync(eventoId);
            if (evento == null) throw new Exception("Evento no encontrado");

            evento.Estado = "Cerrado";
            await _eventoRepo.UpdateAsync(evento);
        }
    }
}
