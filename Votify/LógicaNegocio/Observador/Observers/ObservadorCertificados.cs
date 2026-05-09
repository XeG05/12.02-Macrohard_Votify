using Microsoft.EntityFrameworkCore;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Interfaces;
using Votify.LógicaNegocio.Observador.Interfaces;
using Votify.Persistencia;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Observador.Observers
{
    public class ObservadorCertificados : IObservadorEvento
    {
        private readonly VotifyContext _context;
        private readonly ICertificadoService _certificadoService;
        private readonly IEmailService _emailService;
        private readonly INotificacionService _notificacionService;
        private readonly ILogger<ObservadorCertificados> _logger;

        public ObservadorCertificados(
            VotifyContext context,
            ICertificadoService certificadoService,
            IEmailService emailService,
            INotificacionService notificacionService,
            ILogger<ObservadorCertificados> logger)
        {
            _context = context;
            _certificadoService = certificadoService;
            _emailService = emailService;
            _notificacionService = notificacionService;
            _logger = logger;
        }

        public async Task ActualizarAsync(IEventoSujeto sujeto)
        {
            if (sujeto.FaseActual != FasesEvento.Resultados)
            {
                return;
            }

            var idEvento = sujeto.Evento.Id;

            var evento = await _context.Eventos
                .Include(e => e.Categorias)
                    .ThenInclude(c => c.Criterios)
                .Include(e => e.Categorias)
                    .ThenInclude(c => c.Grupos)
                        .ThenInclude(g => g.Participantes)
                            .ThenInclude(p => p.CorreoUsuarioNavigation)
                .Include(e => e.Categorias)
                    .ThenInclude(c => c.Grupos)
                        .ThenInclude(g => g.Proyecto)
                            .ThenInclude(p => p!.Votos)
                .AsSplitQuery()
                .FirstOrDefaultAsync(e => e.Id == idEvento);

            if (evento is null)
            {
                _logger.LogWarning("ObservadorCertificados: evento {Id} no encontrado.", idEvento);
                return;
            }

            var fechaFin = evento.FechaFinVotacionExperta;

            foreach (var categoria in evento.Categorias)
            {
                var ranking = categoria.Grupos
                    .Select(g => new
                    {
                        Grupo = g,
                        Puntuacion = CalcularPuntuacionProyecto(g.Proyecto, categoria.Criterios)
                    })
                    .OrderByDescending(x => x.Puntuacion)
                    .ToList();

                for (int i = 0; i < ranking.Count; i++)
                {
                    var grupo = ranking[i].Grupo;
                    int posicion = i + 1;

                    foreach (var participante in grupo.Participantes)
                    {
                        await EnviarCertificadoSiNoEnviadoAsync(
                            evento, categoria, grupo, participante, posicion, fechaFin);
                    }
                }
            }
        }

        private async Task EnviarCertificadoSiNoEnviadoAsync(
            Evento evento, Categorium categoria, Grupo grupo,
            Participante participante, int posicion, DateOnly fechaFin)
        {
            var correo = participante.CorreoUsuario;
            var claveUnica = $"{evento.Id}-{TipoNotificacion.CertificadoEnviado}-{correo}";

            var yaEnviado = await _context.Notificacions
                .AnyAsync(n => n.ClaveUnica == claveUnica);

            if (yaEnviado)
            {
                return;
            }

            var nombreParticipante = participante.CorreoUsuarioNavigation?.Nombre ?? correo;

            var datos = new DatosCertificadosDto
            {
                NombreParticipante = nombreParticipante,
                NombreEvento = evento.Nombre,
                NombreCategoria = categoria.Nombre,
                NombreGrupo = grupo.Nombre,
                Posicion = posicion,
                FechaFinalizacion = fechaFin
            };

            try
            {
                var pdf = _certificadoService.Generar(datos);
                var nombreArchivo = $"Certificado_{Sanitizar(evento.Nombre)}_{Sanitizar(nombreParticipante)}.pdf";

                await _emailService.EnviarCertificadoAsync(
                    correo, evento.Nombre, nombreParticipante, pdf, nombreArchivo);

                await _notificacionService.CrearSiNoExisteAsync(
                    correo,
                    evento.Id,
                    TipoNotificacion.CertificadoEnviado,
                    "Certificado disponible",
                    $"Te hemos enviado por email tu certificado del evento \"{evento.Nombre}\"."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error al enviar certificado a {Correo} del evento {EventoId}.",
                    correo, evento.Id);
            }
        }
        private static double CalcularPuntuacionProyecto(Proyecto? proyecto, ICollection<Criterio> criterios)
        {
            if (proyecto is null || proyecto.Votos.Count == 0)
            {
                return 0d;
            }

            double total = 0d;

            foreach (var criterio in criterios)
            {
                var votosCriterio = proyecto.Votos
                    .Where(v => v.IdCriterio == criterio.Id && v.Nota.HasValue)
                    .ToList();

                if (votosCriterio.Count == 0)
                {
                    continue;
                }

                double media = votosCriterio.Average(v => (double)v.Nota!.Value);
                total += media * criterio.Ponderacion;
            }

            return total;
        }

        private static string Sanitizar(string s)
        {
            var invalidos = Path.GetInvalidFileNameChars();
            var limpio = new string(s.Where(c => !invalidos.Contains(c) && c != ' ').ToArray());
            return string.IsNullOrEmpty(limpio) ? "x" : limpio;
        }
    }
}