namespace Votify.Modelos.BusinessLogic.Interfaces
{
    public interface IEmailService
    {
        Task EnviarInvitacionAsync(string correoDestino, string nombreEvento,
                               string nombreCreador, string urlAceptacion);
    }
}
