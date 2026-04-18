using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Votify.Auth;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Interfaces;

namespace Votify.Controladores
{
    [ApiController]
    [Route("api/invitacion")]
    [AllowAnonymous]
    public class InvitacionController : ControllerBase
    {
        private readonly IInvitacionService _invitacionService;
        private readonly JwtService _jwtService;

        public InvitacionController(IInvitacionService invitacionService, JwtService jwtService)
        {
            _invitacionService = invitacionService;
            _jwtService = jwtService;
        }

        [HttpGet("validar/{token}")]
        public async Task<IActionResult> ValidarToken(string token)
        {
            var dto = await _invitacionService.ObtenerInvitacionInfoAsync(token);
            if (dto is not null) return Ok(dto);
            return NotFound(new { mensaje = "Token inválido o expirado." });
        }

        [HttpPost("aceptar")]
        public async Task<IActionResult> AceptarInvitacion([FromBody] AceptarInvitacionDto request)
        {
            try
            {
                var jwtToken = await _invitacionService.AceptarInvitacionAsync(
                    request.Token, request.Contraseña, request.Nombre, request.ConfirmarContraseña);

                // Parseamos el JWT para extraer nombre y correo sin una segunda consulta a BD
                var principal = _jwtService.ValidateToken(jwtToken);
                var correo = principal?.FindFirst(ClaimTypes.Name)?.Value ?? "";
                var nombre = principal?.FindFirst("nombre")?.Value ?? "";

                return Ok(new AuthDto
                {
                    Token  = jwtToken,
                    Correo = correo,
                    Nombre = nombre
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
