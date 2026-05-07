using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Interfaces;
using Votify.Controladores;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Votify.Tests
{
    [TestClass]
    public class EventoParticipanteControllerTests
    {
        private Mock<IEventoParticipanteService> _participanteServiceMock;
        private EventoParticipanteController _controller;

        [TestInitialize]
        public void Setup()
        {
            _participanteServiceMock = new Mock<IEventoParticipanteService>();
            _controller = new EventoParticipanteController(_participanteServiceMock.Object);

            // Simular un contexto de usuario autenticado
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "test@correo.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
        }

        [TestMethod]
        public async Task CrearProyecto_ConParametrosDeReq_RetornaCreated()
        {
            // Arrange
            long eventoId = 1;
            long? grupoId = 123;
            var nombreProyecto = "Mi Proyecto";
            var descripcionBreve = "Breve desc";
            var descripcionCompleta = "Completa desc";

            // Para simular el comportamiento exitoso
            _participanteServiceMock.Setup(s => s.CrearProyecto(It.IsAny<CrearProyectoRequest>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CrearProyecto(
                eventoId,
                grupoId,
                nombreProyecto,
                descripcionBreve,
                descripcionCompleta,
                null,
                null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedResult));
            var createdResult = (CreatedResult)result;
            Assert.AreEqual(string.Empty, createdResult.Location);

            // Verificar que el servicio fue llamado con los párametros correctos encapsulados en CrearProyectoRequest
            _participanteServiceMock.Verify(s => s.CrearProyecto(It.Is<CrearProyectoRequest>(req =>
                req.Correo == "test@correo.com" &&
                req.IdEvento == eventoId &&
                req.GrupoIdActivo == grupoId &&
                req.NombreProyecto == nombreProyecto &&
                req.DescripcionBreve == descripcionBreve &&
                req.DescripcionCompleta == descripcionCompleta &&
                req.Archivo.Length == 0 &&
                req.Imagenes.Count == 0
            )), Times.Once);
        }
    }
}
