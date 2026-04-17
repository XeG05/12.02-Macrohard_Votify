using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Votify.LógicaNegocio.DTOs;
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
            TieneGrupos = e.TieneGrupos,
            GruposCompletos = e.GruposCompletos,
            ParticipantesPorGrupo = e.ParticipantesPorGrupo
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
            Criterios = c.Criterios?.Select(cr => new CriterioDto
            {
                Id = cr.Id,
                Nombre = cr.Nombre,
                Descripcion = cr.Descripcion ?? ""
            }).ToList() ?? new()
        };

        private static ProyectoDto MapProyecto(Proyecto p) => new()
        {
            Id = p.Id,
            Nombre = p.Nombre,
            IdGrupo = p.IdGrupo,
            DescripcionBreve = p.DescripcionBreve,
            Descripcion = p.Descripcion
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

            var eventos = await _eventoService.GetEventosCreados(correo);
            return Ok(eventos.Select(MapEvento));
        }

        /// Eventos en los que el usuario autenticado participa (no como admin).
        [HttpGet("participacion")]
        public async Task<IActionResult> GetEventosParticipacion()
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            var eventos = await _eventoService.GetEventosParticipacion(correo);
            return Ok(eventos.Select(MapEvento));
        }

        /// Eventos disponibles que el usuario puede votar.
        [HttpGet("disponibles")]
        public async Task<IActionResult> GetEventosDisponibles()
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            var eventos = await _eventoService.GetEventosDisponibles(correo);
            return Ok(eventos.Select(MapEvento));
        }
        /// Eventos con rol y grupo detallado (para Home).
        [HttpGet("participacion-detallada")]
        public async Task<IActionResult> GetParticipacionDetallada()
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            var participaciones = await _eventoService.GetParticipacionesDetalladas(correo);
            return Ok(participaciones.Select(p => new ParticipacionDto
            {
                Evento = MapEvento(p.Evento),
                Rol = p.RolTexto,
                Grupo = p.Grupo != null ? MapGrupo(p.Grupo) : null
            }));
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
                        EsAdministrador = entrada.TipoRol.Administrador != null,
                        EsParticipante = entrada.TipoRol.Participantes.Any(),
                        EsVotante = entrada.TipoRol.Votante != null,
                        EsJurado = entrada.TipoRol.Jurado != null
                    }
                });
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
                return CreatedAtAction(nameof(EntrarEvento), new { eventoId = evento.Id }, MapEvento(evento));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{eventoId:long}/proyectos")]
        public async Task<IActionResult> GetProyectos(long eventoId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            var proyectos = await _eventoService.ListaProyectos(eventoId);
            return Ok(proyectos.Select(MapProyecto));
        }

        /// Categorías de un evento (con criterios incluidos).
        [HttpGet("{eventoId:long}/categorias")]
        public async Task<IActionResult> GetCategorias(long eventoId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            var categorias = await _eventoService.CategoriasDelEvento(eventoId);
            return Ok(categorias.Select(MapCategoria));
        }

        /// Datos de un grupo concreto.
        [HttpGet("grupo/{grupoId:long}")]
        public async Task<IActionResult> GetGrupo(long grupoId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            var grupo = await _eventoService.GrupoProyecto(grupoId);
            if (grupo is null) return NotFound(new { error = "Grupo no encontrado" });
            return Ok(MapGrupo(grupo));
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
                if (proyecto is null) return NotFound(new { error = "Proyecto no encontrado" });
                return Ok(MapProyecto(proyecto));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("proyecto/{proyectoId:long}/grupo/{grupoId:long}/participantes")]
        public async Task<IActionResult> ParticipantesGrupo(long grupoId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var participantes = await _eventoService.ParticipantesGrupo(grupoId);
                if (participantes is null) return NotFound(new { error = "Grupo o participantes no encontrado/s" });
                return Ok(participantes.Select(MapMiembro));
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
                if (comentario is null) return NoContent();

                // IMPORTANTE: Devolvemos sólo las propiedades necesarias como objeto anónimo para evitar un problema de
                // referencia circular (el Comentario llama al Proyecto que llama a los Comentarios por la base de datos).
                return Ok(new { Contenido = comentario.Contenido });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// Obtiene el informacion basica del voto en un proyecto concreto
        [HttpGet("proyecto/{proyectoId:long}/evento/{eventoId:long}/voto-info")]
        public async Task<IActionResult> ObtenerInfoVotoSimple(long proyectoId, long eventoId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var info = await _eventoService.ObtenerInfoVotoSimple(eventoId, proyectoId, correo);
                int votosRestantes = Math.Max(0, 3 - info.VotosTotales);

                return Ok(new { HaVotado = info.HaVotado, VotosRestantes = Math.Max(0, 3 - info.VotosTotales) });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// Emite un voto estandar al proyecto
        [HttpPost("proyecto/votar")]
        public async Task<IActionResult> VotarProyectoSimple([FromBody] VotoRequest request)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                await _eventoService.VotarProyectoSimple(request.IdEvento, request.IdProyecto, correo);
                return Ok(new { mensaje = "Voto emitido exitosamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}