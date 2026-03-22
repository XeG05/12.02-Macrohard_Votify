using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Votify.Modelos;
using Votify.Modelos.BusinessLogic.Services;
using Votify.Modelos.Persistence.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Votify.Tests
{
    [TestClass]
    public class RegisterTests : UsuarioServiceTestsBase
    {
        [TestMethod]
        public async Task Register_ConDatosValidos_CreaUsuarioYLoGuardaEnBD()
        {
            var usuario = await UsuarioService.Register(
                "Pablo",
                "pablo@gmail.com",
                "abc123",
                "abc123");

            Assert.IsNotNull(usuario);
            Assert.AreEqual("Pablo", usuario.Nombre);
            Assert.AreEqual("pablo@gmail.com", usuario.Correo);
            Assert.AreEqual("abc123", usuario.Contraseña);

            var usuarioGuardado = await Context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == "pablo@gmail.com");

            Assert.IsNotNull(usuarioGuardado);
            Assert.AreEqual("Pablo", usuarioGuardado.Nombre);
        }

        [TestMethod]
        public async Task Register_ConContraseñasDistintas_LanzaExcepcion()
        {
            var ex = await Assert.ThrowsAsync<Exception>(async () =>
                await UsuarioService.Register(
                    "Pablo",
                    "pablo@gmail.com",
                    "abc123",
                    "xyz999"));

            Assert.AreEqual("Las contraseñas no coinciden", ex.Message);
        }

        [TestMethod]
        public async Task Register_ConCorreoDuplicado_LanzaExcepcion()
        {
            await CrearUsuarioAsync(
                nombre: "Pablo",
                correo: "pablo@gmail.com",
                contraseña: "12345");

            var ex = await Assert.ThrowsAsync<Exception>(async () =>
                await UsuarioService.Register(
                    "Otro Pablo",
                    "pablo@gmail.com",
                    "99999",
                    "99999"));

            Assert.AreEqual("El correo ya está registrado", ex.Message);
        }

        [TestMethod]
        public async Task Register_YLuegoLogin_PermiteAccederCorrectamente()
        {
            await UsuarioService.Register(
                "Pablo",
                "pablo@gmail.com",
                "clave123",
                "clave123");

            var usuario = await UsuarioService.Login("pablo@gmail.com", "clave123");

            Assert.IsNotNull(usuario);
            Assert.AreEqual("Pablo", usuario.Nombre);
            Assert.AreEqual("pablo@gmail.com", usuario.Correo);
        }
    }
}