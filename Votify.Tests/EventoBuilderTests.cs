using Votify.LógicaNegocio.Builder;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Enums;
using Votify.Persistencia.Entities;

namespace Votify.Tests
{
    [TestClass]
    public class EventoBuilderTests
    {
        private EventoConcretoBuilder _builder = null!;
        private EventoDirector _director = null!;

        [TestInitialize]
        public void Setup()
        {
            _builder = new EventoConcretoBuilder();
            _director = new EventoDirector();
        }

        private static EventoCreacionDto DtoValido() => new()
        {
            Nombre = "Hackathon",
            Descripcion = "Evento de prueba",
            FechaFinInscripciones = new DateOnly(2030, 1, 10),
            FechaFinEntrega = new DateOnly(2030, 2, 10),
            FechaFinVotaciones = new DateOnly(2030, 3, 10),
            FechaFinExperta = new DateOnly(2030, 4, 10),
            FechaFinVotExperta = new DateOnly(2030, 5, 10),
            TipoComentario = TipoComentario.Opcional,
            TipoVotacion = TipoVotacion.Puntos,
            ParticipantesVotan = true,
            NumVotarProyectos = 5,
            PuntosARepartir = 200,
            TieneGrupos = true,
            GruposCompletos = false,
            NumParticipantes = 4,
            EsPrivado = true,
            PasswordAcceso = "secreto"
        };

        // ─── ConcreteBuilder: pasos individuales ─────────────────────────────────────

        [TestMethod]
        public void ConIdentificacion_EstableceNombreYDescripcion()
        {
            _builder.ConIdentificacion("MiEvento", "Una descripción");

            var evento = _builder.GetResultado();

            Assert.AreEqual("MiEvento", evento.Nombre);
            Assert.AreEqual("Una descripción", evento.Descripcion);
        }

        [TestMethod]
        public void ConFechas_EstableceTodasLasFechas()
        {
            var inscripciones = new DateOnly(2030, 1, 1);
            var entrega = new DateOnly(2030, 2, 1);
            var votaciones = new DateOnly(2030, 3, 1);
            var evalExperta = new DateOnly(2030, 4, 1);
            var votExperta = new DateOnly(2030, 5, 1);

            _builder.ConFechas(inscripciones, entrega, votaciones, evalExperta, votExperta);
            var evento = _builder.GetResultado();

            Assert.AreEqual(inscripciones, evento.FechaFinInscripciones);
            Assert.AreEqual(entrega, evento.FechaFinEntrega);
            Assert.AreEqual(votaciones, evento.FechaFinVotaciones);
            Assert.AreEqual(evalExperta, evento.FechaFinEvaluacionExperta);
            Assert.AreEqual(votExperta, evento.FechaFinVotacionExperta);
        }

        [TestMethod]
        public void ConComentarios_EstableceTipoComentario()
        {
            _builder.ConComentarios(TipoComentario.Obligatorio);

            var evento = _builder.GetResultado();

            Assert.AreEqual(TipoComentario.Obligatorio, evento.TipoComentario);
        }

        [TestMethod]
        public void ConVotacion_EstableceTodosLosCamposDeVotacion()
        {
            _builder.ConVotacion(TipoVotacion.Nota, participantesVotan: true,
                numVotarProyectos: 3, puntosARepartir: 50);

            var evento = _builder.GetResultado();

            Assert.AreEqual(TipoVotacion.Nota, evento.TipoVotacion);
            Assert.IsTrue(evento.ParticipantesVotan);
            Assert.AreEqual(3, evento.NumVotarProyectos);
            Assert.AreEqual(50, evento.PuntosARepartir);
        }

        [TestMethod]
        public void ConGrupos_EstableceConfiguracionDeGrupos()
        {
            _builder.ConGrupos(tieneGrupos: true, gruposCompletos: true, participantesPorGrupo: 6);

            var evento = _builder.GetResultado();

            Assert.IsTrue(evento.TieneGrupos);
            Assert.IsTrue(evento.GruposCompletos);
            Assert.AreEqual(6, evento.ParticipantesPorGrupo);
        }

        [TestMethod]
        public void ConPrivacidad_EventoPrivado_GuardaPassword()
        {
            _builder.ConPrivacidad(esPrivado: true, passwordAcceso: "pass123");

            var evento = _builder.GetResultado();

            Assert.IsTrue(evento.EsPrivado);
            Assert.AreEqual("pass123", evento.PasswordAcceso);
        }

        [TestMethod]
        public void ConPrivacidad_EventoPublico_IgnoraPassword()
        {
            _builder.ConPrivacidad(esPrivado: false, passwordAcceso: "deberia-ignorarse");

            var evento = _builder.GetResultado();

            Assert.IsFalse(evento.EsPrivado);
            Assert.IsNull(evento.PasswordAcceso);
        }

        // ─── ConcreteBuilder: fluidez y estado por defecto ───────────────────────────

        [TestMethod]
        public void Builder_PermiteEncadenarPasos_FluentInterface()
        {
            var resultado = _builder
                .ConIdentificacion("X", "Y")
                .ConComentarios(TipoComentario.SinComentarios)
                .ConGrupos(false, false, 1);

            Assert.AreSame(_builder, resultado);
        }

