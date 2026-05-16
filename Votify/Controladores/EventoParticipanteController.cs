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
    [Route("api/participante")]
    [Authorize]
    public class EventoParticipanteController : ControllerBase
    {
        private readonly IEventoParticipanteService _participanteService;

        public EventoParticipanteController(IEventoParticipanteService participanteService)
        {
            _participanteService = participanteService;
        }

        private string? ObtenerCorreo()
            => User.FindFirst(ClaimTypes.Name)?.Value;

        // ── Mappers ──────────────────────────────────────────────────────

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


        /// Devuelve el grupo del participante en el evento.
        [HttpGet("{eventoId:long}/mi-grupo")]
        public async Task<IActionResult> GetMiGrupo(long eventoId, [FromQuery] long? grupoId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var grupo = await _participanteService.ObtenerMiGrupo(correo, eventoId, grupoId)
                    ?? throw new NotFoundException("Grupo no encontrado");
                return Ok(MapGrupo(grupo));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ErrorDto { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// Devuelve los miembros del grupo del participante.
        [HttpGet("{eventoId:long}/miembros-grupo")]
        public async Task<IActionResult> GetMiembrosGrupo(long eventoId, [FromQuery] long? grupoId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var miembros = await _participanteService.ObtenerMiembrosGrupo(correo, eventoId, grupoId);
                return Ok(miembros.Select(m => new MiembroGrupoDto
                {
                    Nombre = m.Nombre,
                    Correo = m.Correo
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// Devuelve la categoría del grupo del participante.
        [HttpGet("{eventoId:long}/mi-categoria")]
        public async Task<IActionResult> GetMiCategoria(long eventoId, [FromQuery] long? grupoId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var categoria = await _participanteService.ObtenerCategoriaGrupo(correo, eventoId, grupoId)
                    ?? throw new NotFoundException("Categoría no encontrada");
                return Ok(MapCategoria(categoria));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// Indica si el grupo del participante ya tiene proyecto entregado.
        [HttpGet("{eventoId:long}/tiene-proyecto")]
        public async Task<IActionResult> GetTieneProyecto(long eventoId, [FromQuery] long? grupoId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var tiene = await _participanteService.TieneProyecto(correo, eventoId, grupoId);
                return Ok(new { TieneProyecto = tiene });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// Entrega el proyecto del grupo (multipart/form-data).
        [HttpPost("{eventoId:long}/proyecto")]
        [RequestSizeLimit(50_000_000)]
        public async Task<IActionResult> CrearProyecto(
            long eventoId,
            [FromQuery] long? grupoId,
            [FromForm] string nombreProyecto,
            [FromForm] string? descripcionBreve,
            [FromForm] string? descripcionCompleta,
            IFormFile? archivo,
            List<IFormFile>? imagenes)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                // Leer el archivo principal
                byte[] archivoBytes = Array.Empty<byte>();
                if (archivo != null)
                {
                    using var ms = new MemoryStream();
                    await archivo.CopyToAsync(ms);
                    archivoBytes = ms.ToArray();
                }

                // Leer las imágenes
                var imagenesBytes = new List<byte[]>();
                if (imagenes != null)
                {
                    foreach (var img in imagenes)
                    {
                        using var ms = new MemoryStream();
                        await img.CopyToAsync(ms);
                        imagenesBytes.Add(ms.ToArray());
                    }
                }

                var request = new CrearProyectoRequest
                {
                    Correo = correo,
                    IdEvento = eventoId,
                    GrupoIdActivo = grupoId,
                    NombreProyecto = nombreProyecto,
                    DescripcionBreve = descripcionBreve ?? "",
                    DescripcionCompleta = descripcionCompleta ?? "",
                    Archivo = archivoBytes,
                    Imagenes = imagenesBytes
                };

                await _participanteService.CrearProyecto(request);

                return Created(string.Empty, new { mensaje = "Proyecto entregado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// Elimina el proyecto del grupo del participante.
        [HttpDelete("{eventoId:long}/proyecto")]
        public async Task<IActionResult> EliminarProyecto(long eventoId, [FromQuery] long? grupoId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                await _participanteService.EliminarProyecto(correo, eventoId, grupoId);
                return Ok(new { mensaje = "Proyecto eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// Devuelve el dashboard de resultados del participante.
        [HttpGet("{eventoId:long}/dashboard/{categoriaId:long}")]
        public async Task<IActionResult> GetDashboard(long eventoId, long categoriaId)
        {
            var correo = ObtenerCorreo();
            if (correo is null) return Unauthorized();

            try
            {
                var dashboard = await _participanteService.ObtenerDashboard(categoriaId, correo, eventoId);
                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// Devuelve la fase actual del evento.
        [HttpGet("{eventoId:long}/fase")]
        public IActionResult GetFase(long eventoId)
        {
            try
            {
                var fase = _participanteService.ObtenerFaseActual(eventoId);
                return Ok(new FaseDto { Fase = fase.ToString() });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{eventoId:long}/reclamacion")]
        public async Task<IActionResult> EnviarReclamacion(long eventoId, [FromBody] ReclamacionDto reclamacion)
        {
            var correo = ObtenerCorreo();
            if (string.IsNullOrEmpty(correo)) return Unauthorized();

            if (string.IsNullOrWhiteSpace(reclamacion.Mensaje))
                return BadRequest(new { error = "El mensaje no puede estar vacío" });

            try
            {
                await _participanteService.EnviarReclamacion(correo, eventoId, reclamacion.Mensaje);
                return Ok(new { mensaje = "Reclamación enviada correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
