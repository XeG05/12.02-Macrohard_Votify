using Microsoft.Extensions.Logging;
using Moq;
using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Interfaces;
using Votify.LógicaNegocio.Observador.Interfaces;
using Votify.LógicaNegocio.Observador.Observers;
using Votify.LógicaNegocio.Observador.Sujetos;
using Votify.Persistencia.Entities;

namespace Votify.Tests
{
    [TestClass]
    public class ObservadorTests
    {
        private Mock<ILogger<EventoSujeto>> _loggerMock = null!;
        private Mock<INotificacionService> _notificacionMock = null!;

        [TestInitialize]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<EventoSujeto>>();
            _notificacionMock = new Mock<INotificacionService>();
            _notificacionMock
                .Setup(n => n.CrearSiNoExisteAsync(
                    It.IsAny<string>(), It.IsAny<long>(),
                    It.IsAny<TipoNotificacion>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
        }

        private EventoSujeto CrearSujeto(IEnumerable<IObservadorEvento>? observadores = null)
            => new(_loggerMock.Object, observadores ?? []);

        // Evento donde FaseActual ya coincide con la fase calculada → no habrá cambio
        private static Evento EventoSinCambioFase()
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            return new Evento
            {
                Id = 1, Nombre = "Test", Descripcion = "D",
                FechaFinInscripciones = hoy.AddDays(10),
                FechaFinEntrega = hoy.AddDays(15),
                FechaFinVotaciones = hoy.AddDays(20),
                FechaFinEvaluacionExperta = hoy.AddDays(25),
                FechaFinVotacionExperta = hoy.AddDays(30),
                FaseActual = FasesEvento.Inscripciones
            };
        }

        // Evento que transicionará de Inscripciones → Votaciones al llamar SetEstadoAsync
        private static Evento EventoParaFase(FasesEvento fasaObjetivo)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            return fasaObjetivo switch
            {
                FasesEvento.Entregas => new Evento
                {
                    Id = 1, Nombre = "Test", Descripcion = "D",
                    FechaFinInscripciones = hoy.AddDays(-1),
                    FechaFinEntrega = hoy.AddDays(5),
                    FechaFinVotaciones = hoy.AddDays(10),
                    FechaFinEvaluacionExperta = hoy.AddDays(15),
                    FechaFinVotacionExperta = hoy.AddDays(20),
                    FaseActual = FasesEvento.Inscripciones
                },
                FasesEvento.Votaciones => new Evento
                {
                    Id = 1, Nombre = "Test", Descripcion = "D",
                    FechaFinInscripciones = hoy.AddDays(-5),
                    FechaFinEntrega = hoy.AddDays(-1),
                    FechaFinVotaciones = hoy.AddDays(5),
                    FechaFinEvaluacionExperta = hoy.AddDays(10),
                    FechaFinVotacionExperta = hoy.AddDays(15),
                    FaseActual = FasesEvento.Inscripciones
                },
                FasesEvento.EvaluacionExperta => new Evento
                {
                    Id = 1, Nombre = "Test", Descripcion = "D",
                    FechaFinInscripciones = hoy.AddDays(-10),
                    FechaFinEntrega = hoy.AddDays(-5),
                    FechaFinVotaciones = hoy.AddDays(-1),
                    FechaFinEvaluacionExperta = hoy.AddDays(5),
                    FechaFinVotacionExperta = hoy.AddDays(10),
                    FaseActual = FasesEvento.Inscripciones
                },
                FasesEvento.VotacionExperta => new Evento
                {
                    Id = 1, Nombre = "Test", Descripcion = "D",
                    FechaFinInscripciones = hoy.AddDays(-15),
                    FechaFinEntrega = hoy.AddDays(-10),
                    FechaFinVotaciones = hoy.AddDays(-5),
                    FechaFinEvaluacionExperta = hoy.AddDays(-1),
                    FechaFinVotacionExperta = hoy.AddDays(5),
                    FaseActual = FasesEvento.Inscripciones
                },
                FasesEvento.Resultados => new Evento
                {
                    Id = 1, Nombre = "Test", Descripcion = "D",
                    FechaFinInscripciones = hoy.AddDays(-20),
                    FechaFinEntrega = hoy.AddDays(-15),
                    FechaFinVotaciones = hoy.AddDays(-10),
                    FechaFinEvaluacionExperta = hoy.AddDays(-5),
                    FechaFinVotacionExperta = hoy.AddDays(-1),
                    FaseActual = FasesEvento.Inscripciones
                },
                _ => throw new ArgumentOutOfRangeException(nameof(fasaObjetivo))
            };
        }

