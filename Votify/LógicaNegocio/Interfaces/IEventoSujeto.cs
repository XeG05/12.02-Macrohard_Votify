using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Observador;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Interfaces
{
    public interface IEventoSujeto
    {
        void Attach(IObservadorEvento observador);
        void Detach(IObservadorEvento observador);
        Task Notificar(EstadoNotificacionEvento estado);
    }
}
