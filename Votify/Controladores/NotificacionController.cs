using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Votify.LógicaNegocio.Interfaces;
using Microsoft.EntityFrameworkCore;
using Votify.LógicaNegocio.Enums;
using Votify.Persistencia;
using Votify.LógicaNegocio.Observador.Interfaces;

namespace Votify.Controladores
{

    [ApiController]
    [Authorize]
    [Route("api/notificaciones")]
    public class NotificacionController : ControllerBase
    {
        private readonly INotificacionService _notificacionService;
        private readonly IEventoSujeto _eventoSujeto;
        private readonly VotifyContext _context;

        public NotificacionController(
            INotificacionService notificacionService,
            IEventoSujeto eventoSujeto,
            IEnumerable<IObservadorEvento> observadores,
             VotifyContext context)
        {
            _notificacionService = notificacionService;
            _eventoSujeto = eventoSujeto;
            _context = context;

            foreach (var observador in observadores)
            {
                _eventoSujeto.Attach(observador);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerMisNotificaciones()
        {
            var correo = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrWhiteSpace(correo))
            {
                return Unauthorized();
            }

            await RevisarEventosAsync();

            var notificaciones = await _notificacionService.ObtenerPorUsuarioAsync(correo);

            return Ok(notificaciones);
        }

        private async Task RevisarEventosAsync()
        {
            var eventos = await _context.Eventos
                .Where(e => e.FaseActual != FasesEvento.Resultados)
                .Include(e => e.Roles)
                .ToListAsync();

            foreach (var evento in eventos)
            {
                await _eventoSujeto.SetEstadoAsync(evento);
            }

            await _context.SaveChangesAsync();
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