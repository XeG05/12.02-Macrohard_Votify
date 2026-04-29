using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Exceptions;
using Votify.LógicaNegocio.Interfaces;
using Votify.Persistencia.Entities;

namespace Votify.Controladores
{
    [ApiController]
    [Route("api/evento")]
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

        // ── Mappers ──────────────────────────────────────────────────────

        private static EventoDto MapEvento(Evento e) => new()
        {
            Id = e.Id,
            Nombre = e.Nombre,
            Descripcion = e.Descripcion ?? "",
            FechaFinInscripciones = e.FechaFinInscripciones,
            FechaFinEntrega = e.FechaFinEntrega,
            FechaFinVotaciones = e.FechaFinVotaciones,
            FechaFinEvaluacionExperta = e.FechaFinEvaluacionExperta,
            TieneGrupos = e.TieneGrupos,
            GruposCompletos = e.GruposCompletos,
            ParticipantesPorGrupo = e.ParticipantesPorGrupo,
            Estado = e.Estado,
            TipoComentario = e.TipoComentario,
            TipoVotacion = e.TipoVotacion,
            NumVotarProyectos = e.NumVotarProyectos,
            PuntosARepartir = e.PuntosARepartir,
        };

        private static GrupoDto MapGrupo(Grupo g) => new()
        {
            Id = g.Id,
            Nombre = g.Nombre,
            IdCategoria = g.IdCategoria
        };

        private static CategoriaDto MapCategoria(Categorium c) => new()
        {
            Id = c.Id,
            Nombre = c.Nombre,
            Descripcion = c.Descripcion ?? "",
            IdEvento = c.IdEvento,
            PonderacionPopular = c.PonderacionPopular,
            PonderacionExperto = c.PonderacionExperto,
            CriteriosSonPorcentaje = c.CriteriosSonPorcentaje,
            Criterios = c.Criterios?.Select(cr => new CriterioDto
            {
                Id = cr.Id,
                Nombre = cr.Nombre,
                Descripcion = cr.Descripcion ?? "",
                Ponderacion = cr.Ponderacion
            }).ToList() ?? new()
        };

        private static ProyectoDto MapProyecto(Proyecto p) => new()
        {
            Id = p.Id,
            Nombre = p.Nombre,
            IdGrupo = p.IdGrupo,
            DescripcionBreve = p.DescripcionBreve,
            Descripcion = p.Descripcion,
            Imagenes = p.Imagenes?.Select(i => Convert.ToBase64String(i.Archivo)).ToList() ?? new(),
            TieneArchivo = p.Archivo != null && p.Archivo.Length > 0
        };

        private static MiembroGrupoDto MapMiembro(Usuario u) => new()
        {
            Nombre = u.Nombre,
            Correo = u.Correo
        };

