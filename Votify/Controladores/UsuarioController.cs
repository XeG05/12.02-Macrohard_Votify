using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Votify.Auth;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Exceptions;
using Votify.LógicaNegocio.Interfaces;
using System.Security.Claims;

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
            try
            {
                var usuario = await _usuarioService.Login(loginDto.Correo, loginDto.Contraseña);
                if (usuario == null)
                {
                    return Unauthorized();
                }
                var token = _jwtService.GenerateToken(usuario);
                return Ok(new AuthDto { Token = token, Correo = usuario.Correo, Nombre = usuario.Nombre });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ErrorDto { Error = ex.Message });
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new ErrorDto { Error = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistroDto registroDto)
        {
            try
            {
                var usuario = await _usuarioService.Register(registroDto.Nombre, registroDto.Correo, registroDto.Contraseña, registroDto.ConfirmarContraseña);
                if (usuario == null) return BadRequest(new { error = "Error al registrar el usuario" });
                var token = _jwtService.GenerateToken(usuario);
                return Created(string.Empty, new AuthDto
                {
                    Token = token,
                    Correo = usuario.Correo,
                    Nombre = usuario.Nombre
                });
            }
            catch (ValidationException ex)
            {
                return UnprocessableEntity(new ErrorDto { Error = ex.Message });
            }
            catch (ConflictException ex)
            {
                return Conflict(new ErrorDto { Error = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("perfil")]
        public async Task<IActionResult> ObtenerPerfil()
        {
            try
            {
                var correo = User.FindFirstValue(ClaimTypes.Name);

                if (string.IsNullOrWhiteSpace(correo))
                {
                    return Unauthorized(new ErrorDto { Error = "Usuario no autenticado" });
                }

                var usuario = await _usuarioService.ObtenerPorCorreo(correo);

                return Ok(new PerfilUsuarioDto
                {
                    Nombre = usuario!.Nombre,
                    Correo = usuario.Correo,
                    ImagenBase64 = usuario.Imagen != null ? Convert.ToBase64String(usuario.Imagen) : null
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ErrorDto { Error = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("perfil")]
        public async Task<IActionResult> ActualizarPerfil([FromForm] ActualizarPerfilUsuarioDto dto)
        {
            try
            {
                var correo = User.FindFirstValue(ClaimTypes.Name);

                if (string.IsNullOrWhiteSpace(correo))
                {
                    return Unauthorized(new ErrorDto { Error = "Usuario no autenticado" });
                }

                var usuario = await _usuarioService.ActualizarPerfil(
                    correo,
                    dto.Nombre,
                    dto.NuevaContraseña,
                    dto.RepetirContraseña,
                    dto.Imagen
                );

                var token = _jwtService.GenerateToken(usuario!);

                return Ok(new AuthDto
                {
                    Token = token,
                    Correo = usuario!.Correo,
                    Nombre = usuario.Nombre,
                    ImagenBase64 = usuario.Imagen != null ? Convert.ToBase64String(usuario.Imagen) : null
                });
            }
            catch (ValidationException ex)
            {
                return UnprocessableEntity(new ErrorDto { Error = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ErrorDto { Error = ex.Message });
            }
        }
    }
}
