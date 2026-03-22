using Votify.Modelos.Persistence.Interfaces;
namespace Votify.Modelos.BusinessLogic.Domain
{
    public class EventoParticipante : EventoBase
    {
        private readonly IRepository<Participante> _participanteRepo;
        private readonly IRepository<Grupo> _grupoRepo;
        private readonly IRepository<Proyecto> _proyectoRepo;
        private readonly IRepository<Categorium> _categoriaRepo;
        private readonly IRepository<Usuario> _usuarioRepo;

        public EventoParticipante(
            Evento evento,
            Usuario usuario,
            IRepository<Participante> participanteRepo,
            IRepository<Grupo> grupoRepo,
            IRepository<Proyecto> proyectoRepo,
            IRepository<Categorium> categoriaRepo,
            IRepository<Usuario> usuarioRepo
        ) : base(evento, usuario)
        {
            _participanteRepo = participanteRepo;
            _grupoRepo = grupoRepo;
            _proyectoRepo = proyectoRepo;
            _categoriaRepo = categoriaRepo;
            _usuarioRepo = usuarioRepo;
        }

        private async Task<Participante> ObtenerRegistroParticipante()
        {
            var participante = (await _participanteRepo.FindAsync(p =>
                p.Correo == usuario.Correo &&
                p.Id == evento.Id))
                .FirstOrDefault();

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

        public async Task<List<Categorium>> ObtenerCategoriasEvento()
        {
            return await _categoriaRepo.FindAsync(c => c.IdEvento == evento.Id);
        }

        public async Task<Proyecto?> ObtenerMiProyecto()
        {
            var participante = await ObtenerRegistroParticipante();

            var categoriasEvento = await ObtenerCategoriasEvento();
            var idsCategorias = categoriasEvento.Select(c => c.Id).ToHashSet();

            var proyectosGrupo = await _proyectoRepo.FindAsync(p =>
                p.IdGrupo == participante.IdGrupo);

            return proyectosGrupo.FirstOrDefault(p => idsCategorias.Contains(p.IdCategoria));
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
            long idCategoria)
        {
            if (string.IsNullOrWhiteSpace(nombreProyecto))
                throw new Exception("El nombre del proyecto es obligatorio.");

            var participante = await ObtenerRegistroParticipante();
            var grupo = await _grupoRepo.GetByIdAsync(participante.IdGrupo);

            if (grupo == null)
                throw new Exception("No se ha encontrado el grupo del participante.");

            var categoria = (await _categoriaRepo.FindAsync(c =>
                c.Id == idCategoria &&
                c.IdEvento == evento.Id))
                .FirstOrDefault();

            if (categoria == null)
                throw new Exception("La categoría seleccionada no pertenece al evento.");

            var proyectoExistente = (await _proyectoRepo.FindAsync(p =>
                p.IdGrupo == grupo.Id &&
                p.IdCategoria == idCategoria))
                .FirstOrDefault();

            if (proyectoExistente != null)
                throw new Exception("Ya existe un proyecto entregado para esta categoría. Elimínalo antes de volver a entregarlo.");

            var nuevoProyecto = new Proyecto(
                nombreProyecto.Trim(),
                grupo.Id,
                categoria.Id,
                categoria,
                grupo
            );

            await _proyectoRepo.AddAsync(nuevoProyecto);
        }

        public async Task EliminarProyecto()
        {
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
            }
        }


    }
}