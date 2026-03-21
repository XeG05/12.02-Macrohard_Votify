using Votify.Modelos.BusinessLogic.DTOs;
using Votify.Modelos.Persistence.Interfaces;

namespace Votify.Modelos.BusinessLogic.Domain
{
    public class EventoAdministrador : EventoBase
    {
        private readonly IRepository<Proyecto> _proyectoRepo;
        private readonly IRepository<Evento> _eventoRepo;
        private readonly IRepository<Participante> _participanteRepo;
        private readonly IRepository<Grupo> _grupoRepo;
        private readonly IRepository<Categorium> _categoriumRepo;
        private readonly IRepository<Voto> _votoRepo;
        public EventoAdministrador(Evento evento, 
            Usuario usuario, 
            IRepository<Evento> eventoRepo, 
            IRepository<Proyecto> proyectoRepo, 
            IRepository<Participante> participanteRepo,
            IRepository<Categorium> categoriumRepo,
            IRepository<Grupo> grupoRepo,
            IRepository<Voto> votoRepo) : base(evento, usuario)
        {
            _proyectoRepo = proyectoRepo;
            _eventoRepo = eventoRepo;
            _participanteRepo = participanteRepo;
            _grupoRepo = grupoRepo;
            _categoriumRepo = categoriumRepo;
            _votoRepo = votoRepo;
        }

        public async Task CambiarNombre(string nuevoNombre)
        {
            evento.Nombre = nuevoNombre;
            await _eventoRepo.UpdateAsync(evento);
        }

        public async Task CambiarDescripcion(string nuevaDescripcion)
        {
            evento.Descripcion = nuevaDescripcion;
            await _eventoRepo.UpdateAsync(evento);
        }

        public async Task<List<Categorium>> VerCategoria()
        {
            return await _categoriumRepo.FindAsync(c => c.IdEvento == evento.Id);
        }

        public async Task<List<Grupo>> VerGrupo(long idCategoria)
        {
            var proyectos = await _proyectoRepo.FindAsync(p => p.IdCategoria == idCategoria);

            var grupos = new List<Grupo>();

            foreach (var p in proyectos)
            {
                var grupo = await _grupoRepo.GetByIdAsync(p.IdGrupo);

                if (grupo != null && !grupos.Any(g => g.Id == grupo.Id)) //evitar repetidos
                {
                    grupos.Add(grupo);
                }
            }
            
            return grupos;
            
        }

        public async Task<List<Proyecto>> VerProyectoDeGrupo(long idGrupo)
        {
            return await _proyectoRepo.FindAsync(p => p.IdGrupo == idGrupo);
        }

        public async Task<List<Participante>> VerParticipantes(long idGrupo)
        {
            return await _participanteRepo.FindAsync(p => p.IdGrupo == idGrupo);
        }

        public async Task<List<Proyecto>> VerProyectos(long idCategoria)
        {
            return await _proyectoRepo.FindAsync(p => p.IdCategoria == idCategoria);
        }
        public async Task<int> ContarVotosProyecto(long idProyecto)
        {
            var votos = await _votoRepo.FindAsync(v => v.IdProyectoGru == idProyecto);
            return votos.Count;
        }

        public async Task<List<ProyectoResumenDTO>> ObtenerResultadosCategoria(long idCategoria)
        {
            var proyectos = await _proyectoRepo.FindAsync(p => p.IdCategoria == idCategoria);

            var resultado = new List<ProyectoResumenDTO>();

            foreach (var p in proyectos)
            {
                var grupo = await _grupoRepo.GetByIdAsync(p.IdGrupo);

                var votos = await _votoRepo.FindAsync(v => v.IdProyectoGru == p.IdGrupo);

                resultado.Add(new ProyectoResumenDTO
                {
                    NombreProyecto = p.Nombre,
                    NombreGrupo = grupo?.Nombre ?? "Sin grupo", //en caso de que grupo sea NULL, pone de nombre "sin grupo"
                    CantidadVotos = votos.Count
                });
            }

            return resultado;
        }

        public async Task<int> TotalVotosCategoria(long idCategoria)
        {
            var proyectos = await _proyectoRepo.FindAsync(p => p.IdCategoria == idCategoria);

            int total = 0;

            foreach (var p in proyectos)
            {
                var votos = await _votoRepo.FindAsync(v => v.IdProyectoGru == p.IdGrupo);
                total += votos.Count;
            }

            return total;
        }


    }
}
