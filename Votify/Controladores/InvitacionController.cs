using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Interfaces;

namespace Votify.Controladores
{
    [ApiController]
    [Route("api/invitacion")]
    [Authorize]
    public class InvitacionController : ControllerBase
    {
        private readonly IInvitacionService _invitacionService;

        public InvitacionController(IInvitacionService invitacionService)
        {
            _invitacionService = invitacionService;
        }

        [HttpGet("validar/{token}")]
        public async Task<IActionResult> ValidarToken(string token)
        {
            var esValido = await _invitacionService.ObtenerInvitacionValida(token);
            if (esValido != null) { return Ok(new { mensaje = "Token válido.", invitacion = esValido }); }
            return NotFound(new { mensaje = "Token inválido o expirado." });
        }

        [HttpPost("aceptar")]
        public async Task<IActionResult> AceptarInvitacion([FromBody] AceptarInvitacionDto request)
        {
            return Ok(await _invitacionService.AceptarInvitacionAsync(request.Token, request.Contraseña, request.Nombre, request.ConfirmarContraseña));
        }
    }
}
