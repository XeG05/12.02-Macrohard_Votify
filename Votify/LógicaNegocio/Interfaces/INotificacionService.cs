using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Enums;

namespace Votify.LógicaNegocio.Interfaces
{

    public interface INotificacionService
    {
        Task CrearSiNoExisteAsync(
            string correo,
            long idEvento,
            TipoNotificacion tipo,
            string titulo,
            string contenido);

        Task<List<NotificacionDto>> ObtenerPorUsuarioAsync(string correo);

        Task MarcarComoLeidaAsync(long idNotificacion, string correo);

        Task MarcarTodasComoLeidasAsync(string correo);

        Task EliminarAsync(long idNotificacion, string correo);
    }
}