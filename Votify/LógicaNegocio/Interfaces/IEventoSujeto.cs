using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Observador;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Interfaces
{
    public interface IEventoSujeto
    {
        void Attach(IObservadorEvento observador);

        void Detach(IObservadorEvento observador);

        Task Notificar();

        EstadoNotificacionEvento GetEstado();

        Task SetEstado(EstadoNotificacionEvento estado);

        Task NotificarCambioFase(
            Evento evento,
            FasesEvento faseAnterior,
            FasesEvento faseNueva);

        Task NotificarRecordatorio(
            Evento evento,
            TipoRecordatorio tipoRecordatorio);
    }
}
