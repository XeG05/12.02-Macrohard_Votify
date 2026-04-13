using Votify.LógicaNegocio.Domain;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Factory
{
    public class EventoParticipanteCreator : EventoCreator
    {
        private readonly IRepository<Participante> _participanteRepo;
        private readonly IRepository<Grupo> _grupoRepo;
        private readonly IRepository<Proyecto> _proyectoRepo;
        private readonly IRepository<Categorium> _categoriumRepo;
        private readonly IRepository<Usuario> _usuarioRepo;
        private readonly IRepository<Voto> _votoRepo;
        private readonly IRepository<Comentario> _comentarioRepo;

        public EventoParticipanteCreator(
            IRepository<Participante> participanteRepo,
            IRepository<Grupo> grupoRepo,
            IRepository<Proyecto> proyectoRepo,
            IRepository<Categorium> categoriumRepo,
            IRepository<Usuario> usuarioRepo,
            IRepository<Voto> votoRepo,
            IRepository<Comentario> comentarioRepo)
        {
            _participanteRepo = participanteRepo;
            _grupoRepo = grupoRepo;
            _proyectoRepo = proyectoRepo;
            _categoriumRepo = categoriumRepo;
            _usuarioRepo = usuarioRepo;
            _votoRepo = votoRepo;
            _comentarioRepo = comentarioRepo;
        }

        public override EventoBase CrearEvento(Evento evento, Usuario usuario)
        {
            return new EventoParticipante(
                evento, usuario,
                _participanteRepo, _grupoRepo, _proyectoRepo,
                _categoriumRepo, _usuarioRepo, _votoRepo, _comentarioRepo);
        }
    }
}
