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
        private readonly IRepository<Categorium> _categoriumRepo;

        public EventoVotanteCreator(
            IRepository<Proyecto> proyectoRepo,
            IRepository<Participante> participanteRepo,
            IRepository<Grupo> grupoRepo,
            IRepository<Usuario> usuarioRepo,
            IRepository<Categorium> categoriumRepo)
        {
            _proyectoRepo = proyectoRepo;
            _participanteRepo = participanteRepo;
            _grupoRepo = grupoRepo;
            _usuarioRepo = usuarioRepo;
            _categoriumRepo = categoriumRepo;
        }

        public override EventoBase CrearEvento(Evento evento, Usuario usuario)
        {
            return new EventoVotante(
                evento, usuario,_proyectoRepo, _participanteRepo, _grupoRepo, _usuarioRepo, _categoriumRepo);
        }
    }
}
