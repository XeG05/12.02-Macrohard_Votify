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
        public IActionResult GetMisCategorias(long eventoId)
        {
            // Lógica para obtener las categorías del jurado para el evento especificado
            return Ok();
        }
    }
}
