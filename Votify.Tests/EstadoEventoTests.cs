using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Estado.Interfaz;
using Votify.LógicaNegocio.Estado.States;
using Votify.Persistencia.Entities;

namespace Votify.Tests
{
    [TestClass]
    public class EstadoEventoTests
    {
        private static Evento CrearEvento(FasesEvento faseActual)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);

            return new Evento
            {
                Id = 1,
                Nombre = "Evento de prueba",
                Descripcion = "Evento usado para probar el patrón Estado",
                FechaFinInscripciones = hoy.AddDays(10),
                FechaFinEntrega = hoy.AddDays(20),
                FechaFinVotaciones = hoy.AddDays(30),
                FechaFinEvaluacionExperta = hoy.AddDays(40),
                FechaFinVotacionExperta = hoy.AddDays(50),
                FaseActual = faseActual,
                Estado = "Activo"
            };
        }

        private static void AssertPermisos(
            IEstadoEvento estado,
            bool puedeEntregarProyecto = false,
            bool puedeVotarPopular = false,
            bool puedeVotarExperto = false,
            bool puedeVerResultados = false,
            bool puedeReclamar = false)
        {
            Assert.AreEqual(puedeEntregarProyecto, estado.PuedeEntregarProyecto(), "Permiso de entrega incorrecto.");
            Assert.AreEqual(puedeVotarPopular, estado.PuedeVotarPopular(), "Permiso de voto popular incorrecto.");
            Assert.AreEqual(puedeVotarExperto, estado.PuedeVotarExperto(), "Permiso de voto experto incorrecto.");
            Assert.AreEqual(puedeVerResultados, estado.PuedeVerResultados(), "Permiso de resultados incorrecto.");
            Assert.AreEqual(puedeReclamar, estado.PuedeReclamar(), "Permiso de reclamación incorrecto.");
        }

        // ─── Reconstrucción del EstadoConcreto desde FaseActual ─────────────────────

        [TestMethod]
        public void GetEstado_FaseInscripciones_DevuelveEstadoInscripciones()
        {
            var evento = CrearEvento(FasesEvento.Inscripciones);

            var estado = evento.GetEstado();

            Assert.IsInstanceOfType<EstadoInscripciones>(estado);
            Assert.AreEqual(FasesEvento.Inscripciones, estado.Fase);
        }

        [TestMethod]
        public void GetEstado_FaseEntregas_DevuelveEstadoEntregas()
        {
            var evento = CrearEvento(FasesEvento.Entregas);

            var estado = evento.GetEstado();

            Assert.IsInstanceOfType<EstadoEntregas>(estado);
            Assert.AreEqual(FasesEvento.Entregas, estado.Fase);
        }

        [TestMethod]
        public void GetEstado_FaseVotaciones_DevuelveEstadoVotaciones()
        {
            var evento = CrearEvento(FasesEvento.Votaciones);

            var estado = evento.GetEstado();

            Assert.IsInstanceOfType<EstadoVotaciones>(estado);
            Assert.AreEqual(FasesEvento.Votaciones, estado.Fase);
        }

        [TestMethod]
        public void GetEstado_FaseEvaluacionExperta_DevuelveEstadoEvaluacionExperta()
        {
            var evento = CrearEvento(FasesEvento.EvaluacionExperta);

            var estado = evento.GetEstado();

            Assert.IsInstanceOfType<EstadoEvaluacionExperta>(estado);
            Assert.AreEqual(FasesEvento.EvaluacionExperta, estado.Fase);
        }

        [TestMethod]
        public void GetEstado_FaseVotacionExperta_DevuelveEstadoVotacionExperta()
        {
            var evento = CrearEvento(FasesEvento.VotacionExperta);

            var estado = evento.GetEstado();

            Assert.IsInstanceOfType<EstadoVotacionExperta>(estado);
            Assert.AreEqual(FasesEvento.VotacionExperta, estado.Fase);
        }

        [TestMethod]
        public void GetEstado_FaseResultados_DevuelveEstadoResultados()
        {
            var evento = CrearEvento(FasesEvento.Resultados);

            var estado = evento.GetEstado();

            Assert.IsInstanceOfType<EstadoResultados>(estado);
            Assert.AreEqual(FasesEvento.Resultados, estado.Fase);
        }

        [TestMethod]
        public void GetEstado_SiCambiaFaseActualDirectamente_ReconstruyeEstadoCorrecto()
        {
            var evento = CrearEvento(FasesEvento.Entregas);
            Assert.IsInstanceOfType<EstadoEntregas>(evento.GetEstado());

            evento.FaseActual = FasesEvento.Resultados;

            var estado = evento.GetEstado();

            Assert.IsInstanceOfType<EstadoResultados>(estado);
            Assert.AreEqual(FasesEvento.Resultados, estado.Fase);
        }

        // ─── SetEstado ───────────────────────────────────────────────────────────────

        [TestMethod]
        public void SetEstado_CambiaFaseActualYEstadoConcreto()
        {
            var evento = CrearEvento(FasesEvento.Inscripciones);

            evento.SetEstado(new EstadoVotaciones());

            Assert.AreEqual(FasesEvento.Votaciones, evento.FaseActual);
            Assert.IsInstanceOfType<EstadoVotaciones>(evento.GetEstado());
        }

        // ─── Permisos por fase ──────────────────────────────────────────────────────

        [TestMethod]
        public void EstadoInscripciones_NoPermiteAccionesDeOtrasFases()
        {
            AssertPermisos(new EstadoInscripciones());
        }

        [TestMethod]
        public void EstadoEntregas_SoloPermiteEntregarProyecto()
        {
            AssertPermisos(new EstadoEntregas(), puedeEntregarProyecto: true);
        }

        [TestMethod]
        public void EstadoVotaciones_SoloPermiteVotoPopular()
        {
            AssertPermisos(new EstadoVotaciones(), puedeVotarPopular: true);
        }

        [TestMethod]
        public void EstadoEvaluacionExperta_NoPermiteVotarNiVerResultados()
        {
            AssertPermisos(new EstadoEvaluacionExperta());
        }

        [TestMethod]
        public void EstadoVotacionExperta_SoloPermiteVotoExperto()
        {
            AssertPermisos(new EstadoVotacionExperta(), puedeVotarExperto: true);
        }

        [TestMethod]
        public void EstadoResultados_PermiteVerResultadosYReclamar()
        {
            AssertPermisos(new EstadoResultados(), puedeVerResultados: true, puedeReclamar: true);
        }

        // ─── Transiciones de fase ───────────────────────────────────────────────────

        [TestMethod]
        public void ActualizarFaseSiCorresponde_FechasNoPasadas_NoCambiaFase()
        {
            var evento = CrearEvento(FasesEvento.Inscripciones);

            var haCambiado = evento.ActualizarFaseSiCorresponde();

            Assert.IsFalse(haCambiado);
            Assert.AreEqual(FasesEvento.Inscripciones, evento.FaseActual);
        }

        [TestMethod]
        public void ActualizarFaseSiCorresponde_InscripcionesFinalizadas_CambiaAEntregas()
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var evento = CrearEvento(FasesEvento.Inscripciones);
            evento.FechaFinInscripciones = hoy.AddDays(-1);
            evento.FechaFinEntrega = hoy.AddDays(1);

            var haCambiado = evento.ActualizarFaseSiCorresponde();

            Assert.IsTrue(haCambiado);
            Assert.AreEqual(FasesEvento.Entregas, evento.FaseActual);
            Assert.IsInstanceOfType<EstadoEntregas>(evento.GetEstado());
        }

        [TestMethod]
        public void ActualizarFaseSiCorresponde_EntregasFinalizadas_CambiaAVotaciones()
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var evento = CrearEvento(FasesEvento.Entregas);
            evento.FechaFinEntrega = hoy.AddDays(-1);
            evento.FechaFinVotaciones = hoy.AddDays(1);

            var haCambiado = evento.ActualizarFaseSiCorresponde();

            Assert.IsTrue(haCambiado);
            Assert.AreEqual(FasesEvento.Votaciones, evento.FaseActual);
            Assert.IsInstanceOfType<EstadoVotaciones>(evento.GetEstado());
        }

        [TestMethod]
        public void ActualizarFaseSiCorresponde_VotacionesFinalizadas_CambiaAEvaluacionExperta()
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var evento = CrearEvento(FasesEvento.Votaciones);
            evento.FechaFinVotaciones = hoy.AddDays(-1);
            evento.FechaFinEvaluacionExperta = hoy.AddDays(1);

            var haCambiado = evento.ActualizarFaseSiCorresponde();

            Assert.IsTrue(haCambiado);
            Assert.AreEqual(FasesEvento.EvaluacionExperta, evento.FaseActual);
            Assert.IsInstanceOfType<EstadoEvaluacionExperta>(evento.GetEstado());
        }

        [TestMethod]
        public void ActualizarFaseSiCorresponde_EvaluacionExpertaFinalizada_CambiaAVotacionExperta()
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var evento = CrearEvento(FasesEvento.EvaluacionExperta);
            evento.FechaFinEvaluacionExperta = hoy.AddDays(-1);
            evento.FechaFinVotacionExperta = hoy.AddDays(1);

            var haCambiado = evento.ActualizarFaseSiCorresponde();

            Assert.IsTrue(haCambiado);
            Assert.AreEqual(FasesEvento.VotacionExperta, evento.FaseActual);
            Assert.IsInstanceOfType<EstadoVotacionExperta>(evento.GetEstado());
        }

        [TestMethod]
        public void ActualizarFaseSiCorresponde_VotacionExpertaFinalizada_CambiaAResultados()
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var evento = CrearEvento(FasesEvento.VotacionExperta);
            evento.FechaFinVotacionExperta = hoy.AddDays(-1);

            var haCambiado = evento.ActualizarFaseSiCorresponde();

            Assert.IsTrue(haCambiado);
            Assert.AreEqual(FasesEvento.Resultados, evento.FaseActual);
            Assert.AreEqual("Finalizado", evento.Estado);
            Assert.IsInstanceOfType<EstadoResultados>(evento.GetEstado());
        }

        [TestMethod]
        public void ActualizarFaseSiCorresponde_TodasLasFechasPasadas_SaltaHastaResultados()
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var evento = CrearEvento(FasesEvento.Inscripciones);
            evento.FechaFinInscripciones = hoy.AddDays(-5);
            evento.FechaFinEntrega = hoy.AddDays(-4);
            evento.FechaFinVotaciones = hoy.AddDays(-3);
            evento.FechaFinEvaluacionExperta = hoy.AddDays(-2);
            evento.FechaFinVotacionExperta = hoy.AddDays(-1);

            var haCambiado = evento.ActualizarFaseSiCorresponde();

            Assert.IsTrue(haCambiado);
            Assert.AreEqual(FasesEvento.Resultados, evento.FaseActual);
            Assert.AreEqual("Finalizado", evento.Estado);
            Assert.IsInstanceOfType<EstadoResultados>(evento.GetEstado());
        }
    }
}
