using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Votify.LógicaNegocio.Interfaces;
using Votify.Persistencia.Entities;

namespace Votify.Controladores
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet("login")]
        public async Task<Usuario?> Login(string correo, string contraseña)
        {
            return await _usuarioService.Login(correo, contraseña);
        }

        [HttpGet("register")]
        public async Task<Usuario?> Register(string nombre, string correo, string contraseña, string contraseñaRepetida)
        {
            return await _usuarioService.Register(nombre, correo, contraseña, contraseñaRepetida);
        }
    }
}
