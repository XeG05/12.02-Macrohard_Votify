using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Votify.LógicaNegocio.Builder;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Exceptions;
using Votify.LógicaNegocio.Factory.Roles;
using Votify.LógicaNegocio.Factory.Votacion;
using Votify.LógicaNegocio.Interfaces;
using Votify.LógicaNegocio.Services;
using Votify.Persistencia;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;
using Votify.Persistencia.Repositories;

namespace Votify.Tests
{
    [TestClass]
    public class CrearEventoTests
    {
        private VotifyContext _context = null!;
        private EventoService _servicio = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<VotifyContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new VotifyContext(options);
            _servicio = CrearServicio(_context);
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.DisposeAsync();
        }

        private static EventoService CrearServicio(VotifyContext ctx)
        {
            var rolRepo = new Repository<Rol>(ctx);

            var uowMock = new Mock<IUnitOfWork>();
            uowMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            var emailMock = new Mock<IEmailService>();
            emailMock
                .Setup(e => e.EnviarInvitacionAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            emailMock
                .Setup(e => e.EnviarInvitacionJuradoAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var configMock = new Mock<IConfiguration>();
            var serviceProviderMock = new Mock<IServiceProvider>();

            return new EventoService(
                new AdministradorCreator(rolRepo),
                new VotanteCreator(rolRepo),
                new EventoDirector(),
                new EventoConcretoBuilder(),
                new Repository<Evento>(ctx),
                new Repository<Usuario>(ctx),
                new Repository<Proyecto>(ctx),
                new Repository<Participante>(ctx),
                new Repository<Grupo>(ctx),
                new Repository<Categorium>(ctx),
                new Repository<Administrador>(ctx),
                new Repository<Jurado>(ctx),
                new Repository<Votante>(ctx),
                rolRepo,
                new Repository<Criterio>(ctx),
                new Repository<Voto>(ctx),
                new Repository<Comentario>(ctx),
                new Repository<InvitacionPendiente>(ctx),
                new Repository<InvitacionJurado>(ctx),
                new Repository<Votify.Persistencia.Entities.SintesisJuradoCriterio>(ctx),
                emailMock.Object,
                configMock.Object,
                uowMock.Object,
                new VotacionStrategyFactory(new Mock<IServiceProvider>().Object)
            );
        }

        private static EventoCreacionDto DtoValido()
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            return new EventoCreacionDto
            {
                Nombre = "Concurso 2026",
                Descripcion = "Evento de prueba para tests",
                FechaFinInscripciones = hoy.AddDays(5),
                FechaFinEntrega = hoy.AddDays(10),
                FechaFinVotaciones = hoy.AddDays(15),
                FechaFinExperta = hoy.AddDays(20),
                FechaFinVotExperta = hoy.AddDays(25),
                TipoVotacion = TipoVotacion.Simple,
                NumVotarProyectos = 3,
                PuntosARepartir = 100
            };
        }

        private async Task<Usuario> AgregarUsuario(string correo = "creador@test.com")
        {
            var usuario = new Usuario("Creador Test", "hash123", correo);
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        // ─── Validación del DTO ───────────────────────────────────────────────────────

        [TestMethod]
        public async Task CrearEvento_NombreVacio_LanzaValidationException()
        {
            var dto = DtoValido();
            dto.Nombre = "";

            ValidationException? ex = null;
            try { await _servicio.CrearEvento("creador@test.com", dto); }
            catch (ValidationException e) { ex = e; }

            Assert.IsNotNull(ex, "Se esperaba ValidationException");
        }

        [TestMethod]
        public async Task CrearEvento_FechasEnOrdenIncorrecto_LanzaValidationException()
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var dto = DtoValido();
            dto.FechaFinInscripciones = hoy.AddDays(10);
            dto.FechaFinEntrega = hoy.AddDays(5); // anterior a inscripciones

            ValidationException? ex = null;
            try { await _servicio.CrearEvento("creador@test.com", dto); }
            catch (ValidationException e) { ex = e; }

            Assert.IsNotNull(ex, "Se esperaba ValidationException");
        }

        [TestMethod]
        public async Task CrearEvento_DtoInvalido_MensajeDeErrorEsPropagado()
        {
            var dto = DtoValido();
            dto.Nombre = new string('A', 31); // supera 30 caracteres

            ValidationException? ex = null;
            try { await _servicio.CrearEvento("creador@test.com", dto); }
            catch (ValidationException e) { ex = e; }

            Assert.IsNotNull(ex, "Se esperaba ValidationException");
            StringAssert.Contains(ex.Message, "30");
        }

        // ─── Usuario no encontrado ────────────────────────────────────────────────────

        [TestMethod]
        public async Task CrearEvento_UsuarioNoExiste_LanzaNotFoundException()
        {
            // No se agrega ningún usuario a la BD
            NotFoundException? ex = null;
            try { await _servicio.CrearEvento("nadie@test.com", DtoValido()); }
            catch (NotFoundException e) { ex = e; }

            Assert.IsNotNull(ex, "Se esperaba NotFoundException");
        }

        // ─── Correos duplicados entre grupos ─────────────────────────────────────────

        [TestMethod]
        public async Task CrearEvento_CorreosDuplicadosEnGrupos_LanzaConflictException()
        {
            await AgregarUsuario();

            var dto = DtoValido();
            dto.TieneGrupos = true;
            dto.NumParticipantes = 2;
            dto.Categorias.Add(new CategoriaDto
            {
                Nombre = "Categoría A",
                PonderacionPopular = 50,
                PonderacionExperto = 50,
                Grupos =
                [
                    new GrupoDto
                    {
                        Nombre = "Grupo 1",
                        CorreosParticipantes = ["duplicado@test.com", "unico@test.com"]
                    },
                    new GrupoDto
                    {
                        Nombre = "Grupo 2",
                        CorreosParticipantes = ["duplicado@test.com"] // correo repetido
                    }
                ]
            });

            ValidationException? ex = null;
            try { await _servicio.CrearEvento("creador@test.com", dto); }
            catch (ValidationException e) { ex = e; }

            Assert.IsNotNull(ex, "Se esperaba ConflictException");
        }

        [TestMethod]
        public async Task CrearEvento_SinGruposConCorreosRepetidos_NoLanzaConflictException()
        {
            await AgregarUsuario();

            // Sin grupos, la validación de duplicados no aplica
            var dto = DtoValido();
            dto.TieneGrupos = false;

            var resultado = await _servicio.CrearEvento("creador@test.com", dto);

            Assert.IsNotNull(resultado);
        }

        // ─── Creación exitosa del evento ──────────────────────────────────────────────

        [TestMethod]
        public async Task CrearEvento_DtoValido_RetornaEventoNoNulo()
        {
            await AgregarUsuario();

            var resultado = await _servicio.CrearEvento("creador@test.com", DtoValido());

            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public async Task CrearEvento_DtoValido_RetornaEventoConNombreCorrecto()
        {
            await AgregarUsuario();

            var resultado = await _servicio.CrearEvento("creador@test.com", DtoValido());

            Assert.AreEqual("Concurso 2026", resultado.Nombre);
        }

        [TestMethod]
        public async Task CrearEvento_DtoValido_RetornaEventoConDescripcionCorrecta()
        {
            await AgregarUsuario();

            var resultado = await _servicio.CrearEvento("creador@test.com", DtoValido());

            Assert.AreEqual("Evento de prueba para tests", resultado.Descripcion);
        }

        [TestMethod]
        public async Task CrearEvento_DtoValido_EventoQuedaEnFaseInscripciones()
        {
            await AgregarUsuario();

            var resultado = await _servicio.CrearEvento("creador@test.com", DtoValido());

            Assert.AreEqual(FasesEvento.Inscripciones, resultado.FaseActual);
        }

        [TestMethod]
        public async Task CrearEvento_DtoValido_EventoSeGuardaEnBD()
        {
            await AgregarUsuario();

            await _servicio.CrearEvento("creador@test.com", DtoValido());

            var eventosEnBD = await _context.Eventos.ToListAsync();
            Assert.AreEqual(1, eventosEnBD.Count);
        }

        // ─── Rol Administrador ────────────────────────────────────────────────────────

        [TestMethod]
        public async Task CrearEvento_DtoValido_CreaRolAdministradorParaElCreador()
        {
            await AgregarUsuario("creador@test.com");

            var evento = await _servicio.CrearEvento("creador@test.com", DtoValido());

            var admins = await _context.Administradors.ToListAsync();
            Assert.AreEqual(1, admins.Count);
            Assert.AreEqual("creador@test.com", admins[0].CorreoUsuario);
            Assert.AreEqual(evento.Id, admins[0].IdEvento);
        }

        [TestMethod]
        public async Task CrearEvento_DtoValido_SoloCreaUnAdministrador()
        {
            await AgregarUsuario();

            await _servicio.CrearEvento("creador@test.com", DtoValido());

            var admins = await _context.Administradors.ToListAsync();
            Assert.AreEqual(1, admins.Count);
        }

        // ─── Categorías y criterios ───────────────────────────────────────────────────

        [TestMethod]
        public async Task CrearEvento_ConCategoria_CreaCategoriaEnBD()
        {
            await AgregarUsuario();

            var dto = DtoValido();
            dto.Categorias.Add(new CategoriaDto
            {
                Nombre = "Diseño",
                Descripcion = "Calidad visual",
                PonderacionPopular = 40,
                PonderacionExperto = 60
            });

            await _servicio.CrearEvento("creador@test.com", dto);

            var categorias = await _context.Categoria.ToListAsync();
            Assert.AreEqual(1, categorias.Count);
            Assert.AreEqual("Diseño", categorias[0].Nombre);
        }

        [TestMethod]
        public async Task CrearEvento_ConCriterios_CreaCriteriosAsociadosACategoria()
        {
            await AgregarUsuario();

            var dto = DtoValido();
            dto.Categorias.Add(new CategoriaDto
            {
                Nombre = "Innovación",
                PonderacionPopular = 50,
                PonderacionExperto = 50,
                Criterios =
                [
                    new CriterioDto { Nombre = "Originalidad", Ponderacion = 60 },
                    new CriterioDto { Nombre = "Impacto", Ponderacion = 40 }
                ]
            });

            await _servicio.CrearEvento("creador@test.com", dto);

            var criterios = await _context.Criterios.ToListAsync();
            Assert.AreEqual(2, criterios.Count);
        }

        [TestMethod]
        public async Task CrearEvento_DosCategoriasConCriterios_CreaTodasEnBD()
        {
            await AgregarUsuario();

            var dto = DtoValido();
            dto.Categorias.Add(new CategoriaDto
            {
                Nombre = "Cat1",
                PonderacionPopular = 50,
                PonderacionExperto = 50,
                Criterios = [new CriterioDto { Nombre = "C1", Ponderacion = 100 }]
            });
            dto.Categorias.Add(new CategoriaDto
            {
                Nombre = "Cat2",
                PonderacionPopular = 30,
                PonderacionExperto = 70,
                Criterios = [new CriterioDto { Nombre = "C2", Ponderacion = 100 }]
            });

            await _servicio.CrearEvento("creador@test.com", dto);

            Assert.AreEqual(2, (await _context.Categoria.ToListAsync()).Count);
            Assert.AreEqual(2, (await _context.Criterios.ToListAsync()).Count);
        }

        // ─── Propiedades del evento creado ────────────────────────────────────────────

        [TestMethod]
        public async Task CrearEvento_ConTipoVotacionPuntos_GuardaTipoCorrectamente()
        {
            await AgregarUsuario();

            var dto = DtoValido();
            dto.TipoVotacion = TipoVotacion.Puntos;
            dto.PuntosARepartir = 150;

            var resultado = await _servicio.CrearEvento("creador@test.com", dto);

            Assert.AreEqual(TipoVotacion.Puntos, resultado.TipoVotacion);
            Assert.AreEqual(150, resultado.PuntosARepartir);
        }

        [TestMethod]
        public async Task CrearEvento_EventoPrivado_GuardaPasswordYEsPrivado()
        {
            await AgregarUsuario();

            var dto = DtoValido();
            dto.EsPrivado = true;
            dto.PasswordAcceso = "secreto123";

            var resultado = await _servicio.CrearEvento("creador@test.com", dto);

            Assert.IsTrue(resultado.EsPrivado);
            Assert.AreEqual("secreto123", resultado.PasswordAcceso);
        }
    }
}
