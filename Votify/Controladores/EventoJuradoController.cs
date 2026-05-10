using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Interfaces;
using Votify.Persistencia.Entities;
using Votify.LógicaNegocio.Services;

namespace Votify.Controladores
{
    [ApiController]
    [Route("api/jurado")]
    [Authorize]
    public class EventoJuradoController : ControllerBase
    {
        private readonly IEventoJuradoService _eventoJuradoService;
        private readonly IEventoService _eventoService;
        private readonly SíntesisFeedbackService _sintesisFeedbackService;

        public EventoJuradoController(
            IEventoJuradoService eventoJuradoService, 
            IEventoService eventoService,
            SíntesisFeedbackService sintesisFeedbackService)
        {
            _eventoJuradoService = eventoJuradoService;
            _eventoService = eventoService;
            _sintesisFeedbackService = sintesisFeedbackService;
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
                var proyecto = await _eventoService.RecuperarProyecto(proyectoId);

                Console.WriteLine($"[EventoJuradoController] Proyecto recuperado: {proyecto?.Nombre}");

                if (proyecto != null)
                {
                    // Añadir la síntesis a cada criterio
                    foreach (var cr in comentarios)
                    {
                        var comentariosArray = cr.Comentarios.ToArray();
                        if (comentariosArray.Any())
                        {
                            Console.WriteLine($"[EventoJuradoController] Procesando sintesis para criterio {cr.CriterioId} ({cr.NombreCriterio}) con {comentariosArray.Length} comentarios");
                            var sintesis = await _sintesisFeedbackService.ObtenerSintesisCriterioAsync(proyecto, cr.CriterioId, comentariosArray);
                            cr.Sintesis = sintesis;
                        }
                    }
                }

                return Ok(comentarios);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EventoJuradoController] ERROR: {ex.Message}\n{ex.StackTrace}");
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
                var resultado = await _eventoService.ObtenerResultadoProyecto(proyectoId);

                return Ok(new 
                { 
                    TotalVotos = resultado.TotalVotos,
                    ResultadoVotos = resultado.ResultadoVotos,
                    PosicionRanking = resultado.PosicionRanking,
                    VotosPorCriterio = resultado.VotosPorCriterio
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{eventoId}/categoria/{categoriaId}/proyecto/{proyectoId}/comentariosPopulares")]
        public async Task<IActionResult> GetComentariosPopulares(long proyectoId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var comentarios = await _eventoJuradoService.ObtenerComentariosPopulares(proyectoId);
                return Ok(comentarios);
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