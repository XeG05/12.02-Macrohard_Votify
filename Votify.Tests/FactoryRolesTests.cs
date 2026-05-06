using Moq;
using Votify.LógicaNegocio.Factory.Roles;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.Tests
{
    [TestClass]
    public class FactoryRolesTests
    {
        private Mock<IRepository<Rol>> _rolRepoMock = null!;

        [TestInitialize]
        public void Setup()
        {
            _rolRepoMock = new Mock<IRepository<Rol>>();
            _rolRepoMock
                .Setup(r => r.AddAsync(It.IsAny<Rol>()))
                .Returns(Task.CompletedTask);
        }

        // ─── AdministradorCreator ────────────────────────────────────────────────────

        [TestMethod]
        public async Task AdministradorCreator_Crear_DevuelveAdministrador()
        {
            var creator = new AdministradorCreator(_rolRepoMock.Object);

            var rol = await creator.CrearAsync("admin@test.com", 42);

            Assert.IsInstanceOfType<Administrador>(rol);
        }

        [TestMethod]
        public async Task AdministradorCreator_Crear_AsignaCorreoYEvento()
        {
            var creator = new AdministradorCreator(_rolRepoMock.Object);

            var rol = await creator.CrearAsync("admin@test.com", 42);

            Assert.AreEqual("admin@test.com", rol.CorreoUsuario);
            Assert.AreEqual(42, rol.IdEvento);
        }

        [TestMethod]
        public async Task AdministradorCreator_Crear_LlamaAddAsync()
        {
            var creator = new AdministradorCreator(_rolRepoMock.Object);

            await creator.CrearAsync("admin@test.com", 1);

            _rolRepoMock.Verify(r => r.AddAsync(It.IsAny<Administrador>()), Times.Once);
        }

        // ─── VotanteCreator ──────────────────────────────────────────────────────────

        [TestMethod]
        public async Task VotanteCreator_Crear_DevuelveVotante()
        {
            var creator = new VotanteCreator(_rolRepoMock.Object);

            var rol = await creator.CrearAsync("votante@test.com", 10);

            Assert.IsInstanceOfType<Votante>(rol);
        }

        [TestMethod]
        public async Task VotanteCreator_Crear_AsignaCorreoYEvento()
        {
            var creator = new VotanteCreator(_rolRepoMock.Object);

            var rol = await creator.CrearAsync("votante@test.com", 10);

            Assert.AreEqual("votante@test.com", rol.CorreoUsuario);
            Assert.AreEqual(10, rol.IdEvento);
        }

        [TestMethod]
        public async Task VotanteCreator_Crear_LlamaAddAsync()
        {
            var creator = new VotanteCreator(_rolRepoMock.Object);

            await creator.CrearAsync("votante@test.com", 1);

            _rolRepoMock.Verify(r => r.AddAsync(It.IsAny<Votante>()), Times.Once);
        }

        // ─── JuradoCreator ───────────────────────────────────────────────────────────

        [TestMethod]
        public async Task JuradoCreator_Crear_DevuelveJurado()
        {
            var creator = new JuradoCreator(_rolRepoMock.Object);

            var rol = await creator.CrearAsync("jurado@test.com", 7);

            Assert.IsInstanceOfType<Jurado>(rol);
        }

        [TestMethod]
        public async Task JuradoCreator_Crear_AsignaCorreoYEvento()
        {
            var creator = new JuradoCreator(_rolRepoMock.Object);

            var rol = await creator.CrearAsync("jurado@test.com", 7);

            Assert.AreEqual("jurado@test.com", rol.CorreoUsuario);
            Assert.AreEqual(7, rol.IdEvento);
        }

        [TestMethod]
        public async Task JuradoCreator_Crear_LlamaAddAsync()
        {
            var creator = new JuradoCreator(_rolRepoMock.Object);

            await creator.CrearAsync("jurado@test.com", 1);

            _rolRepoMock.Verify(r => r.AddAsync(It.IsAny<Jurado>()), Times.Once);
        }

        // ─── ParticipanteCreator ─────────────────────────────────────────────────────

        [TestMethod]
        public async Task ParticipanteCreator_CrearConGrupo_DevuelveParticipante()
        {
            var creator = new ParticipanteCreator(_rolRepoMock.Object);

            var rol = await creator.CrearAsync("part@test.com", 5, idGrupo: 99);

            Assert.IsInstanceOfType<Participante>(rol);
        }

        [TestMethod]
        public async Task ParticipanteCreator_CrearConGrupo_AsignaCorreoEventoYGrupo()
        {
            var creator = new ParticipanteCreator(_rolRepoMock.Object);

            var rol = await creator.CrearAsync("part@test.com", 5, idGrupo: 99);

            var participante = (Participante)rol;
            Assert.AreEqual("part@test.com", participante.CorreoUsuario);
            Assert.AreEqual(5, participante.IdEvento);
            Assert.AreEqual(99, participante.IdGrupo);
        }

        [TestMethod]
        public async Task ParticipanteCreator_CrearConGrupo_LlamaAddAsync()
        {
            var creator = new ParticipanteCreator(_rolRepoMock.Object);

            await creator.CrearAsync("part@test.com", 1, idGrupo: 2);

            _rolRepoMock.Verify(r => r.AddAsync(It.IsAny<Participante>()), Times.Once);
        }

        [TestMethod]
        public async Task ParticipanteCreator_CrearSinGrupo_LanzaNotSupportedException()
        {
            var creator = new ParticipanteCreator(_rolRepoMock.Object);

            NotSupportedException? excepcion = null;
            try { await creator.CrearAsync("part@test.com", 1); }
            catch (NotSupportedException ex) { excepcion = ex; }

            Assert.IsNotNull(excepcion, "Se esperaba NotSupportedException");
        }

        // ─── Polimorfismo ────────────────────────────────────────────────────────────

        [TestMethod]
        public async Task CadaCreator_DevuelveTipoDistinto_ParaMismosParametros()
        {
            var adminCreator = new AdministradorCreator(_rolRepoMock.Object);
            var votanteCreator = new VotanteCreator(_rolRepoMock.Object);
            var juradoCreator = new JuradoCreator(_rolRepoMock.Object);

            var admin = await adminCreator.CrearAsync("u@test.com", 1);
            var votante = await votanteCreator.CrearAsync("u@test.com", 1);
            var jurado = await juradoCreator.CrearAsync("u@test.com", 1);

            Assert.IsInstanceOfType<Administrador>(admin);
            Assert.IsInstanceOfType<Votante>(votante);
            Assert.IsInstanceOfType<Jurado>(jurado);
        }
    }
}
