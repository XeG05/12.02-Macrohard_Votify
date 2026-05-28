using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Votify.Controladores;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Exceptions;
using Votify.LógicaNegocio.Interfaces;
using Votify.LógicaNegocio.Services;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.Tests
{
    [TestClass]
    public class ActualizarFechasRefactorTests
    {
        [TestMethod]
        public async Task Controller_ActualizarFechas_RecibeParameterObjectYLlamaAlServicio()
        {
            const long eventoId = 10;
            var dto = CrearDtoValido();

            var administradorServiceMock = new Mock<IEventoAdministradorService>();
            administradorServiceMock
                .Setup(s => s.ActualizarFechas(eventoId, dto))
                .Returns(Task.CompletedTask);

            var controller = new EventoAdministradorController(
                administradorServiceMock.Object,
                Mock.Of<IEventoService>());

            var result = await controller.ActualizarFechas(eventoId, dto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            administradorServiceMock.Verify(
                s => s.ActualizarFechas(
                    eventoId,
                    It.Is<ActualizarFechasDto>(d => ReferenceEquals(d, dto))),
                Times.Once);
        }

        [TestMethod]
        public async Task Controller_ActualizarFechas_SiServicioValidaMal_DevuelveUnprocessableEntity()
        {
            const long eventoId = 10;
            var dto = CrearDtoValido();

            var administradorServiceMock = new Mock<IEventoAdministradorService>();
            administradorServiceMock
                .Setup(s => s.ActualizarFechas(eventoId, dto))
                .ThrowsAsync(new ValidationException("Las fechas deben estar en orden lógico"));

            var controller = new EventoAdministradorController(
                administradorServiceMock.Object,
                Mock.Of<IEventoService>());

            var result = await controller.ActualizarFechas(eventoId, dto);

            Assert.IsInstanceOfType(result, typeof(UnprocessableEntityObjectResult));
            var objectResult = (UnprocessableEntityObjectResult)result;
            var error = objectResult.Value as ErrorDto;

            Assert.IsNotNull(error);
            Assert.AreEqual("Las fechas deben estar en orden lógico", error.Error);
        }

        [TestMethod]
        public async Task Service_ActualizarFechas_ConDtoValido_ActualizaFechasYGuarda()
        {
            var evento = CrearEvento(FasesEvento.Inscripciones);
            var dto = CrearDtoValido();

            var eventoRepoMock = new Mock<IRepository<Evento>>();
            eventoRepoMock
                .Setup(r => r.GetByIdAsync(evento.Id))
                .ReturnsAsync(evento);
            eventoRepoMock
                .Setup(r => r.UpdateAsync(evento))
                .Returns(Task.CompletedTask);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var service = CrearService(eventoRepoMock, unitOfWorkMock);

            await service.ActualizarFechas(evento.Id, dto);

            Assert.AreEqual(DateOnly.FromDateTime(dto.FechaFinInscripciones), evento.FechaFinInscripciones);
            Assert.AreEqual(DateOnly.FromDateTime(dto.FechaFinEntrega), evento.FechaFinEntrega);
            Assert.AreEqual(DateOnly.FromDateTime(dto.FechaFinVotaciones), evento.FechaFinVotaciones);
            Assert.AreEqual(DateOnly.FromDateTime(dto.FechaFinExperta), evento.FechaFinEvaluacionExperta);
            Assert.AreEqual(DateOnly.FromDateTime(dto.FechaFinVotExperta), evento.FechaFinVotacionExperta);

            eventoRepoMock.Verify(r => r.UpdateAsync(evento), Times.Once);
            unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task Service_ActualizarFechas_ConFechasDesordenadas_LanzaValidationExceptionYNoGuarda()
        {
            var evento = CrearEvento(FasesEvento.Inscripciones);
            var dto = CrearDtoValido();
            dto.FechaFinEntrega = dto.FechaFinInscripciones.AddDays(-1);

            var eventoRepoMock = new Mock<IRepository<Evento>>();
            eventoRepoMock
                .Setup(r => r.GetByIdAsync(evento.Id))
                .ReturnsAsync(evento);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var service = CrearService(eventoRepoMock, unitOfWorkMock);

            try
            {
                await service.ActualizarFechas(evento.Id, dto);
                Assert.Fail("Se esperaba una ValidationException.");
            }
            catch (ValidationException)
            {
                // Correcto: las fechas no son válidas.
            }

            eventoRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Evento>()), Times.Never);
            unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [TestMethod]
        public async Task Service_ActualizarFechas_SiEventoNoExiste_LanzaNotFoundException()
        {
            const long eventoId = 999;
            var dto = CrearDtoValido();

            var eventoRepoMock = new Mock<IRepository<Evento>>();
            eventoRepoMock
                .Setup(r => r.GetByIdAsync(eventoId))
                .ReturnsAsync((Evento?)null);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var service = CrearService(eventoRepoMock, unitOfWorkMock);

            try
            {
                await service.ActualizarFechas(eventoId, dto);
                Assert.Fail("Se esperaba una NotFoundException.");
            }
            catch (NotFoundException)
            {
                // Correcto: el evento no existe.
            }

            eventoRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Evento>()), Times.Never);
            unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        private static EventoAdministradorService CrearService(
            Mock<IRepository<Evento>> eventoRepoMock,
            Mock<IUnitOfWork> unitOfWorkMock)
        {
            return new EventoAdministradorService(
                eventoRepoMock.Object,
                Mock.Of<IRepository<Categorium>>(),
                Mock.Of<IRepository<Grupo>>(),
                Mock.Of<IRepository<Proyecto>>(),
                Mock.Of<IRepository<Voto>>(),
                Mock.Of<IRepository<Criterio>>(),
                Mock.Of<IRepository<Participante>>(),
                Mock.Of<IRepository<InvitacionPendiente>>(),
                Mock.Of<IRepository<Rol>>(),
                Mock.Of<IRepository<Administrador>>(),
                Mock.Of<IRepository<Usuario>>(),
                Mock.Of<IRepository<Jurado>>(),
                Mock.Of<IRepository<Votante>>(),
                unitOfWorkMock.Object,
                Mock.Of<IEmailService>(),
                Mock.Of<IConfiguration>(),
                null!);
        }

        private static Evento CrearEvento(FasesEvento fase)
        {
            var baseDate = new DateOnly(2026, 1, 1);

            return new Evento
            {
                Id = 10,
                Nombre = "Evento de prueba",
                Descripcion = "Evento usado para probar actualización de fechas",
                FechaFinInscripciones = baseDate.AddDays(10),
                FechaFinEntrega = baseDate.AddDays(20),
                FechaFinVotaciones = baseDate.AddDays(30),
                FechaFinEvaluacionExperta = baseDate.AddDays(40),
                FechaFinVotacionExperta = baseDate.AddDays(50),
                FaseActual = fase,
                Estado = "Activo"
            };
        }

        private static ActualizarFechasDto CrearDtoValido()
        {
            return new ActualizarFechasDto
            {
                FechaFinInscripciones = new DateTime(2026, 2, 1),
                FechaFinEntrega = new DateTime(2026, 2, 2),
                FechaFinVotaciones = new DateTime(2026, 2, 3),
                FechaFinExperta = new DateTime(2026, 2, 4),
                FechaFinVotExperta = new DateTime(2026, 2, 5)
            };
        }
    }
}