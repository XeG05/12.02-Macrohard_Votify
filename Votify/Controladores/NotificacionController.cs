using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Votify.LógicaNegocio.Interfaces;
using Votify.LógicaNegocio.Services;

namespace Votify.Controladores
{

    [ApiController]
    [Authorize]
    [Route("api/notificaciones")]
    public class NotificacionController : ControllerBase
    {
        private readonly INotificacionService _notificacionService;
        private readonly EventoFaseService _eventoFaseService;

        public NotificacionController(
            INotificacionService notificacionService,
            EventoFaseService eventoFaseService)
        {
            _notificacionService = notificacionService;
            _eventoFaseService = eventoFaseService;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerMisNotificaciones()
        {
            var correo = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrWhiteSpace(correo))
            {
                return Unauthorized();
            }

            await _eventoFaseService.RevisarEventosAsync();

            var notificaciones = await _notificacionService.ObtenerPorUsuarioAsync(correo);

            return Ok(notificaciones);
        }

        [HttpPut("{id:long}/leer")]
        public async Task<IActionResult> MarcarComoLeida(long id)
        {
            var correo = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrWhiteSpace(correo))
            {
                return Unauthorized();
            }

            await _notificacionService.MarcarComoLeidaAsync(id, correo);

            return NoContent();
        }

        [HttpPut("leer-todas")]
        public async Task<IActionResult> MarcarTodasComoLeidas()
        {
            var correo = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrWhiteSpace(correo))
            {
                return Unauthorized();
            }

            await _notificacionService.MarcarTodasComoLeidasAsync(correo);

            return NoContent();
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Eliminar(long id)
        {
            var correo = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrWhiteSpace(correo))
            {
                return Unauthorized();
            }

            await _notificacionService.EliminarAsync(id, correo);

            return NoContent();
        }
    }
}