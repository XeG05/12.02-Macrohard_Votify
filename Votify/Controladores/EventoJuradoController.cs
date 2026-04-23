using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Interfaces;
using Votify.Persistencia.Entities;

namespace Votify.Controladores
{
    [ApiController]
    [Route("api/jurado")]
    [Authorize]
    public class EventoJuradoController : ControllerBase
    {
        private readonly IEventoJuradoService _eventoJuradoService;

        public EventoJuradoController(IEventoJuradoService eventoJuradoService)
        {
            _eventoJuradoService = eventoJuradoService;
        }

        private string? ObtenerCorreo()
    => User.FindFirst(ClaimTypes.Name)?.Value;

        [HttpGet("{eventoId}/mis-categorias")]
        public async Task<IActionResult> GetMisCategorias(long eventoId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var categorias = await _eventoJuradoService.ObtenerCategoriasJurado(correo, eventoId);
                return Ok(categorias);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

    }
}
