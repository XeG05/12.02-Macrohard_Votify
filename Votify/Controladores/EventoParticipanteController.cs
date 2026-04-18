using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Interfaces;
using Votify.LógicaNegocio.Services;
using Votify.Persistencia.Entities;

namespace Votify.Controladores
{
    [ApiController]
    [Route("api/participante")]
    [Authorize]
    public class EventoParticipanteController : ControllerBase
    {
        private readonly EventoParticipanteService _participanteService;

        public EventoParticipanteController(EventoParticipanteService participanteService)
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
            Criterios = c.Criterios?.Select(cr => new CriterioDto
            {
                Id = cr.Id,
                Nombre = cr.Nombre,
                Descripcion = cr.Descripcion ?? ""
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
                var grupo = await _participanteService.ObtenerMiGrupo(correo, eventoId, grupoId);
                if (grupo is null) return NotFound(new { error = "Grupo no encontrado" });
                return Ok(MapGrupo(grupo));
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
                var categoria = await _participanteService.ObtenerCategoriaGrupo(correo, eventoId, grupoId);
                if (categoria is null) return NotFound(new { error = "Categoría no encontrada" });
                return Ok(MapCategoria(categoria));
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
            IFormFileCollection? imagenes)
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

                await _participanteService.CrearProyecto(
                    nombreProyecto,
                    descripcionBreve ?? "",
                    descripcionCompleta ?? "",
                    archivoBytes,
                    imagenesBytes,
                    correo,
                    eventoId,
                    grupoId);

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
    }
}
