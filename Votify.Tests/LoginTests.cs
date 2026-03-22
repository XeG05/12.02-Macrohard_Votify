using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Votify.Modelos;
using Votify.Modelos.BusinessLogic.Services;
using Votify.Modelos.Persistence.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Votify.Tests
{
    [TestClass]
    public class LoginTests : UsuarioServiceTestsBase
    {
        [TestMethod]
        public async Task Login_ConCredencialesCorrectas_DevuelveUsuario()
        {
            await CrearUsuarioAsync(
                nombre: "Pablo",
                correo: "pablo@gmail.com",
                contraseña: "12345");

            var usuario = await UsuarioService.Login("pablo@gmail.com", "12345");

            Assert.IsNotNull(usuario);
            Assert.AreEqual("pablo", usuario.Nombre);
            Assert.AreEqual("pablo@gmail.com", usuario.Correo);
            Assert.AreEqual("12345", usuario.Contraseña);
        }

        [TestMethod]
        public async Task Login_ConCorreoNoExistente_LanzaExcepcion()
        {
            var ex = await Assert.ThrowsAsync<Exception>(async () =>
                await UsuarioService.Login("noexiste@gmail.com", "12345"));

            Assert.AreEqual("Ese usuario no existe", ex.Message);
        }

        [TestMethod]
        public async Task Login_ConContraseñaIncorrecta_LanzaExcepcion()
        {
            await CrearUsuarioAsync(
                nombre: "Pablo",
                correo: "pablo@gmail.com",
                contraseña: "12345");

            var ex = await Assert.ThrowsAsync<Exception>(async () =>
                await UsuarioService.Login("pablo@gmail.com", "xxxxx"));

            Assert.AreEqual("Contraseña incorrecta", ex.Message);
        }
    }
}