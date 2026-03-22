using Votify.Modelos.BusinessLogic.Domain;
using Votify.Modelos.Persistence.Interfaces;

namespace Votify.Modelos.BusinessLogic.Factory
{
    public class EventoParticipanteCreator : EventoCreator
    {
        private readonly IRepository<Participante> _participanteRepo;
        private readonly IRepository<Grupo> _grupoRepo;
        private readonly IRepository<Proyecto> _proyectoRepo;
        private readonly IRepository<Categorium> _categoriaRepo;
        private readonly IRepository<Usuario> _usuarioRepo;

        public EventoParticipanteCreator(
            IRepository<Participante> participanteRepo,
            IRepository<Grupo> grupoRepo,
            IRepository<Proyecto> proyectoRepo,
            IRepository<Categorium> categoriaRepo,
            IRepository<Usuario> usuarioRepo)
        {
            _participanteRepo = participanteRepo;
            _grupoRepo = grupoRepo;
            _proyectoRepo = proyectoRepo;
            _categoriaRepo = categoriaRepo;
            _usuarioRepo = usuarioRepo;
        }
         
        public override EventoBase CrearEvento(Evento evento, Usuario usuario)
        {
            return new EventoParticipante(
                evento,
                usuario,
                _participanteRepo,
                _grupoRepo,
                _proyectoRepo,
                _categoriaRepo,
                _usuarioRepo
            );
        }
    }
}