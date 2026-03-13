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
    [Route("api/usuarios")]
    public class UsuarioController : GenericController<Usuario>
    {
        private readonly I_Usuario_Service usuario_Service;
        private Usuario_service? usuarioService;

        public UsuarioController(I_Usuario_Service usuario_Service) : base(usuario_Service)
        {

            this.usuario_Service = usuario_Service;
        }

        [HttpPost("login")]
        public async Task<ActionResult<Usuario>> Login(string correo, string contraseña)
        {
            var usuario = await usuarioService.login(correo, contraseña);

            if (usuario == null) { return Unauthorized("Correo o contraseña incorrectos"); }

            return Ok(usuario);
        }

    }
}
