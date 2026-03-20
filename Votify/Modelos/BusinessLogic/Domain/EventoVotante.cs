using Votify.Modelos.Persistence.Interfaces;

namespace Votify.Modelos.BusinessLogic.Domain
{
    public class EventoVotante : EventoBase
    {
        private readonly IRepository<Proyecto> _proyectoRepo;
        private readonly IRepository<Evento> _eventoRepo;
        private readonly IRepository<Participante> _participanteRepo;
        private readonly IRepository<Grupo> _grupoRepo;
        private readonly IRepository<Categorium> _categoriumRepo;
        private readonly IRepository<Rol> _rolRepo;
        private readonly IRepository<Criterio> _criterioRepo;
        private readonly IRepository<Voto> _votoRepo;
        public EventoVotante(Evento evento, Usuario usuario, 
            IRepository<Evento> eventoRepo, 
            IRepository<Proyecto> proyectoRepo, 
            IRepository<Participante> participanteRepo, 
            IRepository<Categorium> categoriumRepo, 
            IRepository<Grupo> grupoRepo, 
            IRepository<Rol> rolRepo, 
            IRepository<Criterio> criterioRepo,
            IRepository<Voto> votoRepo) : base(evento, usuario)
        {
            _proyectoRepo = proyectoRepo;
            _eventoRepo = eventoRepo;
            _participanteRepo = participanteRepo;
            _grupoRepo = grupoRepo;
            _categoriumRepo = categoriumRepo;
            _rolRepo = rolRepo;
            _criterioRepo = criterioRepo;
            _votoRepo = votoRepo;
        }

        public async Task<List<Proyecto>> ListaProyectos(long idEvento)
        {
            Evento? eventoActual = await _eventoRepo.GetByIdAsync(idEvento);
            List<Proyecto> proyectos = new List<Proyecto>();
            if (eventoActual != null)
            {
                foreach (Categorium cat in eventoActual.Categorias)
                {
                    proyectos = proyectos.Union(cat.Proyectos).ToList();
                }
            }
            return proyectos;
        }

        public async void votarProyecto(Voto voto)
        {
            Proyecto pr = voto.Proyecto;
            Rol rol = voto.Rol;
            Criterio crit = voto.IdCriterioNavigation;
            pr.Votos.Add(voto);
            rol.Votos.Add(voto);
            crit.Votos.Add(voto);

        }
        
    }
}