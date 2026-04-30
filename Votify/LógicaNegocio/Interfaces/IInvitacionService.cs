using Votify.LógicaNegocio.DTOs;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Interfaces
{
    public interface IInvitacionService
    {
        Task<InvitacionPendiente?> ObtenerInvitacionValida(string token);
        Task<InvitacionDto?> ObtenerInvitacionInfoAsync(string token);
        Task<InvitacionDto?> ObtenerInvitacionJuradoInfoAsync(string token);
        Task<string> AceptarInvitacionJuradoAsync(
            string token, string contraseña, string? nombre, string? contraseña2);
        Task<string> AceptarInvitacionAsync(
            string token, string contraseña, string? nombre, string? contraseña2);
    }
}
