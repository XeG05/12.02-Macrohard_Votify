using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Services;
using Votify.Persistencia;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;
using Votify.Persistencia.Repositories;
using Votify.LógicaNegocio.Interfaces;

namespace Votify.Tests
{
    [TestClass]
    public class CalculoPuntosTests
    {
        private VotifyContext _context;
        private EventoAdministradorService _adminService;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<VotifyContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new VotifyContext(options);

            var uowMock = new Mock<IUnitOfWork>();
            var emailMock = new Mock<IEmailService>();
            var configMock = new Mock<IConfiguration>();

            _adminService = new EventoAdministradorService(
                new Repository<Evento>(_context),
                new Repository<Categorium>(_context),
                new Repository<Grupo>(_context),
                new Repository<Proyecto>(_context),
                new Repository<Voto>(_context),
                new Repository<Criterio>(_context),
                new Repository<Participante>(_context),
                new Repository<InvitacionPendiente>(_context),
                new Repository<Rol>(_context),
                new Repository<Administrador>(_context),
                new Repository<Usuario>(_context),
                new Repository<Jurado>(_context),
                new Repository<Votante>(_context),
                uowMock.Object,
                emailMock.Object,
                configMock.Object,
                _context
            );
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.DisposeAsync();
        }

        [TestMethod]
        public async Task ObtenerResultadosCategoria_CalculaMediaCorrectamente_ConTipoVotacionPuntos()
        {
            // Arrange
            var evento = new Evento
            {
                Id = 1,
                Nombre = "Evento Test Puntos",
                Descripcion = "D",
                PuntosARepartir = 100,
                TipoVotacion = TipoVotacion.Puntos
            };

            var categoria = new Categorium
            {
                Id = 1,
                IdEvento = 1,
                Nombre = "Categoría Principal",
                Descripcion = "D",
                CriteriosSonPorcentaje = true,
                PonderacionExperto = 50,
                PonderacionPopular = 50
            };

            // Criterio 1 (50%)
            var criterio1 = new Criterio
            {
                Id = 1,
                IdCategoria = 1,
                Nombre = "Criterio 1",
                Descripcion = "D",
                Ponderacion = 50
            };

            // Criterio 2 (50%)
            var criterio2 = new Criterio
            {
                Id = 2,
                IdCategoria = 1,
                Nombre = "Criterio 2",
                Descripcion = "D",
                Ponderacion = 50
            };

            var grupo = new Grupo
            {
                Id = 1,
                IdCategoria = 1,
                Nombre = "Grupo A"
            };

            var usuarioNav = new Usuario { Correo = "voto@test.com", Nombre = "Votante", Contraseña = "123" };

            var rolVotante = new Votante { CorreoUsuario = "voto@test.com", IdEvento = 1 };

            var proyecto = new Proyecto
            {
                Id = 1,
                IdGrupo = 1,
                Nombre = "Proyecto Innovador",
                Descripcion = "D",
                DescripcionBreve = "DB",
                Archivo = new byte[] { 0 }
            };

            // Creamos los votos para el proyecto (70 en Criterio 1 y 30 en Criterio 2)
            var votoC1 = new Voto { Id = 1, IdProyectoGru = 1, IdCriterio = 1, Nota = 70, IdRol = 1, CorreoRol = "voto@test.com" };
            var votoC2 = new Voto { Id = 2, IdProyectoGru = 1, IdCriterio = 2, Nota = 30, IdRol = 1, CorreoRol = "voto@test.com" };

            // Añadir al contexto de InMemory
            await _context.Eventos.AddAsync(evento);
            await _context.Categoria.AddAsync(categoria);
            await _context.Criterios.AddRangeAsync(criterio1, criterio2);
            await _context.Grupos.AddAsync(grupo);
            await _context.Proyectos.AddAsync(proyecto);
            await _context.Usuarios.AddAsync(usuarioNav);
            await _context.Votantes.AddAsync(rolVotante); // necesario para que el servicio lo clasifique como votante popular
            await _context.Votos.AddRangeAsync(votoC1, votoC2);
            await _context.SaveChangesAsync();

            // Act
            var resultados = await _adminService.ObtenerResultadosCategoria(categoria.Id);

            // Assert
            Assert.IsNotNull(resultados);
            Assert.AreEqual(1, resultados.Count);

            var resultadoProyecto = resultados.First();

            // Verificamos los resultados devueltos
            Assert.AreEqual("Proyecto Innovador", resultadoProyecto.NombreProyecto);

            // Calculo esperado (el servicio aplica pesoPopular y pesoExperto por separado):
            // C1: ponderacion(0.5) × (puntosVotantes(70) × pesoPopular(0.5) + puntosJurados(0) × pesoExperto(0.5)) = 0.5 × 35 = 17.5
            // C2: ponderacion(0.5) × (puntosVotantes(30) × pesoPopular(0.5) + puntosJurados(0) × pesoExperto(0.5)) = 0.5 × 15 = 7.5
            // Total Puntuacion: 17.5 + 7.5 = 25.0
            Assert.AreEqual(25.0, resultadoProyecto.PuntuacionMedia, delta: 0.01, "La puntuación media de los puntos por criterio debería ser 25.");

            Assert.AreEqual(2, resultadoProyecto.ResultadosPorCriterio.Count);

            var resC1 = resultadoProyecto.ResultadosPorCriterio.FirstOrDefault(c => c.NombreCriterio == "Criterio 1");
            Assert.IsNotNull(resC1);
            Assert.AreEqual(70, resC1.TotalPuntos);

            var resC2 = resultadoProyecto.ResultadosPorCriterio.FirstOrDefault(c => c.NombreCriterio == "Criterio 2");
            Assert.IsNotNull(resC2);
            Assert.AreEqual(30, resC2.TotalPuntos);
        }
    }
}