        // ─── EventoSujeto: Attach / Detach ───────────────────────────────────────────

        [TestMethod]
        public async Task EventoSujeto_Attach_ObservadorNuevo_RecibeLaNotificacion()
        {
            var observadorMock = new Mock<IObservadorEvento>();
            observadorMock
                .Setup(o => o.ActualizarAsync(It.IsAny<IEventoSujeto>()))
                .Returns(Task.CompletedTask);

            var sujeto = CrearSujeto(); // sin observadores iniciales
            sujeto.Attach(observadorMock.Object);

            await sujeto.SetEstadoAsync(EventoParaFase(FasesEvento.Votaciones));

            observadorMock.Verify(o => o.ActualizarAsync(sujeto), Times.Once);
        }

        [TestMethod]
        public async Task EventoSujeto_Attach_MismoObservadorDosVeces_SoloSeRegistraUna()
        {
            var observadorMock = new Mock<IObservadorEvento>();
            observadorMock
                .Setup(o => o.ActualizarAsync(It.IsAny<IEventoSujeto>()))
                .Returns(Task.CompletedTask);

            var sujeto = CrearSujeto();
            sujeto.Attach(observadorMock.Object);
            sujeto.Attach(observadorMock.Object); // duplicado

            await sujeto.SetEstadoAsync(EventoParaFase(FasesEvento.Votaciones));

            observadorMock.Verify(o => o.ActualizarAsync(sujeto), Times.Once);
        }

        [TestMethod]
        public async Task EventoSujeto_Detach_ObservadorEliminado_NoRecibeLaNotificacion()
        {
            var observadorMock = new Mock<IObservadorEvento>();
            observadorMock
                .Setup(o => o.ActualizarAsync(It.IsAny<IEventoSujeto>()))
                .Returns(Task.CompletedTask);

            var sujeto = CrearSujeto([observadorMock.Object]);
            sujeto.Detach(observadorMock.Object);

            await sujeto.SetEstadoAsync(EventoParaFase(FasesEvento.Votaciones));

            observadorMock.Verify(
                o => o.ActualizarAsync(It.IsAny<IEventoSujeto>()), Times.Never);
        }

        // ─── EventoSujeto: SetEstadoAsync ────────────────────────────────────────────

        [TestMethod]
        public async Task EventoSujeto_SetEstado_SinCambioFase_NoNotifica()
        {
            var observadorMock = new Mock<IObservadorEvento>();
            var sujeto = CrearSujeto([observadorMock.Object]);

            await sujeto.SetEstadoAsync(EventoSinCambioFase());

            observadorMock.Verify(
                o => o.ActualizarAsync(It.IsAny<IEventoSujeto>()), Times.Never);
        }

        [TestMethod]
        public async Task EventoSujeto_SetEstado_ConCambioFase_NotificaATodosObservadores()
        {
            var obs1 = new Mock<IObservadorEvento>();
            var obs2 = new Mock<IObservadorEvento>();
            obs1.Setup(o => o.ActualizarAsync(It.IsAny<IEventoSujeto>())).Returns(Task.CompletedTask);
            obs2.Setup(o => o.ActualizarAsync(It.IsAny<IEventoSujeto>())).Returns(Task.CompletedTask);

            var sujeto = CrearSujeto([obs1.Object, obs2.Object]);

            await sujeto.SetEstadoAsync(EventoParaFase(FasesEvento.Votaciones));

            obs1.Verify(o => o.ActualizarAsync(sujeto), Times.Once);
            obs2.Verify(o => o.ActualizarAsync(sujeto), Times.Once);
        }

