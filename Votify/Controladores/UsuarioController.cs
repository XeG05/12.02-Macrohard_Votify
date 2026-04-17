using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Votify.Auth;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Interfaces;

namespace Votify.Controladores
{
    [ApiController]
    [Route("api/usuario")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly JwtService _jwtService;
        public UsuarioController(IUsuarioService usuarioService, JwtService jwtService)
        {
            _usuarioService = usuarioService;
            _jwtService = jwtService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var usuario = await _usuarioService.Login(loginDto.Correo, loginDto.Contraseña);
            if (usuario == null)
            {
                return Unauthorized();
            }
            var token = _jwtService.GenerateToken(usuario);
            return Ok(new AuthDto { Token = token, Correo = usuario.Correo, Nombre = usuario.Nombre});

        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistroDto registroDto)
        {
            var usuario = await _usuarioService.Register(registroDto.Nombre, registroDto.Correo, registroDto.Contraseña, registroDto.ConfirmarContraseña);
            if (usuario == null) return BadRequest(new { error = "Error al registrar el usuario" });
            var token = _jwtService.GenerateToken(usuario);
            return Created(string.Empty, new AuthDto{
                Token = token, Correo = usuario.Correo, Nombre = usuario.Nombre });
        }
    }
}
