using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Exceptions;
using Votify.LógicaNegocio.Interfaces;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Services
{
    public class EventoParticipanteService : IEventoParticipanteService
    {
        private readonly IRepository<Participante> _participanteRepo;
        private readonly IRepository<Grupo> _grupoRepo;
        private readonly IRepository<Proyecto> _proyectoRepo;
        private readonly IRepository<Categorium> _categoriumRepo;
        private readonly IRepository<Evento> _eventoRepo;
        private readonly IRepository<Usuario> _usuarioRepo;
        private readonly IRepository<Voto> _votoRepo;
        private readonly IRepository<Comentario> _comentarioRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly long? _grupoIdActivo;

        public EventoParticipanteService(
            IRepository<Participante> participanteRepo,
            IRepository<Grupo> grupoRepo,
            IRepository<Proyecto> proyectoRepo,
            IRepository<Categorium> categoriumRepo,
            IRepository<Evento> eventoRepo,
            IRepository<Usuario> usuarioRepo,
            IRepository<Voto> votoRepo,
            IRepository<Comentario> comentarioRepo,
            IUnitOfWork unitOfWork,
            long? grupoIdActivo = null
        )
        {
            _participanteRepo = participanteRepo;
            _grupoRepo = grupoRepo;
            _proyectoRepo = proyectoRepo;
            _categoriumRepo = categoriumRepo;
            _eventoRepo = eventoRepo;
            _usuarioRepo = usuarioRepo;
            _votoRepo = votoRepo;
            _comentarioRepo = comentarioRepo;
            _unitOfWork = unitOfWork;
            _grupoIdActivo = grupoIdActivo;
        }

        public async Task<Participante> ObtenerRegistroParticipante(string correo, long idEvento, long? grupoIdActivo = null)
        {
            var p = await _participanteRepo.FindAsync(x =>
                x.CorreoUsuario == correo &&
                x.IdEvento == idEvento);

            Participante? participante = null;

            if (grupoIdActivo.HasValue)
                participante = p.FirstOrDefault(x => x.IdGrupo == grupoIdActivo.Value);

            participante ??= p.FirstOrDefault()
                ?? throw new NotFoundException("El usuario no está registrado como participante en este evento.");

            return participante;
        }

        public async Task<Grupo?> ObtenerMiGrupo(string correo, long idEvento, long? grupoIdActivo = null)
        {
            var participante = await ObtenerRegistroParticipante(correo, idEvento, grupoIdActivo);
            return await _grupoRepo.GetByIdAsync(participante.IdGrupo);
        }

        public async Task<List<(string Nombre, string Correo)>> ObtenerMiembrosGrupo(string correo, long idEvento, long? grupoIdActivo = null)
        {
            var participante = await ObtenerRegistroParticipante(correo, idEvento, grupoIdActivo);

            var participantesGrupo = await _participanteRepo.FindAsync(p =>
                p.IdGrupo == participante.IdGrupo);

            var correos = participantesGrupo.Select(p => p.CorreoUsuario).ToList();
            var usuarios = await _usuarioRepo.FindAsync(u => correos.Contains(u.Correo));
            var usuariosPorCorreo = usuarios.ToDictionary(u => u.Correo);

            return participantesGrupo.Select(m => (
                usuariosPorCorreo.TryGetValue(m.CorreoUsuario, out var u) ? u.Nombre : m.CorreoUsuario,
                m.CorreoUsuario
            )).ToList();
        }

        public async Task<Categorium?> ObtenerCategoriaGrupo(string correo, long idEvento, long? grupoIdActivo = null)
        {
            var grupo = await ObtenerMiGrupo(correo, idEvento, grupoIdActivo);
            if (grupo == null) return null;
            return (await _categoriumRepo.FindAsync(c => c.Id == grupo.IdCategoria)).FirstOrDefault();
        }

        public async Task<List<Categorium>> ObtenerCategoriasEvento(long idEvento)
        {
            return await _categoriumRepo.FindAsync(c => c.IdEvento == idEvento);
        }

        public async Task<Proyecto?> ObtenerMiProyecto(string correo, long idEvento, long? grupoIdActivo)
        {
            // (PABLO) LO QUE HABIA ANTES:
            /*
            var participante = await ObtenerRegistroParticipante();

            var categoriasEvento = await ObtenerCategoriasEvento();
            var idsCategorias = categoriasEvento.Select(c => c.Id).ToHashSet();

            var proyectosGrupo = await _proyectoRepo.FindAsync(p =>
                p.IdGrupo == participante.IdGrupo);

            return proyectosGrupo.FirstOrDefault(p => idsCategorias.Contains(p.IdCategoria));*/


            //LO NUEVO CORREGIDO (CREO):
            var participante = await ObtenerRegistroParticipante(correo, idEvento, grupoIdActivo);

            return (await _proyectoRepo
                .FindAsync(p => p.IdGrupo == participante.IdGrupo))
                .FirstOrDefault();
        }

        public async Task<bool> TieneProyecto(string correo, long idEvento, long? grupoIdActivo)
        {
            var proyecto = await ObtenerMiProyecto(correo, idEvento, grupoIdActivo);
            return proyecto != null;
        }

        public async Task CrearProyecto(CrearProyectoRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.NombreProyecto))
                    throw new ValidationException("El nombre del proyecto es obligatorio.");

                var participante = await ObtenerRegistroParticipante(request.Correo, request.IdEvento, request.GrupoIdActivo);
                var grupo = await _grupoRepo.GetByIdAsync(participante.IdGrupo)
                    ?? throw new NotFoundException("No se ha encontrado el grupo del participante.");

                var categoria = (await _categoriumRepo.FindAsync(c =>
                    c.Id == grupo.IdCategoria &&
                    c.IdEvento == request.IdEvento))
                    .FirstOrDefault() ?? throw new ConflictException("La categoría del grupo no pertenece al evento.");

                var proyectoExistente = (await _proyectoRepo.FindAsync(p =>
                    p.IdGrupo == grupo.Id
                )).FirstOrDefault();

                if (proyectoExistente != null)
                    throw new ConflictException("Ya existe un proyecto entregado para este grupo. Elimínalo antes de volver a entregarlo.");


                string descBreve;
                if (request.DescripcionBreve == null)
                    descBreve = string.Empty;
                else
                    descBreve = request.DescripcionBreve.Trim();

                string descCompleta;
                if (request.DescripcionCompleta == null)
                    descCompleta = string.Empty;
                else
                    descCompleta = request.DescripcionCompleta.Trim();

                var nuevoProyecto = new Proyecto(
                    request.NombreProyecto.Trim(),
                    grupo.Id,
                    null!, // No asignar la navegación aquí para evitar que EF intente insertar el grupo
                    descBreve,
                    descCompleta
                )
                {
                    Archivo = request.Archivo,
                    Imagenes = new List<ImagenProyecto>()
                };

                if (request.Imagenes != null && request.Imagenes.Any())
                {
                    var imagenesProyecto = request.Imagenes.Select(img => new ImagenProyecto
                    {
                        Archivo = img
                    }).ToList();

                    nuevoProyecto.Imagenes.AddRange(imagenesProyecto);
                }

                await _proyectoRepo.AddAsync(nuevoProyecto);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error interno: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }

        public async Task EliminarProyecto(string correo, long idEvento, long? grupoIdActivo)
        {
            //(PABLO) LO QUE HABIA ANTES:
            /*
            var participante = await ObtenerRegistroParticipante();

            var categoriasEvento = await ObtenerCategoriasEvento();
            var idsCategorias = categoriasEvento.Select(c => c.Id).ToHashSet();

            var proyectosGrupo = await _proyectoRepo.FindAsync(p =>
                p.IdGrupo == participante.IdGrupo);

            var proyectosEvento = proyectosGrupo
                .Where(p => idsCategorias.Contains(p.IdCategoria))
                .ToList();

            if (!proyectosEvento.Any())
                throw new Exception("No hay ningún proyecto entregado para este evento.");

            foreach (var proyecto in proyectosEvento)
            {
                await _proyectoRepo.DeleteAsync(proyecto);
            }*/

            //LO NUEVO CORREGIDO (CREO):
            var participante = await ObtenerRegistroParticipante(correo, idEvento, grupoIdActivo);

            var proyecto = (await _proyectoRepo
                .FindAsync(p => p.IdGrupo == participante.IdGrupo))
                .FirstOrDefault() ?? throw new NotFoundException("No hay ningún proyecto para este grupo.");

            await _proyectoRepo.DeleteAsync(proyecto);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<DashboardParticipanteDto> ObtenerDashboard(long idCategoria,string correo, long idEvento)
        {
            if (ObtenerFaseActual(idEvento) != FasesEvento.Resultados)
                throw new Exception("Los resultados aún no están disponibles");

            var participaciones = await _participanteRepo.FindAsync(p =>
                p.CorreoUsuario == correo && p.IdEvento == idEvento);

            var idsGrupos = participaciones.Select(p => p.IdGrupo).ToList();
            var grupos = await _grupoRepo.FindAsync(g => idsGrupos.Contains(g.Id));
            var gruposPorId = grupos.ToDictionary(g => g.Id);

            Participante? participante = participaciones
                .FirstOrDefault(p => gruposPorId.TryGetValue(p.IdGrupo, out var g) && g.IdCategoria == idCategoria);

            if (participante == null)
            {
                if (_grupoIdActivo.HasValue)
                    participante = participaciones.FirstOrDefault(x => x.IdGrupo == _grupoIdActivo.Value);
                participante ??= participaciones.FirstOrDefault();
            }

            if (participante == null)
                throw new NotFoundException("Participante no encontrado en este evento.");

            var proyecto = (await _proyectoRepo.FindAsync(p => p.IdGrupo == participante.IdGrupo)).FirstOrDefault()
                    ?? throw new ConflictException("El grupo no tiene proyecto");

            var grupo = gruposPorId.GetValueOrDefault(proyecto.IdGrupo);

            var gruposCategoria = await _grupoRepo.FindAsync(g => g.IdCategoria == idCategoria);
            var idsGruposCategoria = gruposCategoria.Select(g => g.Id).ToList();

            var proyectos = await _proyectoRepo.FindAsync(p => idsGruposCategoria.Contains(p.IdGrupo));

            var idsProyectos = proyectos.Select(p => p.Id).ToList();
            var todosLosVotos = await _votoRepo.FindAsync(v => idsProyectos.Contains(v.IdProyectoGru));
            var votosPorProyecto = todosLosVotos
                .GroupBy(v => v.IdProyectoGru)
                .ToDictionary(g => g.Key, g => g.Count());

            var ranking = proyectos
                .Select(p => (proyecto: p, votos: votosPorProyecto.GetValueOrDefault(p.Id, 0)))
                .OrderByDescending(r => r.votos)
                .ToList();

            var posicion = ranking.FindIndex(r => r.proyecto.IdGrupo == proyecto.IdGrupo) + 1;
            var totalVotos = votosPorProyecto.GetValueOrDefault(proyecto.Id, 0);

            var comentarios = await _comentarioRepo.FindAsync(c => c.IdProyectoGru == proyecto.Id);

            return new DashboardParticipanteDto
            {
                NombreProyecto = proyecto.Nombre,
                NombreGrupo = grupo?.Nombre ?? "Sin grupo",
                GrupoId = grupo?.Id ?? 0,
                TotalVotos = totalVotos,
                Posicion = posicion,
                Comentarios = comentarios.Select(c => new ComentarioDto
                {
                    Autor = c.CorreoRol ?? "Anónimo",
                    Contenido = c.Contenido
                }).ToList()
            };
        }
        public FasesEvento ObtenerFaseActual(long idEvento)
        {
            var evento = _eventoRepo.GetByIdAsync(idEvento).Result
                ?? throw new NotFoundException("Evento no encontrado");

            var hoy = DateOnly.FromDateTime(DateTime.Now);
            if (hoy <= evento.FechaFinInscripciones) return FasesEvento.Inscripciones;
            if (hoy <= evento.FechaFinEntrega) return FasesEvento.Entregas;
            if (hoy <= evento.FechaFinVotaciones) return FasesEvento.Votaciones;
            return FasesEvento.Resultados;
        }
    }
}
