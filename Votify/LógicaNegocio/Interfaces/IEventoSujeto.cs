using Votify.LógicaNegocio.Observador;


namespace Votify.LógicaNegocio.Interfaces
{
    public interface IEventoSujeto
    {
        void Attach(IObservadorEvento observador);
        void Detach(IObservadorEvento observador);
        Task Notificar(EstadoNotificacionEvento estado);
    }
}
