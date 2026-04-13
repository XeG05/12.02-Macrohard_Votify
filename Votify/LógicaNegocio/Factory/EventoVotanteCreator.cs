using Votify.LógicaNegocio.Domain;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Factory
{
    public class EventoVotanteCreator : EventoCreator
    {
        private readonly IRepository<Proyecto> _proyectoRepo;
        private readonly IRepository<Participante> _participanteRepo;
        private readonly IRepository<Grupo> _grupoRepo;
        private readonly IRepository<Usuario> _usuarioRepo;

        public EventoVotanteCreator(
            IRepository<Proyecto> proyectoRepo,
            IRepository<Participante> participanteRepo,
            IRepository<Grupo> grupoRepo,
            IRepository<Usuario> usuarioRepo)
        {
            _proyectoRepo = proyectoRepo;
            _participanteRepo = participanteRepo;
            _grupoRepo = grupoRepo;
            _usuarioRepo = usuarioRepo;
        }

        public override EventoBase CrearEvento(Evento evento, Usuario usuario)
        {
            return new EventoVotante(
                evento, usuario,_proyectoRepo, _participanteRepo, _grupoRepo, _usuarioRepo);
        }
    }
}
