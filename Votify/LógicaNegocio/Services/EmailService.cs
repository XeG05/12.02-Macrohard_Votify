using System.Net;
using System.Net.Mail;
using Votify.LógicaNegocio.Interfaces;

namespace Votify.LógicaNegocio.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarInvitacionAsync(string correoDestino, string nombreEvento,
                                             string nombreCreador, string urlAceptacion)
        {
            try
            {
                var smtp = new SmtpClient(_config["Email:Host"])
                {
                    Port = int.Parse(_config["Email:Port"]!),
                    Credentials = new NetworkCredential(
                        _config["Email:Usuario"],
                        _config["Email:Contraseña"]),
                    EnableSsl = true
                };

                var mensaje = new MailMessage
                {
                    From = new MailAddress(_config["Email:Usuario"]!),
                    Subject = $"Invitación al evento: {nombreEvento}",
                    Body = $"""
                    Hola,

                    {nombreCreador} te ha invitado a participar en "{nombreEvento}" en Votify.

                    Acepta tu invitación aquí:
                    {urlAceptacion}

                    El enlace caduca en 7 días.
                    """,
                    IsBodyHtml = false
                };
                mensaje.To.Add(correoDestino);

                await smtp.SendMailAsync(mensaje);
            }
            catch (Exception ex)
            {
                // Verás el error exacto en la consola de Visual Studio
                Console.WriteLine($"[EMAIL ERROR] {ex.GetType().Name}: {ex.Message}");
                throw; // relanza para que el componente lo muestre
            }
        }
    }
}
