using System.Net;
using System.Net.Mail;
using Votify.LógicaNegocio.Interfaces;
using Votify.LógicaNegocio.Templates.EmailTemplates;

namespace Votify.LógicaNegocio.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarInvitacionAsync(
                string correoDestino,
                string nombreEvento,
                string nombreCreador,
                string urlAceptacion)
        {
            var plantilla = new InvitacionParticipanteEmail(_config);

            await plantilla.EnviarAsync(
                correoDestino,
                nombreEvento,
                nombreCreador,
                urlAceptacion);
        }

        public async Task EnviarInvitacionJuradoAsync(
            string correoDestino,
            string nombreEvento,
            string nombreCreador,
            string nombreCategoria,
            string urlAceptacion)
        {
            var plantilla = new InvitacionJuradoEmail(
                _config,
                nombreCategoria);

            await plantilla.EnviarAsync(
                correoDestino,
                nombreEvento,
                nombreCreador,
                urlAceptacion);
        }

        public async Task EnviarCertificadoAsync(string correoDestino, string nombreEvento,
                                                  string nombreParticipante, byte[] pdf, string nombreArchivo)
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
                    Subject = $"Tu certificado del evento: {nombreEvento}",
                    Body = $"""
                    Hola {nombreParticipante},

                    El evento "{nombreEvento}" ha finalizado. Adjuntamos tu certificado de participación
                    con tus resultados.

                    ¡Gracias por participar!

                    — Equipo Votify
                    """,
                    IsBodyHtml = false
                };
                mensaje.To.Add(correoDestino);

                var adjunto = new Attachment(new MemoryStream(pdf), nombreArchivo, "application/pdf");
                mensaje.Attachments.Add(adjunto);

                await smtp.SendMailAsync(mensaje);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL ERROR] {ex.GetType().Name}: {ex.Message}");
                throw;
            }
        }

        public async Task EnviarReclamacionAsync(string correoAdministrador, string correoParticipante, string nombreEvento, string mensajeReclamacion)
        {
            try
            {
                using var smtp = new SmtpClient(_config["Smtp:Host"], int.Parse(_config["Smtp:Port"]!))
                {
                    Credentials = new NetworkCredential(_config["Smtp:User"], _config["Smtp:Pass"]),
                    EnableSsl = true
                };

                var mensaje = new MailMessage
                {
                    From = new MailAddress(_config["Smtp:User"]!, "Notificaciones Votify"),
                    Subject = $"Nueva Reclamación - Evento: {nombreEvento}",
                    Body = $"""
                    Hola administrador,

                    El participante ({correoParticipante}) ha enviado una reclamación para el evento "{nombreEvento}".

                    Mensaje:
                    {mensajeReclamacion}

                    Por favor, revise esta solicitud.

                    — Equipo Votify
                    """,
                    IsBodyHtml = false
                };
                mensaje.To.Add(correoAdministrador);

                await smtp.SendMailAsync(mensaje);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL ERROR] {ex.GetType().Name}: {ex.Message}");
                throw;
            }
        }
    }
}
