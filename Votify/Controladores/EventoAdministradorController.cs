using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Interfaces;

namespace Votify.Controladores
{
    [ApiController]
    [Route("api/administrador")]
    [Authorize]
    public class EventoAdministradorController : Controller
    {
        private readonly IEventoAdministradorService _eventoAdministradorService;

        public EventoAdministradorController(IEventoAdministradorService eventoAdministradorService)
        {
            _eventoAdministradorService = eventoAdministradorService;
        }

        [HttpGet("{eventoId:long}/categorias")]
        public async Task<IActionResult> GetCategorias(long eventoId)
        {
            var categorias = await _eventoAdministradorService.VerCategorias(eventoId);
            return Ok(categorias);
        }

        [HttpGet("categoria/{id}/grupos")]
        public async Task<IActionResult> GetGrupos(long id)
        {
            var grupos = await _eventoAdministradorService.VerGrupo(id);
            return Ok(grupos);
        }

        [HttpGet("grupo/{id}/participantes")]
        public async Task<IActionResult> GetParticipantes(long id)
        {
            var participantes = await _eventoAdministradorService.VerParticipantes(id);
            return Ok(participantes);
        }

        [HttpGet("categoria/{id}/proyectos")]
        public async Task<IActionResult> GetProyectos(long id)
        {
            var proyectos = await _eventoAdministradorService.VerProyectosCategoria(id);
            return Ok(proyectos);
        }

        [HttpGet("categoria/{id}/votos")]
        public async Task<IActionResult> GetVotos(long id)
        {
            var votos = await _eventoAdministradorService.TotalVotosCategoria(id);
            return Ok(votos);
        }

        [HttpGet("categoria/{id}/resultados")]
        public async Task<IActionResult> GetResultados(long id)
        {
            var resultados = await _eventoAdministradorService.ObtenerResultadosCategoria(id);
            return Ok(resultados);
        }

        [HttpGet("{eventoId:long}/fase")]
        public IActionResult GetFase(long eventoId)
        {
            var fase = _eventoAdministradorService.ObtenerFaseActual(eventoId);
            return Ok(new FaseDto { Fase = fase.ToString() });
        }

        [HttpPut("{eventoId:long}/nombre")]
        public async Task<IActionResult> CambiarNombre(long eventoId, [FromBody] string nuevoNombre)
        {
            await _eventoAdministradorService.CambiarNombre(eventoId, nuevoNombre);
            return NoContent();

        }

        [HttpPut("{eventoId:long}/descripcion")]
        public async Task<IActionResult> CambiarDescripcion(long eventoId, [FromBody] string nuevaDescripcion)
        {
            await _eventoAdministradorService.CambiarDescripcion(eventoId, nuevaDescripcion);
            return NoContent();
        }

        [HttpPost("{eventoId:long}/categoria/{id}/grupo")]
        public async Task<IActionResult> AgregarGrupo(long eventoId, long id, [FromBody] string nombreGrupo)
        {
            await _eventoAdministradorService.AñadirGrupo(nombreGrupo, id, eventoId);
            return NoContent();
        }

        [HttpDelete("{eventoId:long}/grupo/{grupoId}")]
        public async Task<IActionResult> EliminarGrupo(long eventoId, long grupoId)
        {
            await _eventoAdministradorService.EliminarGrupo(eventoId, grupoId);
            return NoContent();
        }

        [HttpPut("{eventoId:long}/fechas")]
        public async Task<IActionResult> ActualizarFechas(long eventoId, [FromBody] ActualizarFechasDto dto)
        {
            try
            {
                await _eventoAdministradorService.ActualizarFechas(
                    eventoId,
                    dto.FechaFinInscripciones,
                    dto.FechaFinEntrega,
                    dto.FechaFinVotaciones);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
