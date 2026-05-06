
using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Observador.Interfaces;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Observador.Sujetos
{
    public class EventoSujeto : IEventoSujeto
    {
        private readonly List<IObservadorEvento> _observadores = new();
        private readonly ILogger<EventoSujeto> _logger;

        private Evento? _evento;
        public Evento Evento =>
            _evento ?? throw new InvalidOperationException("No hay evento cargado en el sujeto.");
        public FasesEvento? FaseAnterior { get; private set; }
        public FasesEvento FaseActual => Evento.FaseActual;


        public EventoSujeto(ILogger<EventoSujeto> logger, IEnumerable<IObservadorEvento> observadores)
        {
            _logger = logger;
            _observadores = observadores.ToList();
        }

        public void Attach(IObservadorEvento observador)
        {
            if (!_observadores.Contains(observador))
            {
                _observadores.Add(observador);
            }
        }

        public void Detach(IObservadorEvento observador)
        {
            _observadores.Remove(observador);
        }

        public async Task SetEstadoAsync(Evento evento)
        {
            var faseAnterior = evento.FaseActual;
            var hoy = DateOnly.FromDateTime(DateTime.Today);

            var haCambiado = evento.ActualizarFaseSiCorresponde();

            if (!haCambiado)
            {
                return;
            }

            _evento = evento;
            FaseAnterior = faseAnterior;

            await NotificarAsync();
        }

        public async Task NotificarAsync()
        {
            foreach (var observador in _observadores.ToList())
            {
                try
                {
                    await observador.ActualizarAsync(this);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error en observador {Observador} al procesar evento {EventoId}",
                        observador.GetType().Name,
                        Evento.Id
                    );
                }
            }
        }
    }
}