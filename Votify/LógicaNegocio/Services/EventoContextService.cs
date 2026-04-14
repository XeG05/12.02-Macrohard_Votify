using Votify.LógicaNegocio.Domain;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Services
{

    public class EventoContextService
    {
        private Evento? _eventoActual;
        private string? _rolActual;
        private long? _idEventoActual;

        public Evento? EventoActual => _eventoActual;
        public string? RolActual => _rolActual;
        public long? IdEventoActual => _idEventoActual;
        public bool EstaEnEvento => _eventoActual != null;

        public event Action? OnCambio;

        public void EstablecerEvento(Evento evento, string rol, long idEvento)
        {
            _eventoActual = evento;
            _rolActual = rol;
            _idEventoActual = idEvento;
            OnCambio?.Invoke();
        }

        public void SalirEvento()
        {
            _eventoActual = null;
            _rolActual = null;
            _idEventoActual = null;
            OnCambio?.Invoke();
        }

        public bool EsAdministrador => _rolActual == "ADMIN";
        public bool EsJurado => _rolActual == "JURADO";
        public bool EsParticipante => _rolActual == "PARTICIPANTE";
        public bool EsVotante => _rolActual == "VOTANTE";
    }
}