        /// Eventos creados por el usuario autenticado.
        [HttpGet("creados")]
        public async Task<IActionResult> GetEventosCreados()
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var eventos = await _eventoService.GetEventosCreados(correo);
                return Ok(eventos.Select(MapEvento));
            }
            catch (NotFoundException ex) { return NotFound(new ErrorDto { Error = ex.Message }); }
            catch (Exception ex) { return BadRequest(new ErrorDto { Error = ex.Message }); }
        }

        /// Eventos en los que el usuario autenticado participa (no como admin).
        [HttpGet("participacion")]
        public async Task<IActionResult> GetEventosParticipacion()
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var eventos = await _eventoService.GetEventosParticipacion(correo);
                return Ok(eventos.Select(e => new EventoParticipanteDto
                {
                    Evento = MapEvento(e),
                    Rol = "Participante"
                }));
            }
            catch (NotFoundException ex) { return NotFound(new ErrorDto { Error = ex.Message }); }
            catch (Exception ex) { return BadRequest(new ErrorDto { Error = ex.Message }); }
        }

        /// Eventos disponibles que el usuario puede votar.
        [HttpGet("disponibles")]
        public async Task<IActionResult> GetEventosDisponibles()
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var eventos = await _eventoService.GetEventosDisponibles(correo);
                return Ok(eventos.Select(MapEvento));
            }
            catch (NotFoundException ex) { return NotFound(new ErrorDto { Error = ex.Message }); }
            catch (Exception ex) { return BadRequest(new ErrorDto { Error = ex.Message }); }
        }

        [HttpGet("experto")]
        public async Task<IActionResult> GetEventosJurados()
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var eventos = await _eventoService.GetEventosJurados(correo);
                return Ok(eventos.Select(e => new EventoJuradoDto
                {
                    Evento = MapEvento(e),
                    Rol = "Jurado"
                }));
            }
            catch (NotFoundException ex) { return NotFound(new ErrorDto { Error = ex.Message }); }
            catch (Exception ex) { return BadRequest(new ErrorDto { Error = ex.Message }); }
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
                return Ok(new EntradaEventoRespuestaDto
                {
                    Evento = MapEvento(entrada.Evento),
                    Rol = new RolInfoDto
                    {
                        EsAdministrador = entrada.TipoRol is Administrador,
                        EsParticipante = entrada.TipoRol is Participante,
                        EsVotante = entrada.TipoRol is Votante,
                        EsJurado = entrada.TipoRol is Jurado
                    }
                });
            }
            catch (NotFoundException ex) { return NotFound(new ErrorDto { Error = ex.Message }); }
            catch (UnauthorizedException ex) { return Unauthorized(new ErrorDto { Error = ex.Message }); }
            catch (Exception ex) { return BadRequest(new ErrorDto { Error = ex.Message }); }
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
                return CreatedAtAction(nameof(EntrarEvento), new { eventoId = evento.Id }, MapEvento(evento));
            }
            catch (ValidationException ex) { return UnprocessableEntity(new ErrorDto { Error = ex.Message }); }
            catch (ConflictException ex) { return Conflict(new ErrorDto { Error = ex.Message }); }
            catch (Exception ex) { return BadRequest(new ErrorDto { Error = ex.Message }); }
        }

        [HttpGet("{eventoId:long}/proyectos")]
        public async Task<IActionResult> GetProyectos(long eventoId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var proyectos = await _eventoService.ListaProyectos(eventoId);
                return Ok(proyectos.Select(MapProyecto));
            }
            catch (NotFoundException ex) { return NotFound(new ErrorDto { Error = ex.Message }); }
            catch (Exception ex) { return BadRequest(new ErrorDto { Error = ex.Message }); }
        }

        /// Categorías de un evento (con criterios incluidos).
        [HttpGet("{eventoId:long}/categorias")]
        public async Task<IActionResult> GetCategorias(long eventoId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var categorias = await _eventoService.CategoriasDelEvento(eventoId);
                return Ok(categorias.Select(MapCategoria));
            }
            catch (NotFoundException ex) { return NotFound(new ErrorDto { Error = ex.Message }); }
            catch (Exception ex) { return BadRequest(new ErrorDto { Error = ex.Message }); }
        }

        /// Datos de un grupo concreto.
        [HttpGet("grupo/{grupoId:long}")]
        public async Task<IActionResult> GetGrupo(long grupoId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var grupo = await _eventoService.GrupoProyecto(grupoId);
                if (grupo is null) return NotFound(new ErrorDto { Error = "Grupo no encontrado" });
                return Ok(MapGrupo(grupo));
            }
            catch (NotFoundException ex) { return NotFound(new ErrorDto { Error = ex.Message }); }
            catch (Exception ex) { return BadRequest(new ErrorDto { Error = ex.Message }); }
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
                if (proyecto is null) return NotFound(new ErrorDto { Error = "Proyecto no encontrado" });
                return Ok(MapProyecto(proyecto));
            }
            catch (NotFoundException ex) { return NotFound(new ErrorDto { Error = ex.Message }); }
            catch (Exception ex) { return BadRequest(new ErrorDto { Error = ex.Message }); }
        }

        [HttpGet("proyecto/{proyectoId:long}/descargar")]
        public async Task<IActionResult> DescargarProyecto(long proyectoId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var proyecto = await _eventoService.RecuperarProyecto(proyectoId);
                if (proyecto is null || proyecto.Archivo is null || proyecto.Archivo.Length == 0)
                    return NotFound(new ErrorDto { Error = "Archivo del proyecto no encontrado" });
                var nombreArchivo = $"{proyecto.Nombre}.zip";
                return File(proyecto.Archivo, "application/zip", nombreArchivo);
            }
            catch (NotFoundException ex) { return NotFound(new ErrorDto { Error = ex.Message }); }
            catch (Exception ex) { return BadRequest(new ErrorDto { Error = ex.Message }); }
        }

        [HttpGet("proyecto/{proyectoId:long}/grupo/{grupoId:long}/participantes")]
        public async Task<IActionResult> ParticipantesGrupo(long grupoId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var participantes = await _eventoService.ParticipantesGrupo(grupoId);
                if (participantes is null) return NotFound(new ErrorDto { Error = "Grupo o participantes no encontrado/s" });
                return Ok(participantes.Select(MapMiembro));
            }
            catch (NotFoundException ex) { return NotFound(new ErrorDto { Error = ex.Message }); }
            catch (Exception ex) { return BadRequest(new ErrorDto { Error = ex.Message }); }
        }

        /// Guarda un comentario en un proyecto
        [HttpPost("proyecto/comentar")]
        public async Task<IActionResult> ComentarProyecto([FromBody] ComentarioRequest request)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                await _eventoService.ComentarProyecto(request.IdEvento, correo, request.IdProyecto, request.IdCriterio, request.Comentario);
                return Ok(new { mensaje = "Comentario agregado exitosamente" });
            }
            catch (NotFoundException ex) { return NotFound(new ErrorDto { Error = ex.Message }); }
            catch (ValidationException ex) { return UnprocessableEntity(new ErrorDto { Error = ex.Message }); }
            catch (Exception ex) { return BadRequest(new ErrorDto { Error = ex.Message }); }
        }

        /// Obtiene el comentario de un usuario en un proyecto (si existe)
        [HttpGet("proyecto/{proyectoId:long}/comentario/usuario")]
        public async Task<IActionResult> ObtenerComentarioUsuario(long proyectoId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var comentarios = await _eventoService.ObtenerComentariosUsuario(proyectoId, correo);
                if (!comentarios.Any()) return NoContent();
                return Ok(comentarios);
            }
            catch (NotFoundException ex) { return NotFound(new ErrorDto { Error = ex.Message }); }
            catch (Exception ex) { return BadRequest(new ErrorDto { Error = ex.Message }); }
        }

        /// Obtiene el informacion basica del voto en un proyecto concreto
        [HttpGet("proyecto/{proyectoId:long}/evento/{eventoId:long}/voto-info")]
        public async Task<IActionResult> ObtenerInfoVotoSimple(long proyectoId, long eventoId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var info = await _eventoService.ObtenerInfoVotoPorCriterios(eventoId, proyectoId, correo);
                return Ok(info);
            }
            catch (NotFoundException ex) { return NotFound(new ErrorDto { Error = ex.Message }); }
            catch (Exception ex) { return BadRequest(new ErrorDto { Error = ex.Message }); }
        }

        /// Emite un voto al proyecto (Simple, Nota o Puntos según el tipo del evento)
        [HttpPost("proyecto/votar")]
        public async Task<IActionResult> VotarProyecto([FromBody] VotoRequest request)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                await _eventoService.VotarProyecto(request.IdEvento, request.IdProyecto, request.IdCriterio, request.Valor, correo);
                return Ok(new { mensaje = "Voto emitido exitosamente" });
            }
            catch (ConflictException ex) { return Conflict(new ErrorDto { Error = ex.Message }); }
            catch (ValidationException ex) { return UnprocessableEntity(new ErrorDto { Error = ex.Message }); }
            catch (NotFoundException ex) { return NotFound(new ErrorDto { Error = ex.Message }); }
            catch (Exception ex) { return BadRequest(new ErrorDto { Error = ex.Message }); }
        }
    }
}
