using System.Net;
using System.Net.Mail;

namespace Votify.LógicaNegocio.Templates.EmailTemplates
{
    public abstract class InvitacionEmailTemplate
    {
        protected readonly IConfiguration _config;

        protected InvitacionEmailTemplate(IConfiguration config)
        {
            _config = config;
        }


        public async Task EnviarAsync(
            string correoDestino,
            string nombreEvento,
            string nombreCreador,
            string urlAceptacion)
        {
            try
            {
                using var smtp = CrearClienteSmtp();

                var mensaje = new MailMessage
                {
                    From = new MailAddress(_config["Email:Usuario"]!),
                    Subject = ObtenerAsunto(nombreEvento),
                    Body = ObtenerBody(
                        nombreEvento,
                        nombreCreador,
                        urlAceptacion),
                    IsBodyHtml = false
                };

                mensaje.To.Add(correoDestino);

                await smtp.SendMailAsync(mensaje);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL ERROR] {ex.GetType().Name}: {ex.Message}");
                throw;
            }
        }


        protected virtual SmtpClient CrearClienteSmtp()
        {
            return new SmtpClient(_config["Email:Host"])
            {
                Port = int.Parse(_config["Email:Port"]!),
                Credentials = new NetworkCredential(
                    _config["Email:Usuario"],
                    _config["Email:Contraseña"]),
                EnableSsl = true
            };
        }


        protected abstract string ObtenerAsunto(string nombreEvento);

        protected abstract string ObtenerBody(
            string nombreEvento,
            string nombreCreador,
            string urlAceptacion
        );
    }


}
