using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Votify.Modelos;
using Votify.Modelos.BusinessLogic.Services;
using Votify.Modelos.BusinessLogic.Interfaces;

namespace Votify.Components.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuarioController : GenericController<Usuario>
    {
        private readonly I_Usuario_Service usuario_Service;
        private Usuario_service? usuarioService;

        public UsuarioController(I_Usuario_Service usuario_Service) : base(usuario_Service)
        {

            this.usuario_Service = usuario_Service;
        }
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("UsuarioController funciona");
        }

        [HttpPost("login")]
        public async Task<ActionResult<Usuario>> Login(string correo, string contraseña)
        {
            try
            {
                var usuario = await usuarioService.login(correo, contraseña);

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<Usuario>> register(string nombre, string correo, string contraseña, string contraseña2)
        {
            try
            {
                var usuario = await usuarioService.register(nombre, correo, contraseña, contraseña2);

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

    }
}
