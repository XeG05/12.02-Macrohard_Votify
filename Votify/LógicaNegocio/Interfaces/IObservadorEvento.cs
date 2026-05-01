using Votify.LógicaNegocio.Observador;

namespace Votify.LógicaNegocio.Interfaces
{

    public interface IObservadorEvento
    {
        Task Actualizar(EstadoNotificacionEvento estado);
    }
}