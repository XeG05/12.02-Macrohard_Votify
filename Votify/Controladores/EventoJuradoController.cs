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

        [HttpGet("{eventoId}/get-nombre")]
        public async Task<IActionResult> GetEventoNombre(long eventoId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var nombreEvento = await _eventoJuradoService.ObtenerNombreEvento(eventoId);
                return Ok(new { Nombre = nombreEvento });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{eventoId}/categoria/{categoriaId}")]
        public async Task<IActionResult> GetCategoria(long eventoId, long categoriaId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var categoria = await _eventoJuradoService.ObtenerCategoria(eventoId, categoriaId, correo);
                return Ok(categoria);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{eventoId}/categoria/{categoriaId}/proyectos")]
        public async Task<IActionResult> GetProyectosCategoria(long eventoId, long categoriaId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var proyectos = await _eventoJuradoService.ObtenerProyectosCategoriaParaJurado(correo, eventoId, categoriaId);
                return Ok(proyectos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{eventoId}/categoria/{categoriaId}/proyecto/{proyectoId}/comentarios")]
        public async Task<IActionResult> GetComentariosDeOtros(long eventoId, long categoriaId, long proyectoId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var comentarios = await _eventoJuradoService.ObtenerComentariosDeOtrosJueces(correo, eventoId, categoriaId, proyectoId);
                return Ok(comentarios);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{eventoId}/categoria/{categoriaId}/proyecto/{proyectoId}/resultados")]
        public async Task<IActionResult> GetResultadosVotacionPopular(long eventoId, long categoriaId, long proyectoId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var comentarios = await _eventoJuradoService.ObtenerComentariosPopulares(proyectoId);

                return Ok(new 
                { 
                    Comentarios = comentarios,
                    PosicionRanking = 1,
                    VotosRecibidos = 27
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{eventoId}/categoria/{categoriaId}/jurados")]
        public async Task<IActionResult> GetJuradosCategoria(long eventoId, long categoriaId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var jurados = await _eventoJuradoService.ObtenerJuradosCategoria(eventoId, categoriaId);
                return Ok(jurados);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}