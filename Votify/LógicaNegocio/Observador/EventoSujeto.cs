using Microsoft.Extensions.Logging;
using Votify.LógicaNegocio.Interfaces;

namespace Votify.LógicaNegocio.Observador
{
    public class EventoSujeto : IEventoSujeto
    {
        private readonly List<IObservadorEvento> _observadores = new();
        private readonly ILogger<EventoSujeto> _logger;

        public EventoSujeto(ILogger<EventoSujeto> logger)
        {
            _logger = logger;
        }

        public void Attach(IObservadorEvento observador)
        {
            if (!_observadores.Contains(observador))
                _observadores.Add(observador);
        }

        public void Detach(IObservadorEvento observador)
        {
            _observadores.Remove(observador);
        }

        public async Task Notificar(EstadoNotificacionEvento estado)
        {
            foreach (var observador in _observadores.ToList())
            {
                try
                {
                    await observador.Actualizar(estado);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error en observador {Observador} al procesar evento {EventoId}",
                        observador.GetType().Name,
                        estado.Evento?.Id);
                }
            }
        }
    }
}