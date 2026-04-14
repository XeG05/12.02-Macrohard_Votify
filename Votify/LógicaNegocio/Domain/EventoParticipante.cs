using Votify.Persistencia.Entities;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Enums;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Domain
{
    public class EventoParticipante : EventoBase
    {
        private readonly IRepository<Participante> _participanteRepo;
        private readonly IRepository<Grupo> _grupoRepo;
        private readonly IRepository<Proyecto> _proyectoRepo;
        private readonly IRepository<Categorium> _categoriumRepo;
        private readonly IRepository<Usuario> _usuarioRepo;
        private readonly IRepository<Voto> _votoRepo;
        private readonly IRepository<Comentario> _comentarioRepo;
        private readonly long? _grupoIdActivo;

        public EventoParticipante(
            Evento evento,
            Usuario usuario,
            IRepository<Participante> participanteRepo,
            IRepository<Grupo> grupoRepo,
            IRepository<Proyecto> proyectoRepo,
            IRepository<Categorium> categoriumRepo,
            IRepository<Usuario> usuarioRepo,
            IRepository<Voto> votoRepo,
            IRepository<Comentario> comentarioRepo,
            long? grupoIdActivo = null
        ) : base(evento, usuario)
        {
            _participanteRepo = participanteRepo;
            _grupoRepo = grupoRepo;
            _proyectoRepo = proyectoRepo;
            _categoriumRepo = categoriumRepo;
            _usuarioRepo = usuarioRepo;
            _votoRepo = votoRepo;
            _comentarioRepo = comentarioRepo;
            _grupoIdActivo = grupoIdActivo;
        }

        private async Task<Participante> ObtenerRegistroParticipante()
        {
            var p = await _participanteRepo.FindAsync(p =>
                p.Correo == usuario.Correo &&
                p.Id == evento.Id);

            Participante? participante = null;

            if (_grupoIdActivo.HasValue)
                participante = p.FirstOrDefault(x => x.IdGrupo == _grupoIdActivo.Value);

            participante ??= p.FirstOrDefault();

            if (participante == null)
                throw new Exception("El usuario no está registrado como participante en este evento.");

            return participante;
        }

        public async Task<Grupo?> ObtenerMiGrupo()
        {
            var participante = await ObtenerRegistroParticipante();
            return await _grupoRepo.GetByIdAsync(participante.IdGrupo);
        }

        public async Task<List<(string Nombre, string Correo)>> ObtenerMiembrosGrupo()
        {
            var participante = await ObtenerRegistroParticipante();

            var participantesGrupo = await _participanteRepo.FindAsync(p =>
                p.IdGrupo == participante.IdGrupo);

            var resultado = new List<(string Nombre, string Correo)>();

            foreach (var miembro in participantesGrupo)
            {
                var usuarioMiembro = (await _usuarioRepo.FindAsync(u => u.Correo == miembro.Correo))
                    .FirstOrDefault();

                resultado.Add((
                    usuarioMiembro?.Nombre ?? miembro.Correo,
                    miembro.Correo
                ));
            }

            return resultado;
        }

        public async Task<Categorium?> ObtenerCategoriaGrupo()
        {
            var grupo = await ObtenerMiGrupo();
            if (grupo == null) return null;
            return (await _categoriumRepo.FindAsync(c => c.Id == grupo.IdCategoria)).FirstOrDefault();
        }

        public async Task<List<Categorium>> ObtenerCategoriasEvento()
        {
            return await _categoriumRepo.FindAsync(c => c.IdEvento == evento.Id);
        }

        public async Task<Proyecto?> ObtenerMiProyecto()
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
            var participante = await ObtenerRegistroParticipante();

            return (await _proyectoRepo
                .FindAsync(p => p.IdGrupo == participante.IdGrupo))
                .FirstOrDefault();
        }

        public async Task<bool> TieneProyecto()
        {
            var proyecto = await ObtenerMiProyecto();
            return proyecto != null;
        }

        public async Task CrearProyecto(
            string nombreProyecto,
            string descripcionBreve,
            string descripcionCompleta,
            byte[] archivo,
            List<byte[]> imagenes)
        {
            if (string.IsNullOrWhiteSpace(nombreProyecto))
                throw new Exception("El nombre del proyecto es obligatorio.");

            var participante = await ObtenerRegistroParticipante();
            var grupo = await _grupoRepo.GetByIdAsync(participante.IdGrupo);

            if (grupo == null)
                throw new Exception("No se ha encontrado el grupo del participante.");

            var categoria = (await _categoriumRepo.FindAsync(c =>
                c.Id == grupo.IdCategoria &&
                c.IdEvento == evento.Id))
                .FirstOrDefault();

            if (categoria == null)
                throw new Exception("La categoría del grupo no pertenece al evento.");

            var proyectoExistente = (await _proyectoRepo.FindAsync(p =>
                p.IdGrupo == grupo.Id
                ))
                .FirstOrDefault();

            if (proyectoExistente != null)
                throw new Exception("Ya existe un proyecto entregado para este grupo. Elimínalo antes de volver a entregarlo.");


            string descBreve;
            if (descripcionBreve == null)
                descBreve = string.Empty;
            else
                descBreve = descripcionBreve.Trim();

            string descCompleta;
            if (descripcionCompleta == null)
                descCompleta = string.Empty;
            else
                descCompleta = descripcionCompleta.Trim();

            var nuevoProyecto = new Proyecto(
                nombreProyecto.Trim(),
                grupo.Id,
                grupo,
                descBreve,
                descCompleta
            )
            {
                Archivo = archivo,
                Imagenes = new List<ImagenProyecto>()
            };

            await _proyectoRepo.AddAsync(nuevoProyecto);

            if (imagenes != null && imagenes.Any())
            {
                var imagenesProyecto = imagenes.Select(img => new ImagenProyecto
                {
                    Archivo = img,
                    ProyectoIdGrupo = grupo.Id
                }).ToList();

                nuevoProyecto.Imagenes.AddRange(imagenesProyecto);
                await _proyectoRepo.UpdateAsync(nuevoProyecto);
            }
        }

        public async Task EliminarProyecto()
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
            var participante = await ObtenerRegistroParticipante();

            var proyecto = (await _proyectoRepo
                .FindAsync(p => p.IdGrupo == participante.IdGrupo))
                .FirstOrDefault();

            if (proyecto == null)
                throw new Exception("No hay ningún proyecto para este grupo.");

            await _proyectoRepo.DeleteAsync(proyecto);
        }
        public async Task<DashboardParticipanteDTO> ObtenerDashboard(long idCategoria)
        {
            if (ObtenerFaseActual() != FasesEvento.Resultados)
            {
                throw new Exception("Los resultados aún no están disponibles");
            }

            var participaciones = await _participanteRepo.FindAsync(p => p.Correo == usuario.Correo && p.Id == evento.Id);
            Participante? participante = null;

            foreach (var p in participaciones)
            {
                var g = await _grupoRepo.GetByIdAsync(p.IdGrupo);
                if (g != null && g.IdCategoria == idCategoria)
                {
                    participante = p;
                    break;
                }
            }

            if (participante == null)
            {
                if (_grupoIdActivo.HasValue)
                    participante = participaciones.FirstOrDefault(x => x.IdGrupo == _grupoIdActivo.Value);
                participante ??= participaciones.FirstOrDefault();
            }

            if (participante == null)
                throw new Exception("Participante no encontrado en este evento.");

            var proyecto = (await _proyectoRepo.FindAsync(p => p.IdGrupo == participante.IdGrupo)).FirstOrDefault();

            if (proyecto == null)
                throw new Exception("El grupo no tiene proyecto");

            var grupo = (await _grupoRepo.FindAsync(g => g.Id == proyecto.IdGrupo)).FirstOrDefault();

            var gruposCategoria = await _grupoRepo.FindAsync(g => g.IdCategoria == idCategoria);

            var idsGruposCat = gruposCategoria.Select(g => g.Id).ToList();

            var proyectos = (await _proyectoRepo.FindAsync(p => idsGruposCat.Contains(p.IdGrupo))).ToList();

            var ranking = new List<(Proyecto proyecto, int votos)>();

            foreach (var p in proyectos)
            {
                var votos = await _votoRepo.FindAsync(v =>
                    v.IdProyectoGru == p.Id);

                ranking.Add((p, votos.Count));
            }

            var rankingOrdenado = ranking.OrderByDescending(r => r.votos).ToList();

            var proyectoElegido = rankingOrdenado.FirstOrDefault(r => r.proyecto.IdGrupo == proyecto.IdGrupo);

            if (proyectoElegido.proyecto == null)
                throw new Exception("El proyecto no se encontró en el ranking");

            var posicion = rankingOrdenado.FindIndex(r => r.proyecto.IdGrupo == proyecto.IdGrupo) + 1;

            var totalVotos = proyectoElegido.votos;

            var comentarios = await _comentarioRepo.FindAsync(c => c.IdProyectoGru == proyecto.Id);


         

            return new DashboardParticipanteDTO
            {
                NombreProyecto = proyecto.Nombre,
                NombreGrupo = grupo?.Nombre ?? "Sin grupo",
                GrupoId = grupo?.Id ?? 0,
                TotalVotos = totalVotos,
                Posicion = posicion,

                Comentarios = comentarios.Select(c => new ComentarioDTO
                {
                    Autor = c.CorreoRol ?? "Anónimo",
                    Contenido = c.Contenido
                }).ToList()
            };
        }
    }
}
        
 