        [TestMethod]
        public async Task EventoSujeto_SetEstado_ConCambioFase_ActualizaFaseActualYFaseAnterior()
        {
            var sujeto = CrearSujeto();

            await sujeto.SetEstadoAsync(EventoParaFase(FasesEvento.Votaciones));

            Assert.AreEqual(FasesEvento.Inscripciones, sujeto.FaseAnterior);
            Assert.AreEqual(FasesEvento.Votaciones, sujeto.FaseActual);
        }

        [TestMethod]
        public async Task EventoSujeto_Notificar_SiUnObservadorFalla_ContinuaConElSiguiente()
        {
            var obs1 = new Mock<IObservadorEvento>();
            var obs2 = new Mock<IObservadorEvento>();
            obs1.Setup(o => o.ActualizarAsync(It.IsAny<IEventoSujeto>()))
                .ThrowsAsync(new Exception("fallo del primer observador"));
            obs2.Setup(o => o.ActualizarAsync(It.IsAny<IEventoSujeto>()))
                .Returns(Task.CompletedTask);

            var sujeto = CrearSujeto([obs1.Object, obs2.Object]);

            await sujeto.SetEstadoAsync(EventoParaFase(FasesEvento.Votaciones));

            // El segundo observador debe recibir la notificación a pesar del fallo del primero
            obs2.Verify(o => o.ActualizarAsync(sujeto), Times.Once);
        }

        // ─── ObservadorVotantes ──────────────────────────────────────────────────────

