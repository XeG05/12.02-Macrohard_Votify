using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Votify.LógicaNegocio.Domain;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Interfaces;
using Votify.LógicaNegocio.Services;
using Votify.Modelos;
using Votify.Modelos.Persistence.Repositories;
using Votify.Persistencia;
using Votify.Persistencia.Entities;

namespace Votify.Tests
{
    [TestClass]
    public class EventoAdministradorTests
    {
        private VotifyContext _context = null!;
        private Evento_Service _eventoService = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<VotifyContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new VotifyContext(options);

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["App:Url"] = "https://localhost:7001"
                })
                .Build();

            _eventoService = new Evento_Service(
                new Repository<Evento>(_context),
                new Repository<Usuario>(_context),
                new Repository<Proyecto>(_context),
                new Repository<Participante>(_context),
                new Repository<Grupo>(_context),
                new Repository<Categorium>(_context),
                new Repository<Administrador>(_context),
                new Repository<Jurado>(_context),
                new Repository<Votante>(_context),
                new Repository<Rol>(_context),
                new Repository<Criterio>(_context),
                new Repository<Voto>(_context),
                new Repository<Comentario>(_context),
                new Repository<InvitacionPendiente>(_context),
                new FakeEmailService(),
                config);
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.DisposeAsync();
        }
        //Admin puede cambiar el nombre de un evento
        [TestMethod]
        public async Task CambiarNombreAdmin()
        {
            var datos = await CrearEscenarioAdminAsync();
            var admin = await ObtenerEventoAdministrador(datos);

            await admin.CambiarNombre("DemoDay 2026");

            var eventoActualizado = await _context.Eventos.FindAsync(datos.Evento.Id);

            Assert.IsNotNull(eventoActualizado);
            Assert.AreEqual("DemoDay 2026", eventoActualizado.Nombre);
        }
        //Admin puede cambiar la descripcion de un evento
        [TestMethod]
        public async Task CambiarDescrpAdmin()
        {
            var datos = await CrearEscenarioAdminAsync();
            var admin = await ObtenerEventoAdministrador(datos);

            await admin.CambiarDescripcion("Nueva descripcion funcional del evento");

            var eventoActualizado = await _context.Eventos.FindAsync(datos.Evento.Id);

            Assert.IsNotNull(eventoActualizado);
            Assert.AreEqual("Nueva descripcion funcional del evento", eventoActualizado.Descripcion);
        }
        //Admin puede consultar las Categorias de un evento
        [TestMethod]
        public async Task CategoriasEventoAdmin()
        {
            var datos = await CrearEscenarioAdminAsync();
            var admin = await ObtenerEventoAdministrador(datos);

            var categorias = await admin.VerCategoria();

            Assert.HasCount(2, categorias);
            CollectionAssert.AreEquivalent(
                new List<string> { "Web", "IA" },
                categorias.Select(c => c.Nombre).ToList());
        }
        //Admin puede consultar los Grupos de una categoria
        [TestMethod]
        public async Task GruposEventoAdmin()
        {
            var datos = await CrearEscenarioAdminAsync();
            var admin = await ObtenerEventoAdministrador(datos);

            var grupos = await admin.VerGrupo(datos.CategoriaPrincipal.Id);

            Assert.HasCount(2, grupos);
            CollectionAssert.AreEquivalent(
                new List<string> { "Grupo A", "Grupo B" },
                grupos.Select(g => g.Nombre).ToList());
        }
        //Admin puede consultar los Proyectos de un grupo
        [TestMethod]
        public async Task ProyectosGrupoAdmin()
        {
            var datos = await CrearEscenarioAdminAsync();
            var admin = await ObtenerEventoAdministrador(datos);

            var proyectos = await admin.VerProyectoDeGrupo(datos.GrupoA.Id);

            Assert.HasCount(1, proyectos);
            Assert.AreEqual("Proyecto A", proyectos[0].Nombre);
        }
        //Admin puede consultar los Participantes de un grupo
        [TestMethod]
        public async Task ParticipantesGrupoAdmin()
        {
            var datos = await CrearEscenarioAdminAsync();
            var admin = await ObtenerEventoAdministrador(datos);

            var participantes = await admin.VerParticipantes(datos.GrupoA.Id);

            Assert.HasCount(2, participantes);
            CollectionAssert.AreEquivalent(
                new List<string> { "eva@gmail.com", "luis@gmail.com" },
                participantes.Select(p => p.Correo).ToList());
        }
        //Admin puede consultar los Proyectos de una categoria
        [TestMethod]
        public async Task ProyectosCategoriaAdmin()
        {
            var datos = await CrearEscenarioAdminAsync();
            var admin = await ObtenerEventoAdministrador(datos);

            var proyectos = await admin.VerProyectosCategoria(datos.CategoriaPrincipal.Id);

            Assert.HasCount(2, proyectos);
            CollectionAssert.AreEquivalent(
                new List<string> { "Proyecto A", "Proyecto B" },
                proyectos.Select(p => p.Nombre).ToList());
        }
        //Admin puede consultar los votos de un proyecto
        [TestMethod]
        public async Task VotosProyectoAdmin()
        {
            var datos = await CrearEscenarioAdminAsync();
            var admin = await ObtenerEventoAdministrador(datos);

            var votosGrupoA = await admin.ContarVotosProyecto(datos.GrupoA.Id);
            var votosGrupoB = await admin.ContarVotosProyecto(datos.GrupoB.Id);

            Assert.AreEqual(2, votosGrupoA);
            Assert.AreEqual(1, votosGrupoB);
        }
        //Admin puede consultar los resultados de una categoria
        [TestMethod]
        public async Task ResultadosCategoriaAdmin()
        {
            var datos = await CrearEscenarioAdminAsync();
            var admin = await ObtenerEventoAdministrador(datos);

            List<ProyectoResumenDTO> resultados = await admin.ObtenerResultadosCategoria(datos.CategoriaPrincipal.Id);

            Assert.HasCount(2, resultados);

            var resultadoProyectoA = resultados.Single(r => r.NombreProyecto == "Proyecto A");
            var resultadoProyectoB = resultados.Single(r => r.NombreProyecto == "Proyecto B");

            Assert.AreEqual("Grupo A", resultadoProyectoA.NombreGrupo);
            Assert.AreEqual(2, resultadoProyectoA.CantidadVotos);

            Assert.AreEqual("Grupo B", resultadoProyectoB.NombreGrupo);
            Assert.AreEqual(1, resultadoProyectoB.CantidadVotos);
        }
        //Admin puede consultar el total de votos de una categoria
        [TestMethod]
        public async Task VotosTotalesCategoriaAdmin()
        {
            var datos = await CrearEscenarioAdminAsync();
            var admin = await ObtenerEventoAdministrador(datos);

            var totalVotos = await admin.TotalVotosCategoria(datos.CategoriaPrincipal.Id);

            Assert.AreEqual(3, totalVotos);
        }
        //Admin puede añadir un grupo a una categoria durante la fase de inscripciones
        [TestMethod]
        public async Task GrupoFaseinscripionAdmin()
        {
            var datos = await CrearEscenarioAdminAsync();
            var admin = await ObtenerEventoAdministrador(datos);

            await admin.AñadirGrupo("Grupo Nuevo", datos.CategoriaPrincipal.Id);

            var grupoCreado = await _context.Grupos.FirstOrDefaultAsync(g => g.Nombre == "Grupo Nuevo");
            Assert.IsNotNull(grupoCreado);

            var proyectoAsociado = await _context.Proyectos.FirstOrDefaultAsync(p =>
                p.IdGrupo == grupoCreado.Id &&
                p.IdCategoria == datos.CategoriaPrincipal.Id);

            Assert.IsNotNull(proyectoAsociado);
            Assert.AreEqual("Sin nombre", proyectoAsociado.Nombre);
        }
        //Admin no puede añadir un grupo a una categoria fuera de la fase de inscripciones
        [TestMethod]
        public async Task GrupoOtraFaseAdmin()
        {
            var datos = await CrearEscenarioAdminAsync(
                fechaFinInscripciones: DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                fechaFinEntrega: DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
                fechaFinVotaciones: DateOnly.FromDateTime(DateTime.Now.AddDays(10)));

            var admin = await ObtenerEventoAdministrador(datos);

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                admin.AñadirGrupo("Grupo Tardio", datos.CategoriaPrincipal.Id));

            Assert.AreEqual("No se pueden añadir grupos fuera de la fase de inscripción", ex.Message);
        }
        //Admin puede eliminar un grupo en la fase de Inscripciones
        [TestMethod]
        public async Task EliminarGrupoAdmin()
        {
            var datos = await CrearEscenarioAdminAsync();
            var admin = await ObtenerEventoAdministrador(datos);

            await admin.EliminarGrupo(datos.GrupoA.Id);

            var grupo = await _context.Grupos.FindAsync(datos.GrupoA.Id);
            var proyectos = await _context.Proyectos.Where(p => p.IdGrupo == datos.GrupoA.Id).ToListAsync();
            var participantes = await _context.Participantes.Where(p => p.IdGrupo == datos.GrupoA.Id).ToListAsync();

            Assert.IsNull(grupo);
            Assert.IsEmpty(proyectos);
            Assert.IsEmpty(participantes);
        }
        //Admin no puede eliminar un grupo fuera de la fase de Inscripciones
        [TestMethod]
        public async Task EliminarGrupoOtraFaseAdmin()
        {
            var datos = await CrearEscenarioAdminAsync(
                fechaFinInscripciones: DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                fechaFinEntrega: DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
                fechaFinVotaciones: DateOnly.FromDateTime(DateTime.Now.AddDays(10)));

            var admin = await ObtenerEventoAdministrador(datos);

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                admin.EliminarGrupo(datos.GrupoA.Id));

            Assert.AreEqual("No se pueden eliminar grupos fuera de la fase de inscripción", ex.Message);
        }
        //Error al eliminar grupo inexistente
        [TestMethod]
        public async Task EliminarGrupoInexistente()
        {
            var datos = await CrearEscenarioAdminAsync();
            var admin = await ObtenerEventoAdministrador(datos);

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                admin.EliminarGrupo(999999));

            Assert.AreEqual("Grupo no encontrado", ex.Message);
        }

        private async Task<EventoAdministrador> ObtenerEventoAdministrador(DatosEscenarioAdmin datos)
        {
            var entrada = await _eventoService.EntrarEvento(datos.Admin.Correo, datos.Evento.Id);

            Assert.IsNotNull(entrada);
            Assert.IsNotNull(entrada.Evento);
            Assert.IsInstanceOfType(entrada.Evento, typeof(EventoAdministrador));

            return (EventoAdministrador)entrada.Evento;
        }

        private async Task<DatosEscenarioAdmin> CrearEscenarioAdminAsync(
            DateOnly? fechaFinInscripciones = null,
            DateOnly? fechaFinEntrega = null,
            DateOnly? fechaFinVotaciones = null)
        {
            var admin = new Usuario
            {
                Nombre = "Ana",
                Correo = "ana@gmail.com",
                Contraseña = "12345"
            };
            await _context.Usuarios.AddAsync(admin);

            var evento = new Evento
            {
                Nombre = "DemoDay",
                Descripcion = "Evento de pruebas",
                FechaFinInscripciones = fechaFinInscripciones ?? DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
                FechaFinEntrega = fechaFinEntrega ?? DateOnly.FromDateTime(DateTime.Now.AddDays(10)),
                FechaFinVotaciones = fechaFinVotaciones ?? DateOnly.FromDateTime(DateTime.Now.AddDays(15)),
                TieneGrupos = true,
                GruposCompletos = false,
                ParticipantesPorGrupo = 4
            };
            await _context.Eventos.AddAsync(evento);
            await _context.SaveChangesAsync();

            var rolAdmin = new Rol
            {
                CorreoUsuario = admin.Correo,
                IdEvento = evento.Id,
                CorreoUsuarioNavigation = admin,
                IdEventoNavigation = evento
            };
            await _context.Rols.AddAsync(rolAdmin);

            var adminEvento = new Administrador
            {
                Correo = admin.Correo,
                Id = evento.Id,
                Rol = rolAdmin
            };
            await _context.Administradors.AddAsync(adminEvento);

            var categoriaPrincipal = new Categorium
            {
                Nombre = "Web",
                Descripcion = "Categoria web",
                IdEvento = evento.Id,
                IdEventoNavigation = evento,
                PonderacionPopular = 50,
                PonderacionExperto = 50
            };

            var categoriaSecundaria = new Categorium
            {
                Nombre = "IA",
                Descripcion = "Categoria IA",
                IdEvento = evento.Id,
                IdEventoNavigation = evento,
                PonderacionPopular = 40,
                PonderacionExperto = 60
            };

            await _context.Categoria.AddRangeAsync(categoriaPrincipal, categoriaSecundaria);
            await _context.SaveChangesAsync();

            var grupoA = new Grupo
            {
                Nombre = "Grupo A",
                HojaRutaTexto = ""
            };

            var grupoB = new Grupo
            {
                Nombre = "Grupo B",
                HojaRutaTexto = ""
            };

            await _context.Grupos.AddRangeAsync(grupoA, grupoB);
            await _context.SaveChangesAsync();

            var proyectoA = new Proyecto
            {
                Nombre = "Proyecto A",
                IdGrupo = grupoA.Id,
                IdCategoria = categoriaPrincipal.Id,
                Archivo = Array.Empty<byte>(),
                Descripcion = "Descripcion A",
                DescripcionBreve = "Breve A",
                IdGrupoNavigation = grupoA,
                IdCategoriaNavigation = categoriaPrincipal
            };

            var proyectoB = new Proyecto
            {
                Nombre = "Proyecto B",
                IdGrupo = grupoB.Id,
                IdCategoria = categoriaPrincipal.Id,
                Archivo = Array.Empty<byte>(),
                Descripcion = "Descripcion B",
                DescripcionBreve = "Breve B",
                IdGrupoNavigation = grupoB,
                IdCategoriaNavigation = categoriaPrincipal
            };

            await _context.Proyectos.AddRangeAsync(proyectoA, proyectoB);

            var participante1 = await CrearParticipanteAsync("eva@gmail.com", "Eva", "clave1", evento, grupoA);
            var participante2 = await CrearParticipanteAsync("luis@gmail.com", "Luis", "clave2", evento, grupoA);
            await CrearParticipanteAsync("marta@gmail.com", "Marta", "clave3", evento, grupoB);

            await _context.SaveChangesAsync();

            var criterio = new Criterio
            {
                Nombre = "General",
                Descripcion = "Criterio General",
                IdCategoria = categoriaPrincipal.Id,
                Ponderacion = 100
            };
            await _context.Criterios.AddAsync(criterio);
            await _context.SaveChangesAsync();

            await _context.Votos.AddRangeAsync(
                new Voto
                {
                    IdCriterio = criterio.Id,
                    IdRol = evento.Id,
                    CorreoRol = participante1.Correo,
                    IdProyectoCat = categoriaPrincipal.Id,
                    IdProyectoGru = grupoA.Id,
                    Nota = 5
                },
                new Voto
                {
                    IdCriterio = criterio.Id,
                    IdRol = evento.Id,
                    CorreoRol = participante2.Correo,
                    IdProyectoCat = categoriaPrincipal.Id,
                    IdProyectoGru = grupoA.Id,
                    Nota = 4
                },
                new Voto
                {
                    IdCriterio = criterio.Id,
                    IdRol = evento.Id,
                    CorreoRol = participante1.Correo,
                    IdProyectoCat = categoriaPrincipal.Id,
                    IdProyectoGru = grupoB.Id,
                    Nota = 3
                });

            await _context.SaveChangesAsync();

            return new DatosEscenarioAdmin
            {
                Admin = admin,
                Evento = evento,
                CategoriaPrincipal = categoriaPrincipal,
                GrupoA = grupoA,
                GrupoB = grupoB
            };
        }

        private async Task<Participante> CrearParticipanteAsync(
            string correo,
            string nombre,
            string contrasena,
            Evento evento,
            Grupo grupo)
        {
            var usuario = new Usuario
            {
                Nombre = nombre,
                Correo = correo,
                Contraseña = contrasena
            };
            await _context.Usuarios.AddAsync(usuario);

            var rol = new Rol
            {
                CorreoUsuario = correo,
                IdEvento = evento.Id,
                CorreoUsuarioNavigation = usuario,
                IdEventoNavigation = evento
            };
            await _context.Rols.AddAsync(rol);

            var participante = new Participante
            {
                Correo = correo,
                Id = evento.Id,
                IdGrupo = grupo.Id,
                IdGrupoNavigation = grupo,
                Rol = rol
            };
            await _context.Participantes.AddAsync(participante);
            await _context.SaveChangesAsync();

            return participante;
        }

        private sealed class FakeEmailService : IEmailService
        {
            public Task EnviarInvitacionAsync(string correoDestino, string nombreEvento, string nombreCreador, string urlAceptacion)
            {
                return Task.CompletedTask;
            }
        }

        private sealed class DatosEscenarioAdmin
        {
            public Usuario Admin { get; set; } = null!;
            public Evento Evento { get; set; } = null!;
            public Categorium CategoriaPrincipal { get; set; } = null!;
            public Grupo GrupoA { get; set; } = null!;
            public Grupo GrupoB { get; set; } = null!;
        }
    }
}