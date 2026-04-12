using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Interfaces;

namespace Votify.Controladores
{

    public class ComentarioRequest
    {
        public long IdEvento { get; set; }
        public long IdProyecto { get; set; }
        public string Comentario { get; set; } = string.Empty;
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize] //Hay que implementar JW Token para que detecte si estan logueados o no los usuarios,lo dejamos para mas adelante
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

        /// Recupera un proyecto por su Id.
        [HttpGet("proyecto/{proyectoId:long}")]
        public async Task<IActionResult> RecuperarProyecto(long proyectoId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var proyecto = await _eventoService.RecuperarProyecto(proyectoId);
                if (proyecto == null)
                {
                    return NotFound(new { error = "Proyecto no encontrado" });
                }
                return Ok(proyecto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("proyecto/{proyectoId:long}/grupo/{grupoId:long}/participantes")]
        public async Task<IActionResult> participantesGrupo(long grupoId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var participantes = await _eventoService.ParticipantesGrupo(grupoId);
                if (participantes == null)
                {
                    return NotFound(new { error = "Grupo o participantes no encontrado/s" });
                }
                return Ok(participantes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// Guarda un comentario en un proyecto
        [HttpPost("proyecto/comentar")]
        public async Task<IActionResult> ComentarProyecto([FromBody] ComentarioRequest request)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                // Usamos el 'correo' extraído del JWT por seguridad
                await _eventoService.ComentarProyecto(request.IdEvento, correo, request.IdProyecto, request.Comentario);
                return Ok(new { mensaje = "Comentario agregado exitosamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// Obtiene el comentario de un usuario en un proyecto (si existe)
        [HttpGet("proyecto/{proyectoId:long}/comentario/usuario")]
        public async Task<IActionResult> ObtenerComentarioUsuario(long proyectoId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var comentario = await _eventoService.ObtenerComentarioUsuario(proyectoId, correo);
                if (comentario == null)
                {
                    return NoContent();
                }

                // IMPORTANTE: Devolvemos sólo las propiedades necesarias como objeto anónimo para evitar un problema de
                // referencia circular (el Comentario llama al Proyecto que llama a los Comentarios por la base de datos).
                return Ok(new { Contenido = comentario.Contenido });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}