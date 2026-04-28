using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Exceptions;
using Votify.LógicaNegocio.Interfaces;
using Votify.LógicaNegocio.Services;

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
            try
            {
                var categorias = await _eventoAdministradorService.VerCategorias(eventoId);
                return Ok(categorias);
            }
            catch (NotFoundException ex) { return NotFound(new ErrorDto { Error = ex.Message }); }
            catch (Exception ex) { return BadRequest(new ErrorDto { Error = ex.Message }); }
        }

        [HttpGet("categoria/{id}/grupos")]
        public async Task<IActionResult> GetGrupos(long id)
        {
            try
            {
                var grupos = await _eventoAdministradorService.VerGrupo(id);
                return Ok(grupos);
            }
            catch (NotFoundException ex) { return NotFound(new ErrorDto { Error = ex.Message }); }
            catch (Exception ex) { return BadRequest(new ErrorDto { Error = ex.Message }); }
        }

        [HttpGet("grupo/{id}/participantes")]
        public async Task<IActionResult> GetParticipantes(long id)
        {
            try
            {
                var participantes = await _eventoAdministradorService.VerParticipantes(id);
                return Ok(participantes);
            }
            catch (NotFoundException ex) { return NotFound(new ErrorDto { Error = ex.Message }); }
            catch (Exception ex) { return BadRequest(new ErrorDto { Error = ex.Message }); }
        }



        [HttpGet("categoria/{id}/proyectos")]
        public async Task<IActionResult> GetProyectos(long id)
        {
            try
            {
                var proyectos = await _eventoAdministradorService.VerProyectosCategoria(id);
                return Ok(proyectos);
            }
            catch (NotFoundException ex) { return NotFound(new ErrorDto { Error = ex.Message }); }
            catch (Exception ex) { return BadRequest(new ErrorDto { Error = ex.Message }); }
        }

        [HttpGet("categoria/{id}/votos")]
        public async Task<IActionResult> GetVotos(long id)
        {
            try
            {
                var votos = await _eventoAdministradorService.TotalVotosCategoria(id);
                return Ok(votos);
            }
            catch (NotFoundException ex) { return NotFound(new ErrorDto { Error = ex.Message }); }
            catch (Exception ex) { return BadRequest(new ErrorDto { Error = ex.Message }); }
        }

        [HttpGet("categoria/{id}/resultados")]
        public async Task<IActionResult> GetResultados(long id)
        {
            try
            {
                var resultados = await _eventoAdministradorService.ObtenerResultadosCategoria(id);
                return Ok(resultados);
            }
            catch (NotFoundException ex) { return NotFound(new ErrorDto { Error = ex.Message }); }
            catch (Exception ex) { return BadRequest(new ErrorDto { Error = ex.Message }); }
        }

        [HttpGet("{eventoId:long}/fase")]
        public IActionResult GetFase(long eventoId)
        {
            try
            {
                var fase = _eventoAdministradorService.ObtenerFaseActual(eventoId);
                return Ok(new FaseDto { Fase = fase.ToString() });
            }
            catch (NotFoundException ex) { return NotFound(new ErrorDto { Error = ex.Message }); }
            catch (Exception ex) { return BadRequest(new ErrorDto { Error = ex.Message }); }
        }

        [HttpPut("{eventoId:long}/nombre")]
        public async Task<IActionResult> CambiarNombre(long eventoId, [FromBody] string nuevoNombre)
        {
            try
            {
                await _eventoAdministradorService.CambiarNombre(eventoId, nuevoNombre);
                return NoContent();
            }
            catch (NotFoundException ex) { return NotFound(new ErrorDto { Error = ex.Message }); }
            catch (Exception ex) { return BadRequest(new ErrorDto { Error = ex.Message }); }
        }

        [HttpPut("{eventoId:long}/descripcion")]
        public async Task<IActionResult> CambiarDescripcion(long eventoId, [FromBody] string nuevaDescripcion)
        {
            try
            {
                await _eventoAdministradorService.CambiarDescripcion(eventoId, nuevaDescripcion);
                return NoContent();
            }
            catch (NotFoundException ex) { return NotFound(new ErrorDto { Error = ex.Message }); }
            catch (Exception ex) { return BadRequest(new ErrorDto { Error = ex.Message }); }
        }

        [HttpPut("{eventoId:long}/grupo/{grupoId}/participantes")]
        public async Task<IActionResult> ActualizarParticipantes(
            long eventoId,
            long grupoId,
            [FromBody] List<string> correos)
        {
            try
            {
                var dto = new GrupoDto
                {
                    Id = grupoId,
                    CorreosParticipantes = correos
                };

                await _eventoAdministradorService.ActualizarParticipantesGrupo(eventoId, dto);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("grupo/{grupoId}")]
        public async Task<IActionResult> ActualizarGrupo(long grupoId, [FromBody] GrupoDto dto)
        {
            try
            {
                await _eventoAdministradorService.ActualizarGrupo(grupoId, dto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorDto { Error = ex.Message });
            }
        }

        [HttpDelete("grupo/{grupoId}")]
        public async Task<IActionResult> EliminarGrupo(long grupoId)
        {
            try
            {
                await _eventoAdministradorService.EliminarGrupo(grupoId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorDto { Error = ex.Message });
            }
        
        }

        [HttpPost("categoria/{idCategoria}/grupo")]
        public async Task<IActionResult> CrearGrupo(long idCategoria, [FromBody] GrupoDto dto)
        {
            try
            {
                await _eventoAdministradorService.AñadirGrupo(dto.Nombre, idCategoria);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorDto { Error = ex.Message });
            }
        }
        [HttpPost("aceptar-invitacion/{token}")]
        public async Task<IActionResult> AceptarInvitacion(string token)
        {
            try
            {
                var correo = User.Claims.FirstOrDefault(c =>
                    c.Type == ClaimTypes.Email ||
                    c.Type == "email" ||
                    c.Type == "sub"
                )?.Value;

                await _eventoAdministradorService.AceptarInvitacion(token, correo);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorDto { Error = ex.Message });
            }
        }

        [HttpGet("grupo/{id}/pendientes")]
        public async Task<IActionResult> Pendientes(long id)
        {
            var cantidad = await _eventoAdministradorService.ContarInvitacionesPendientes(id);
            Console.WriteLine($"Grupo {id} pendientes = {cantidad}");
            return Ok(cantidad);

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
                    dto.FechaFinVotaciones,
                    dto.FechaFinExperta);
                return NoContent();
            }
            catch (ValidationException ex) { return UnprocessableEntity(new ErrorDto { Error = ex.Message }); }
            catch (NotFoundException ex) { return NotFound(new ErrorDto { Error = ex.Message }); }
            catch (Exception ex) { return BadRequest(new ErrorDto { Error = ex.Message }); }
        }

        [HttpPut("{eventoId:long}/pausar")]
        public async Task<IActionResult> PausarEvento(long eventoId)
        {
            await _eventoAdministradorService.PausarEvento(eventoId);
            return NoContent();
        }

        [HttpPut("{eventoId:long}/reanudar")]
        public async Task<IActionResult> ReanudarEvento(long eventoId)
        {
            await _eventoAdministradorService.ReanudarEvento(eventoId);
            return NoContent();
        }

        [HttpPut("{eventoId:long}/cerrar")]
        public async Task<IActionResult> CerrarEvento(long eventoId)
        {
            await _eventoAdministradorService.CerrarEvento(eventoId);
            return NoContent();
        }


    }
}