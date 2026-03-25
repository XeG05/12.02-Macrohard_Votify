using Votify.Modelos.BusinessLogic.Domain;
using Votify.Modelos.Persistence.Interfaces;
namespace Votify.Modelos.BusinessLogic.Factory
{
    public class EventoVotanteCreator : EventoCreator
    {
        private readonly IRepository<Evento> _eventoRepo;
        private readonly IRepository<Proyecto> _proyectoRepo;
        private readonly IRepository<Participante> _participanteRepo;
        private readonly IRepository<Grupo> _grupoRepo;
        private readonly IRepository<Categorium> _categoriumRepo;
        private readonly IRepository<Rol> _rolRepo;
        private readonly IRepository<Criterio> _criterioRepo;
        private readonly IRepository<Voto> _votoRepo;
        private readonly IRepository<Comentario> _comentarioRepo;
        private readonly IRepository<Usuario> _usuarioRepo;

        public EventoVotanteCreator(
            IRepository<Evento> eventoRepo,
            IRepository<Proyecto> proyectoRepo,
            IRepository<Participante> participanteRepo,
            IRepository<Grupo> grupoRepo,
            IRepository<Categorium> categoriumRepo,
            IRepository<Rol> rolRepo, 
            IRepository<Criterio> criterioRepo,
            IRepository<Voto> votoRepo,
            IRepository<Comentario> comentarioRepo,
            IRepository<Usuario> usuarioRepo)
        {
            _eventoRepo = eventoRepo;
            _proyectoRepo = proyectoRepo;
            _participanteRepo = participanteRepo;
            _grupoRepo = grupoRepo;
            _categoriumRepo = categoriumRepo;
            _rolRepo = rolRepo;
            _criterioRepo = criterioRepo;
            _votoRepo = votoRepo;
            _comentarioRepo = comentarioRepo;
            _usuarioRepo = usuarioRepo;
        }

        public override EventoBase CrearEvento(Evento evento, Usuario usuario)
        {
            return new EventoVotante(evento, usuario, _eventoRepo, _proyectoRepo, _participanteRepo, _categoriumRepo, _grupoRepo, _rolRepo, _criterioRepo, _votoRepo, _comentarioRepo, _usuarioRepo);
        }
    }
}