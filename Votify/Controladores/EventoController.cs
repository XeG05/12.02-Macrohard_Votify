using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Interfaces;

namespace Votify.Controladores
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventoController : ControllerBase
    {
        private readonly IEventoService _eventoService;

        public EventoController(IEventoService eventoService)
        {
            _eventoService = eventoService;
        }

        private string? ObtenerCorreo()
            => User.FindFirst(ClaimTypes.Name)?.Value;

        /// Eventos creados por el usuario autenticado.
        [HttpGet("creados")]
        public async Task<IActionResult> GetEventosCreados()
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            var eventos = await _eventoService.GetEventosCreados(correo);
            return Ok(eventos);
        }

        /// Eventos en los que el usuario autenticado participa (no como admin).
        [HttpGet("participacion")]
        public async Task<IActionResult> GetEventosParticipacion()
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            var eventos = await _eventoService.GetEventosParticipacion(correo);
            return Ok(eventos);
        }

        /// Eventos disponibles que el usuario puede votar.
        [HttpGet("disponibles")]
        public async Task<IActionResult> GetEventosDisponibles()
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            var eventos = await _eventoService.GetEventosDisponibles(correo);
            return Ok(eventos);
        }

        /// Entra a un evento concreto
        [HttpGet("{eventoId:long}")]
        public async Task<IActionResult> EntrarEvento(long eventoId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var entrada = await _eventoService.EntrarEvento(correo, eventoId);
                return Ok(entrada);
            }
            catch (Exception ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        /// Crea un nuevo evento. El creador se extrae del JWT.
        [HttpPost]
        public async Task<IActionResult> CrearEvento([FromBody] EventoCreacionDto dto)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var evento = await _eventoService.CrearEvento(correo, dto);
                // 201 Created con la ruta al nuevo recurso
                return CreatedAtAction(nameof(EntrarEvento), new { eventoId = evento.Id }, evento);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}