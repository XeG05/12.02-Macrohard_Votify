namespace Votify.LógicaNegocio.Interfaces
{
    public interface IEmailService
    {
        Task EnviarInvitacionAsync(string correoDestino, string nombreEvento,
                                   string nombreCreador, string urlAceptacion);
        Task EnviarInvitacionJuradoAsync(string correoDestino, string nombreEvento,
                                          string nombreCreador, string nombreCategoria, string urlAceptacion);

        Task EnviarCertificadoAsync(string correoDestino, string nombreEvento, string nombreParticipante,
                                    byte[] pdf, string nombreArchivo);
    }
}
