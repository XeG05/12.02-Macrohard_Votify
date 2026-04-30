using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Interfaces;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Observador
{
    public class EventoSujeto : IEventoSujeto
    {
        private readonly List<IObservadorEvento> _observadores = new();

        private EstadoNotificacionEvento? _estado;

        public void Attach(IObservadorEvento observador)
        {
            if (!_observadores.Contains(observador))
            {
                _observadores.Add(observador);
            }
        }

        public void Detach(IObservadorEvento observador)
        {
            if (_observadores.Contains(observador))
            {
                _observadores.Remove(observador);
            }
        }

        public EstadoNotificacionEvento GetEstado()
        {
            if (_estado == null)
            {
                throw new InvalidOperationException("El estado del sujeto todavía no ha sido establecido.");
            }

            return _estado;
        }

        public async Task SetEstado(EstadoNotificacionEvento estado)
        {
            _estado = estado;

            await Notificar();
        }

        public async Task Notificar()
        {
            foreach (var observador in _observadores)
            {
                await observador.Actualizar();
            }
        }

        public async Task NotificarCambioFase(
            Evento evento,
            FasesEvento faseAnterior,
            FasesEvento faseNueva)
        {
            var estado = new EstadoNotificacionEvento
            {
                Evento = evento,
                TipoCambio = TipoCambioEstado.CambioFase,
                FaseAnterior = faseAnterior,
                FaseNueva = faseNueva
            };

            await SetEstado(estado);
        }

        public async Task NotificarRecordatorio(
            Evento evento,
            TipoRecordatorio tipoRecordatorio)
        {
            var estado = new EstadoNotificacionEvento
            {
                Evento = evento,
                TipoCambio = TipoCambioEstado.Recordatorio,
                TipoRecordatorio = tipoRecordatorio
            };

            await SetEstado(estado);
        }
    }
}