        [TestMethod]
        public void GetResultado_SinPasos_DevuelveEventoConValoresPorDefecto()
        {
            var evento = _builder.GetResultado();

            Assert.AreEqual("Activo", evento.Estado);
            Assert.AreEqual(FasesEvento.Inscripciones, evento.FaseActual);
        }

        [TestMethod]
        public void GetResultado_ReseteaElBuilder_NoArrastraEstadoEntreConstrucciones()
        {
            _builder.ConIdentificacion("Primero", "Desc1");
            var primero = _builder.GetResultado();

            // Sin reset explícito: el siguiente build no debe heredar "Primero".
            _builder.ConIdentificacion("Segundo", "Desc2");
            var segundo = _builder.GetResultado();

            Assert.AreEqual("Primero", primero.Nombre);
            Assert.AreEqual("Segundo", segundo.Nombre);
            Assert.AreNotSame(primero, segundo);
        }

        [TestMethod]
        public void Reset_DescartaElEventoEnConstruccion()
        {
            _builder.ConIdentificacion("Temporal", "Se descartará");

            _builder.Reset();
            var evento = _builder.GetResultado();

            Assert.AreNotEqual("Temporal", evento.Nombre);
            Assert.AreEqual(string.Empty, evento.Nombre ?? string.Empty);
        }

        // ─── Director ────────────────────────────────────────────────────────────────

        [TestMethod]
        public void Director_Construir_DevuelveEventoNoNulo()
        {
            var evento = _director.Construir(_builder, DtoValido());

            Assert.IsNotNull(evento);
            Assert.IsInstanceOfType<Evento>(evento);
        }

        [TestMethod]
        public void Director_Construir_MapeaTodosLosCamposDelDto()
        {
            var dto = DtoValido();

            var evento = _director.Construir(_builder, dto);

            Assert.AreEqual(dto.Nombre, evento.Nombre);
            Assert.AreEqual(dto.Descripcion, evento.Descripcion);
            Assert.AreEqual(dto.FechaFinInscripciones, evento.FechaFinInscripciones);
            Assert.AreEqual(dto.FechaFinEntrega, evento.FechaFinEntrega);
            Assert.AreEqual(dto.FechaFinVotaciones, evento.FechaFinVotaciones);
            Assert.AreEqual(dto.FechaFinExperta, evento.FechaFinEvaluacionExperta);
            Assert.AreEqual(dto.FechaFinVotExperta, evento.FechaFinVotacionExperta);
            Assert.AreEqual(dto.TipoComentario, evento.TipoComentario);
            Assert.AreEqual(dto.TipoVotacion, evento.TipoVotacion);
            Assert.AreEqual(dto.ParticipantesVotan, evento.ParticipantesVotan);
            Assert.AreEqual(dto.NumVotarProyectos, evento.NumVotarProyectos);
            Assert.AreEqual(dto.PuntosARepartir, evento.PuntosARepartir);
            Assert.AreEqual(dto.TieneGrupos, evento.TieneGrupos);
            Assert.AreEqual(dto.GruposCompletos, evento.GruposCompletos);
            Assert.AreEqual(dto.NumParticipantes, evento.ParticipantesPorGrupo);
            Assert.AreEqual(dto.EsPrivado, evento.EsPrivado);
            Assert.AreEqual(dto.PasswordAcceso, evento.PasswordAcceso);
        }

        [TestMethod]
        public void Director_Construir_FuncionaConElMismoBuilderDosVeces()
        {
            var dto1 = DtoValido();
            var dto2 = DtoValido();
            dto2.Nombre = "OtroEvento";
            dto2.EsPrivado = false;
            dto2.PasswordAcceso = null;

            var evento1 = _director.Construir(_builder, dto1);
            var evento2 = _director.Construir(_builder, dto2);

            Assert.AreEqual("Hackathon", evento1.Nombre);
            Assert.AreEqual("OtroEvento", evento2.Nombre);
            Assert.IsTrue(evento1.EsPrivado);
            Assert.IsFalse(evento2.EsPrivado);
            Assert.AreNotSame(evento1, evento2);
        }

        [TestMethod]
        public void Director_Construir_AceptaCualquierEventoBuilder_Polimorfismo()
        {
            // El Director sólo conoce la interfaz abstracta del Builder.
            EventoBuilder builderAbstracto = new EventoConcretoBuilder();

            var evento = _director.Construir(builderAbstracto, DtoValido());

            Assert.IsNotNull(evento);
            Assert.AreEqual("Hackathon", evento.Nombre);
        }

        [TestMethod]
        public void Director_Construir_EventoPublico_NoConservaPassword()
        {
            var dto = DtoValido();
            dto.EsPrivado = false;
            dto.PasswordAcceso = "no-deberia-quedar";

            var evento = _director.Construir(_builder, dto);

            Assert.IsFalse(evento.EsPrivado);
            Assert.IsNull(evento.PasswordAcceso);
        }
    }
}
