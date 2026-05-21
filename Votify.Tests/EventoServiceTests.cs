using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Votify.LógicaNegocio.Builder;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Factory.Roles;
using Votify.LógicaNegocio.Interfaces;
using Votify.LógicaNegocio.Services;
using Votify.Persistencia;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;
using Votify.Persistencia.Repositories;

namespace Votify.Tests
{
    [TestClass]
    public class EventoServiceTests
    {
        private VotifyContext _context;
        private EventoService _eventoService;
        private Mock<IEmailService> _emailMock;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<VotifyContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new VotifyContext(options);

            var uowMock = new Mock<IUnitOfWork>();
            _emailMock = new Mock<IEmailService>();
            var configMock = new Mock<IConfiguration>();

            var adminRepo = new Repository<Administrador>(_context);
            var votanteRepo = new Repository<Votante>(_context);
            var rolRepo = new Repository<Rol>(_context);

            var adminCreator = new AdministradorCreator(rolRepo);
            var votanteCreator = new VotanteCreator(rolRepo);

            _eventoService = new EventoService(
                adminCreator,
                votanteCreator,
                new EventoDirector(),
                new EventoConcretoBuilder(),
                new Repository<Evento>(_context),
                new Repository<Usuario>(_context),
                new Repository<Proyecto>(_context),
                new Repository<Participante>(_context),
                new Repository<Grupo>(_context),
                new Repository<Categorium>(_context),
                adminRepo,
                new Repository<Jurado>(_context),
                votanteRepo,
                rolRepo,
                new Repository<Criterio>(_context),
                new Repository<Voto>(_context),
                new Repository<Comentario>(_context),
                new Repository<InvitacionPendiente>(_context),
                new Repository<InvitacionJurado>(_context),
                new Repository<Votify.Persistencia.Entities.SintesisJuradoCriterio>(_context),
                _emailMock.Object,
                configMock.Object,
                uowMock.Object
            );
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.DisposeAsync();
        }

        [TestMethod]
        public async Task CrearEvento_ConGrupo_GeneraFechaExpiracionA7DiasVotify()
        {
            // Arrange
            var creador = new Usuario { Nombre = "Admin", Correo = "admin@test.com", Contraseña = "123" };
            _context.Usuarios.Add(creador);
            await _context.SaveChangesAsync();

            var dto = new EventoCreacionDto
            {
                Nombre = "Evento Test Expiracion",
                Descripcion = "Evento de prueba",
                FechaFinInscripciones = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
                FechaFinEntrega = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(12)),
                FechaFinVotaciones = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14)),
                FechaFinExperta = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(16)),
                FechaFinVotExperta = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(18)),
                TieneGrupos = true,
                Categorias = new System.Collections.Generic.List<CategoriaDto>
                {
                    new CategoriaDto
                    {
                        Nombre = "Categoría test",
                        PonderacionExperto = 50,
                        PonderacionPopular = 50,
                        Grupos = new System.Collections.Generic.List<GrupoDto>
                        {
                            new GrupoDto
                            {
                                Nombre = "Grupo Test",
                                CorreosParticipantes = new System.Collections.Generic.List<string> { "participante@test.com" }
                            }
                        }
                    }
                }
            };

            // Act
            var evento = await _eventoService.CrearEvento(creador.Correo, dto);

            // Assert
            var invitacion = await _context.InvitacionesPendientes.FirstOrDefaultAsync(i => i.CorreoDestino == "participante@test.com");

            Assert.IsNotNull(invitacion, "La invitación no fue generada en la base de datos.");

            // Validar de forma certera que la fecha de expiracion está calculada para dentro de 7 dias (descontamos un par de segundos por el tiempo de ejecución)
            var diasDiferencia = (invitacion.FechaExpiracion - DateTime.UtcNow).TotalDays;

            Assert.IsTrue(diasDiferencia > 6.9 && diasDiferencia <= 7.0, $"La fecha de expiración no se computó a 7 días. Se obtuvo {diasDiferencia} días de diferencia.");
        }
    }
}