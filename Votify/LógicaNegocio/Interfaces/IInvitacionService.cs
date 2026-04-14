using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Interfaces
{
    public interface IInvitacionService
    {
        Task<InvitacionPendiente?> ObtenerInvitacionValida(string token);
        Task<string> AceptarInvitacionAsync(
            string token, string contraseña, string? nombre, string? contraseña2);
    }
}
