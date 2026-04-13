using Votify.LógicaNegocio.Domain;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Factory
{
    public class EventoAdministradorCreator : EventoCreator
    {
        private readonly IRepository<Evento> _eventoRepo;
        private readonly IRepository<Proyecto> _proyectoRepo;
        private readonly IRepository<Participante> _participanteRepo;
        private readonly IRepository<Categorium> _categoriumRepo;
        private readonly IRepository<Grupo> _grupoRepo;
        private readonly IRepository<Voto> _votoRepo;

        public EventoAdministradorCreator(
            IRepository<Evento> eventoRepo,
            IRepository<Proyecto> proyectoRepo,
            IRepository<Participante> participanteRepo,
            IRepository<Categorium> categoriumRepo,
            IRepository<Grupo> grupoRepo,
            IRepository<Voto> votoRepo)
        {
            _eventoRepo = eventoRepo;
            _proyectoRepo = proyectoRepo;
            _participanteRepo = participanteRepo;
            _categoriumRepo = categoriumRepo;
            _grupoRepo = grupoRepo;
            _votoRepo = votoRepo;
        }

        public override EventoBase CrearEvento(Evento evento, Usuario usuario)
        {
            return new EventoAdministrador(
                evento, usuario,
                _eventoRepo, _proyectoRepo,
                _participanteRepo, _categoriumRepo, _grupoRepo, _votoRepo);
        }
    }
}
