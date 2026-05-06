using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Enums;

namespace Votify.Tests
{
    [TestClass]
    public class EventoCreacionDtoTests
    {
        private static EventoCreacionDto DtoValido()
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            return new EventoCreacionDto
            {
                Nombre = "Hackathon 2026",
                Descripcion = "Evento de programación competitiva",
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

        // ─── Casos válidos ────────────────────────────────────────────────────────────

        [TestMethod]
        public void Validar_DtoMinimalValido_RetornaEsValidoTrue()
        {
            var (esValido, error) = DtoValido().Validar();

            Assert.IsTrue(esValido);
            Assert.AreEqual("", error);
        }

        [TestMethod]
        public void Validar_DtoConCategoriaYCriteriosValidos_RetornaEsValidoTrue()
        {
            var dto = DtoValido();
            dto.Categorias.Add(new CategoriaDto
            {
                Nombre = "Cat OK",
                PonderacionPopular = 50,
                PonderacionExperto = 50,
                Criterios =
                [
                    new CriterioDto { Nombre = "C1", Ponderacion = 60 },
                    new CriterioDto { Nombre = "C2", Ponderacion = 40 }
                ]
            });

            var (esValido, _) = dto.Validar();

            Assert.IsTrue(esValido);
        }

        // ─── Nombre ───────────────────────────────────────────────────────────────────

        [TestMethod]
        public void Validar_NombreVacio_RetornaError()
        {
            var dto = DtoValido();
            dto.Nombre = "   ";

            var (esValido, error) = dto.Validar();

            Assert.IsFalse(esValido);
            StringAssert.Contains(error, "nombre");
        }

        [TestMethod]
        public void Validar_NombreExactamente30Chars_EsValido()
        {
            var dto = DtoValido();
            dto.Nombre = new string('A', 30);

            var (esValido, _) = dto.Validar();

            Assert.IsTrue(esValido);
        }

        [TestMethod]
        public void Validar_NombreMasde30Chars_RetornaError()
        {
            var dto = DtoValido();
            dto.Nombre = new string('A', 31);

            var (esValido, error) = dto.Validar();

            Assert.IsFalse(esValido);
            StringAssert.Contains(error, "30");
        }

        // ─── Descripción ─────────────────────────────────────────────────────────────

        [TestMethod]
        public void Validar_DescripcionVacia_RetornaError()
        {
            var dto = DtoValido();
            dto.Descripcion = "";

            var (esValido, error) = dto.Validar();

            Assert.IsFalse(esValido);
            StringAssert.Contains(error, "descripción");
        }

        [TestMethod]
        public void Validar_DescripcionExactamente120Chars_EsValida()
        {
            var dto = DtoValido();
            dto.Descripcion = new string('X', 120);

            var (esValido, _) = dto.Validar();

            Assert.IsTrue(esValido);
        }

        [TestMethod]
        public void Validar_DescripcionMasde120Chars_RetornaError()
        {
            var dto = DtoValido();
            dto.Descripcion = new string('X', 121);

            var (esValido, error) = dto.Validar();

            Assert.IsFalse(esValido);
            StringAssert.Contains(error, "120");
        }

        // ─── Fechas nulas ─────────────────────────────────────────────────────────────

        [TestMethod]
        public void Validar_FechaFinInscripcionesNula_RetornaError()
        {
            var dto = DtoValido();
            dto.FechaFinInscripciones = null;

            var (esValido, error) = dto.Validar();

            Assert.IsFalse(esValido);
            StringAssert.Contains(error, "fechas");
        }

        [TestMethod]
        public void Validar_FechaFinEntregaNula_RetornaError()
        {
            var dto = DtoValido();
            dto.FechaFinEntrega = null;

            var (esValido, _) = dto.Validar();

            Assert.IsFalse(esValido);
        }

        [TestMethod]
        public void Validar_FechaFinVotacionesNula_RetornaError()
        {
            var dto = DtoValido();
            dto.FechaFinVotaciones = null;

            var (esValido, _) = dto.Validar();

            Assert.IsFalse(esValido);
        }

        [TestMethod]
        public void Validar_FechaFinExpertaNula_RetornaError()
        {
            var dto = DtoValido();
            dto.FechaFinExperta = null;

            var (esValido, _) = dto.Validar();

            Assert.IsFalse(esValido);
        }

        // ─── Orden de fechas ──────────────────────────────────────────────────────────

        [TestMethod]
        public void Validar_FechaInscripcionesIgualAEntrega_RetornaError()
        {
            var dto = DtoValido();
            var mismaFecha = DateOnly.FromDateTime(DateTime.Today).AddDays(5);
            dto.FechaFinInscripciones = mismaFecha;
            dto.FechaFinEntrega = mismaFecha;

            var (esValido, error) = dto.Validar();

            Assert.IsFalse(esValido);
            StringAssert.Contains(error, "inscripciones");
        }

        [TestMethod]
        public void Validar_FechaEntregaIgualAVotaciones_RetornaError()
        {
            var dto = DtoValido();
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            dto.FechaFinInscripciones = hoy.AddDays(3);
            var mismaFecha = hoy.AddDays(7);
            dto.FechaFinEntrega = mismaFecha;
            dto.FechaFinVotaciones = mismaFecha;

            var (esValido, error) = dto.Validar();

            Assert.IsFalse(esValido);
            StringAssert.Contains(error, "entrega");
        }

        [TestMethod]
        public void Validar_FechaVotacionesIgualAExperta_RetornaError()
        {
            var dto = DtoValido();
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var mismaFecha = hoy.AddDays(12);
            dto.FechaFinVotaciones = mismaFecha;
            dto.FechaFinExperta = mismaFecha;

            var (esValido, error) = dto.Validar();

            Assert.IsFalse(esValido);
            StringAssert.Contains(error, "votaciones");
        }

        [TestMethod]
        public void Validar_FechaInscripcionesEnElPasado_RetornaError()
        {
            var dto = DtoValido();
            dto.FechaFinInscripciones = DateOnly.FromDateTime(DateTime.Today).AddDays(-1);

            var (esValido, error) = dto.Validar();

            Assert.IsFalse(esValido);
            StringAssert.Contains(error, "pasado");
        }

        [TestMethod]
        public void Validar_FechaInscripcionesHoy_EsValida()
        {
            var dto = DtoValido();
            dto.FechaFinInscripciones = DateOnly.FromDateTime(DateTime.Today);

            var (esValido, _) = dto.Validar();

            Assert.IsTrue(esValido);
        }

        // ─── Grupos y participantes ───────────────────────────────────────────────────

        [TestMethod]
        public void Validar_TieneGruposConNumParticipantesCero_RetornaError()
        {
            var dto = DtoValido();
            dto.TieneGrupos = true;
            dto.NumParticipantes = 0;

            var (esValido, error) = dto.Validar();

            Assert.IsFalse(esValido);
            StringAssert.Contains(error, "participantes");
        }

        [TestMethod]
        public void Validar_TieneGruposConNumParticipantesUno_EsValido()
        {
            var dto = DtoValido();
            dto.TieneGrupos = true;
            dto.NumParticipantes = 1;

            var (esValido, _) = dto.Validar();

            Assert.IsTrue(esValido);
        }

        [TestMethod]
        public void Validar_SinGruposConNumParticipantesCero_EsValido()
        {
            var dto = DtoValido();
            dto.TieneGrupos = false;
            dto.NumParticipantes = 0; // no importa si no hay grupos

            var (esValido, _) = dto.Validar();

            Assert.IsTrue(esValido);
        }

        // ─── Tipo de votación ─────────────────────────────────────────────────────────

        [TestMethod]
        public void Validar_VotacionSimpleConNumVotarProyectosCero_RetornaError()
        {
            var dto = DtoValido();
            dto.TipoVotacion = TipoVotacion.Simple;
            dto.NumVotarProyectos = 0;

            var (esValido, error) = dto.Validar();

            Assert.IsFalse(esValido);
            StringAssert.Contains(error, "proyectos");
        }

        [TestMethod]
        public void Validar_VotacionPuntosConPuntosARepartirCero_RetornaError()
        {
            var dto = DtoValido();
            dto.TipoVotacion = TipoVotacion.Puntos;
            dto.PuntosARepartir = 0;

            var (esValido, error) = dto.Validar();

            Assert.IsFalse(esValido);
            StringAssert.Contains(error, "puntos");
        }

        [TestMethod]
        public void Validar_VotacionPuntosIgnoraNumVotarProyectos()
        {
            var dto = DtoValido();
            dto.TipoVotacion = TipoVotacion.Puntos;
            dto.NumVotarProyectos = 0; // no aplica en modo Puntos
            dto.PuntosARepartir = 50;

            var (esValido, _) = dto.Validar();

            Assert.IsTrue(esValido);
        }

        // ─── Categorías y ponderaciones ───────────────────────────────────────────────

        [TestMethod]
        public void Validar_CategoriaConPonderacionNoSuma100_RetornaError()
        {
            var dto = DtoValido();
            dto.Categorias.Add(new CategoriaDto
            {
                Nombre = "Cat Mala",
                PonderacionPopular = 60,
                PonderacionExperto = 30 // 60+30 = 90 ≠ 100
            });

            var (esValido, error) = dto.Validar();

            Assert.IsFalse(esValido);
            StringAssert.Contains(error, "Cat Mala");
        }

        [TestMethod]
        public void Validar_CategoriaConPonderacionSuma100_EsValida()
        {
            var dto = DtoValido();
            dto.Categorias.Add(new CategoriaDto
            {
                Nombre = "Cat Buena",
                PonderacionPopular = 70,
                PonderacionExperto = 30
            });

            var (esValido, _) = dto.Validar();

            Assert.IsTrue(esValido);
        }

        // ─── Criterios ────────────────────────────────────────────────────────────────

        [TestMethod]
        public void Validar_CriteriosNoSuman100_RetornaError()
        {
            var dto = DtoValido();
            dto.Categorias.Add(new CategoriaDto
            {
                Nombre = "Cat Crit",
                PonderacionPopular = 50,
                PonderacionExperto = 50,
                Criterios =
                [
                    new CriterioDto { Nombre = "C1", Ponderacion = 40 },
                    new CriterioDto { Nombre = "C2", Ponderacion = 40 } // 80 ≠ 100
                ]
            });

            var (esValido, error) = dto.Validar();

            Assert.IsFalse(esValido);
            StringAssert.Contains(error, "Cat Crit");
        }

        [TestMethod]
        public void Validar_CriterioPonderacionFueraDeRango_RetornaError()
        {
            var dto = DtoValido();
            dto.Categorias.Add(new CategoriaDto
            {
                Nombre = "Cat Rango",
                PonderacionPopular = 50,
                PonderacionExperto = 50,
                // Suma = 100 pero valores individuales fuera de rango
                Criterios =
                [
                    new CriterioDto { Nombre = "C1", Ponderacion = 110 },
                    new CriterioDto { Nombre = "C2", Ponderacion = -10 }
                ]
            });

            var (esValido, error) = dto.Validar();

            Assert.IsFalse(esValido);
            StringAssert.Contains(error, "Cat Rango");
        }

        [TestMethod]
        public void Validar_ConCriterios_EstableceCriteriosSonPorcentajeTrue()
        {
            var dto = DtoValido();
            var cat = new CategoriaDto
            {
                Nombre = "Cat Flag",
                PonderacionPopular = 50,
                PonderacionExperto = 50,
                CriteriosSonPorcentaje = false, // valor inicial
                Criterios =
                [
                    new CriterioDto { Nombre = "C1", Ponderacion = 50 },
                    new CriterioDto { Nombre = "C2", Ponderacion = 50 }
                ]
            };
            dto.Categorias.Add(cat);

            dto.Validar();

            // La validación establece esta bandera cuando existen criterios
            Assert.IsTrue(cat.CriteriosSonPorcentaje);
        }

        [TestMethod]
        public void Validar_SinCriterios_NoCambiaCriteriosSonPorcentaje()
        {
            var dto = DtoValido();
            var cat = new CategoriaDto
            {
                Nombre = "Cat Sin Criterios",
                PonderacionPopular = 50,
                PonderacionExperto = 50,
                CriteriosSonPorcentaje = false // no debe cambiar
            };
            dto.Categorias.Add(cat);

            dto.Validar();

            Assert.IsFalse(cat.CriteriosSonPorcentaje);
        }
    }
}
