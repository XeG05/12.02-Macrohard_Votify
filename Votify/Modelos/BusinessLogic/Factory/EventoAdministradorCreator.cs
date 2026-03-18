using Votify.Modelos.BusinessLogic.Domain;
using Votify.Modelos.Persistence.Interfaces;

namespace Votify.Modelos.BusinessLogic.Factory
{
    public class EventoAdministradorCreator : EventoCreator
    {
        private readonly IRepository<Evento> _eventoRepo;
        private readonly IRepository<Proyecto> _proyectoRepo;
        private readonly IRepository<Participante> _participanteRepo;
        private readonly IRepository<Grupo> _grupoRepo;
        private readonly IRepository<Categorium> _categoriumRepo;

        public EventoAdministradorCreator(
            IRepository<Evento> eventoRepo,
            IRepository<Proyecto> proyectoRepo,
            IRepository<Participante> participanteRepo,
            IRepository<Grupo> grupoRepo,
            IRepository<Categorium> categoriumRepo)
        {
            _eventoRepo = eventoRepo;
            _proyectoRepo = proyectoRepo;
            _participanteRepo = participanteRepo;
            _grupoRepo = grupoRepo;
            _categoriumRepo = categoriumRepo;
        }

        public override EventoBase CrearEvento(Evento evento, Usuario usuario)
        {
            return new EventoAdministrador(evento, usuario, _eventoRepo, _proyectoRepo, _participanteRepo, _categoriumRepo, _grupoRepo);
        }
    }
}