        [TestMethod]
        public async Task ObservadorVotantes_FaseNoEsVotaciones_NoEnviaNotificacion()
        {
            var observer = new ObservadorVotantes(_notificacionMock.Object);
            var sujetoMock = new Mock<IEventoSujeto>();
            sujetoMock.Setup(s => s.FaseActual).Returns(FasesEvento.Entregas);

            await observer.ActualizarAsync(sujetoMock.Object);

            _notificacionMock.Verify(
                n => n.CrearSiNoExisteAsync(
                    It.IsAny<string>(), It.IsAny<long>(),
                    It.IsAny<TipoNotificacion>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public async Task ObservadorVotantes_FaseEsVotaciones_NotificaSoloAVotantes()
        {
            var observer = new ObservadorVotantes(_notificacionMock.Object);

            var evento = new Evento { Id = 1, Nombre = "Test" };
            evento.Roles.Add(new Votante { CorreoUsuario = "v1@test.com", IdEvento = 1 });
            evento.Roles.Add(new Votante { CorreoUsuario = "v2@test.com", IdEvento = 1 });
            evento.Roles.Add(new Participante { CorreoUsuario = "p@test.com", IdEvento = 1 }); // no debe notificarse

            var sujetoMock = new Mock<IEventoSujeto>();
            sujetoMock.Setup(s => s.FaseActual).Returns(FasesEvento.Votaciones);
            sujetoMock.Setup(s => s.Evento).Returns(evento);

            await observer.ActualizarAsync(sujetoMock.Object);

            _notificacionMock.Verify(
                n => n.CrearSiNoExisteAsync(
                    It.IsAny<string>(), 1, TipoNotificacion.InicioVotacionPopular,
                    It.IsAny<string>(), It.IsAny<string>()),
                Times.Exactly(2));
        }

        [TestMethod]
        public async Task ObservadorVotantes_FaseEsVotaciones_NotificaConCorreosCorrecto()
        {
            var observer = new ObservadorVotantes(_notificacionMock.Object);

            var evento = new Evento { Id = 5, Nombre = "Evento X" };
            evento.Roles.Add(new Votante { CorreoUsuario = "solo@test.com", IdEvento = 5 });

            var sujetoMock = new Mock<IEventoSujeto>();
            sujetoMock.Setup(s => s.FaseActual).Returns(FasesEvento.Votaciones);
            sujetoMock.Setup(s => s.Evento).Returns(evento);

            await observer.ActualizarAsync(sujetoMock.Object);

            _notificacionMock.Verify(
                n => n.CrearSiNoExisteAsync(
                    "solo@test.com", 5, TipoNotificacion.InicioVotacionPopular,
                    It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);
        }

        // ─── ObservadorParticipantes ─────────────────────────────────────────────────

        [TestMethod]
        public async Task ObservadorParticipantes_FaseNoEsEntregas_NoEnviaNotificacion()
        {
            var observer = new ObservadorParticipantes(_notificacionMock.Object);
            var sujetoMock = new Mock<IEventoSujeto>();
            sujetoMock.Setup(s => s.FaseActual).Returns(FasesEvento.Votaciones);

            await observer.ActualizarAsync(sujetoMock.Object);

            _notificacionMock.Verify(
                n => n.CrearSiNoExisteAsync(
                    It.IsAny<string>(), It.IsAny<long>(),
                    It.IsAny<TipoNotificacion>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public async Task ObservadorParticipantes_FaseEsEntregas_NotificaSoloAParticipantes()
        {
            var observer = new ObservadorParticipantes(_notificacionMock.Object);

            var evento = new Evento { Id = 2, Nombre = "Test" };
            evento.Roles.Add(new Participante { CorreoUsuario = "p1@test.com", IdEvento = 2 });
            evento.Roles.Add(new Participante { CorreoUsuario = "p2@test.com", IdEvento = 2 });
            evento.Roles.Add(new Votante { CorreoUsuario = "v@test.com", IdEvento = 2 }); // no debe notificarse

            var sujetoMock = new Mock<IEventoSujeto>();
            sujetoMock.Setup(s => s.FaseActual).Returns(FasesEvento.Entregas);
            sujetoMock.Setup(s => s.Evento).Returns(evento);

            await observer.ActualizarAsync(sujetoMock.Object);

            _notificacionMock.Verify(
                n => n.CrearSiNoExisteAsync(
                    It.IsAny<string>(), 2, TipoNotificacion.InicioEntrega,
                    It.IsAny<string>(), It.IsAny<string>()),
                Times.Exactly(2));
        }

        // ─── ObservadorJurado ────────────────────────────────────────────────────────

        [TestMethod]
        public async Task ObservadorJurado_FaseEsEvaluacionExperta_NotificaConTipoComentarios()
        {
            var observer = new ObservadorJurado(_notificacionMock.Object);

            var evento = new Evento { Id = 3, Nombre = "Test" };
            evento.Roles.Add(new Jurado { CorreoUsuario = "j@test.com", IdEvento = 3 });

            var sujetoMock = new Mock<IEventoSujeto>();
            sujetoMock.Setup(s => s.FaseActual).Returns(FasesEvento.EvaluacionExperta);
            sujetoMock.Setup(s => s.Evento).Returns(evento);

            await observer.ActualizarAsync(sujetoMock.Object);

            _notificacionMock.Verify(
                n => n.CrearSiNoExisteAsync(
                    "j@test.com", 3, TipoNotificacion.InicioComentariosJurado,
                    It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);
        }

        [TestMethod]
        public async Task ObservadorJurado_FaseEsVotacionExperta_NotificaConTipoVotacionExperta()
        {
            var observer = new ObservadorJurado(_notificacionMock.Object);

            var evento = new Evento { Id = 3, Nombre = "Test" };
            evento.Roles.Add(new Jurado { CorreoUsuario = "j@test.com", IdEvento = 3 });

            var sujetoMock = new Mock<IEventoSujeto>();
            sujetoMock.Setup(s => s.FaseActual).Returns(FasesEvento.VotacionExperta);
            sujetoMock.Setup(s => s.Evento).Returns(evento);

            await observer.ActualizarAsync(sujetoMock.Object);

            _notificacionMock.Verify(
                n => n.CrearSiNoExisteAsync(
                    "j@test.com", 3, TipoNotificacion.InicioVotacionExperta,
                    It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);
        }

        [TestMethod]
        public async Task ObservadorJurado_FaseNoEsExpertaNiVotacion_NoEnviaNotificacion()
        {
            var observer = new ObservadorJurado(_notificacionMock.Object);
            var sujetoMock = new Mock<IEventoSujeto>();
            sujetoMock.Setup(s => s.FaseActual).Returns(FasesEvento.Votaciones);

            await observer.ActualizarAsync(sujetoMock.Object);

            _notificacionMock.Verify(
                n => n.CrearSiNoExisteAsync(
                    It.IsAny<string>(), It.IsAny<long>(),
                    It.IsAny<TipoNotificacion>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public async Task ObservadorJurado_MultiJurados_NotificaATodos()
        {
            var observer = new ObservadorJurado(_notificacionMock.Object);

            var evento = new Evento { Id = 4, Nombre = "Test" };
            evento.Roles.Add(new Jurado { CorreoUsuario = "j1@test.com", IdEvento = 4 });
            evento.Roles.Add(new Jurado { CorreoUsuario = "j2@test.com", IdEvento = 4 });
            evento.Roles.Add(new Jurado { CorreoUsuario = "j3@test.com", IdEvento = 4 });

            var sujetoMock = new Mock<IEventoSujeto>();
            sujetoMock.Setup(s => s.FaseActual).Returns(FasesEvento.EvaluacionExperta);
            sujetoMock.Setup(s => s.Evento).Returns(evento);

            await observer.ActualizarAsync(sujetoMock.Object);

            _notificacionMock.Verify(
                n => n.CrearSiNoExisteAsync(
                    It.IsAny<string>(), 4, TipoNotificacion.InicioComentariosJurado,
                    It.IsAny<string>(), It.IsAny<string>()),
                Times.Exactly(3));
        }

        // ─── ObservadorFinalizacion ──────────────────────────────────────────────────

        [TestMethod]
        public async Task ObservadorFinalizacion_FaseNoEsResultados_NoEnviaNotificacion()
        {
            var observer = new ObservadorFinalizacion(_notificacionMock.Object);
            var sujetoMock = new Mock<IEventoSujeto>();
            sujetoMock.Setup(s => s.FaseActual).Returns(FasesEvento.VotacionExperta);

            await observer.ActualizarAsync(sujetoMock.Object);

            _notificacionMock.Verify(
                n => n.CrearSiNoExisteAsync(
                    It.IsAny<string>(), It.IsAny<long>(),
                    It.IsAny<TipoNotificacion>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public async Task ObservadorFinalizacion_FaseEsResultados_NotificaATodosLosRoles()
        {
            var observer = new ObservadorFinalizacion(_notificacionMock.Object);

            var evento = new Evento { Id = 6, Nombre = "Test" };
            evento.Roles.Add(new Administrador { CorreoUsuario = "admin@test.com", IdEvento = 6 });
            evento.Roles.Add(new Votante { CorreoUsuario = "votante@test.com", IdEvento = 6 });
            evento.Roles.Add(new Participante { CorreoUsuario = "part@test.com", IdEvento = 6 });

            var sujetoMock = new Mock<IEventoSujeto>();
            sujetoMock.Setup(s => s.FaseActual).Returns(FasesEvento.Resultados);
            sujetoMock.Setup(s => s.Evento).Returns(evento);

            await observer.ActualizarAsync(sujetoMock.Object);

            _notificacionMock.Verify(
                n => n.CrearSiNoExisteAsync(
                    It.IsAny<string>(), 6, TipoNotificacion.EventoFinalizado,
                    It.IsAny<string>(), It.IsAny<string>()),
                Times.Exactly(3));
        }

        [TestMethod]
        public async Task ObservadorFinalizacion_FaseEsResultados_DeduplicaCorreos()
        {
            var observer = new ObservadorFinalizacion(_notificacionMock.Object);

            var evento = new Evento { Id = 7, Nombre = "Test" };
            // Mismo correo en dos roles distintos → solo debe notificarse una vez
            evento.Roles.Add(new Votante { CorreoUsuario = "dual@test.com", IdEvento = 7 });
            evento.Roles.Add(new Administrador { CorreoUsuario = "dual@test.com", IdEvento = 7 });

            var sujetoMock = new Mock<IEventoSujeto>();
            sujetoMock.Setup(s => s.FaseActual).Returns(FasesEvento.Resultados);
            sujetoMock.Setup(s => s.Evento).Returns(evento);

            await observer.ActualizarAsync(sujetoMock.Object);

            _notificacionMock.Verify(
                n => n.CrearSiNoExisteAsync(
                    "dual@test.com", 7, TipoNotificacion.EventoFinalizado,
                    It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);
        }
    }
}
