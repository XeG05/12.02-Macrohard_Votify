using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Exceptions;
using Votify.LógicaNegocio.Strategy.Interfaces;
using Votify.LógicaNegocio.Strategy.Strategies;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.Tests.Strategies
{
    [TestClass] // Obligatorio en MSTest para que detecte la clase
    public class VotacionStrategyTests
    {
        // Dependencias comunes simuladas
        private Mock<IRepository<Voto>> _mockVotoRepo;
        private Mock<IRepository<Proyecto>> _mockProyectoRepo;
        private Mock<IRepository<Grupo>> _mockGrupoRepo;
        private Mock<IRepository<Criterio>> _mockCriterioRepo;
        private Mock<IRepository<Rol>> _mockRolRepo;
        private Mock<IUnitOfWork> _mockUow;

        [TestInitialize] // Este método se ejecuta automáticamente ANTES de cada test, dejando todo limpio
        public void Setup()
        {
            _mockVotoRepo = new Mock<IRepository<Voto>>();
            _mockProyectoRepo = new Mock<IRepository<Proyecto>>();
            _mockGrupoRepo = new Mock<IRepository<Grupo>>();
            _mockCriterioRepo = new Mock<IRepository<Criterio>>();
            _mockRolRepo = new Mock<IRepository<Rol>>();
            _mockUow = new Mock<IUnitOfWork>();
        }

        #region =================== TESTS DE VOTACION SIMPLE STRATEGY ===================

        [TestMethod] // En MSTest se usa TestMethod en lugar de Fact
        public async Task SimpleStrategy_Votar_DatosValidos_GuardaVoto()
        {
            var strategy = new VotacionSimpleStrategy(
                _mockVotoRepo.Object, _mockProyectoRepo.Object, _mockGrupoRepo.Object,
                _mockCriterioRepo.Object, _mockRolRepo.Object, _mockUow.Object);

            var evento = new Evento { Id = 1, NumVotarProyectos = 3 };
            _mockProyectoRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new Proyecto { Id = 10, IdGrupo = 50 });
            _mockGrupoRepo.Setup(r => r.GetByIdAsync(50)).ReturnsAsync(new Grupo { Id = 50, IdCategoria = 5 });
            _mockCriterioRepo.Setup(r => r.GetByIdAsync(100)).ReturnsAsync(new Criterio { Id = 100, IdCategoria = 5 });
            _mockVotoRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Voto, bool>>>())).ReturnsAsync(new List<Voto>());
            _mockRolRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Rol, bool>>>()))
                .ReturnsAsync(new List<Rol> { new Rol { CorreoUsuario = "test@test.com", IdEvento = 1 } });

            await strategy.Votar(evento, 10, 100, 1, "test@test.com");

            _mockVotoRepo.Verify(r => r.AddAsync(It.IsAny<Voto>()), Times.Once);
            _mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task SimpleStrategy_ObtenerInfoCriterio_CalculaCorrectamente()
        {
            var strategy = new VotacionSimpleStrategy(
                _mockVotoRepo.Object, _mockProyectoRepo.Object, _mockGrupoRepo.Object,
                _mockCriterioRepo.Object, _mockRolRepo.Object, _mockUow.Object);

            var proyecto = new Proyecto { Id = 10, IdGrupo = 5 };
            var criterio = new Criterio { Id = 100 };
            var evento = new Evento { NumVotarProyectos = 3 };
            var votosUsuario = new List<Voto> { new Voto { IdProyectoGru = 10 } };

            _mockGrupoRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new Grupo { Id = 5, IdCategoria = 1 });
            _mockProyectoRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(proyecto);

            var result = await strategy.ObtenerInfoCriterio(evento, proyecto, criterio, votosUsuario);

            Assert.IsTrue(result.HaVotado); // En MSTest es IsTrue en vez de True
            Assert.AreEqual(2, result.VotosRestantes); // 3 permitidos - 1 usado
        }

        [TestMethod]
        public void SimpleStrategy_CalcularPuntajeTotal_DevuelveConteoPonderado()
        {
            var strategy = new VotacionSimpleStrategy(null, null, null, null, null, null);
            var criterios = new List<Criterio> { new Criterio { Id = 1, Ponderacion = 50 } };
            var votos = new List<Voto> { new Voto { IdProyectoGru = 1, IdCriterio = 1 }, new Voto { IdProyectoGru = 1, IdCriterio = 1 } };

            var result = strategy.CalcularPuntajeTotal(1, criterios, votos);

            Assert.AreEqual(1.0, result); // 2 votos * 0.5 ponderacion
        }

        [TestMethod]
        public void SimpleStrategy_CalcularPuntuacionCriterio_DevuelvePuntuacionEsperada()
        {
            var strategy = new VotacionSimpleStrategy(null, null, null, null, null, null);
            var result = strategy.CalcularPuntuacionCriterio(
                new List<Voto> { new Voto(), new Voto() },
                new List<Voto> { new Voto() },
                1.0, 0.7, 0.3);

            // El 0.1 al final es el 'delta' o margen de error permitido para decimales en MSTest
            Assert.AreEqual(1.7, result.Puntuacion, 0.1);
        }

        [TestMethod]
        public void SimpleStrategy_CalcularValorCrudo_DevuelveCantidad()
        {
            var strategy = new VotacionSimpleStrategy(null, null, null, null, null, null);
            Assert.AreEqual(3.0, strategy.CalcularValorCrudo(new List<Voto> { new Voto(), new Voto(), new Voto() }));
        }

        #endregion

        #region =================== TESTS DE VOTACION NOTA STRATEGY ===================

        [TestMethod]
        public async Task NotaStrategy_Votar_NotaNegativa_LanzaExcepcion()
        {
            var strategy = new VotacionNotaStrategy(
                _mockVotoRepo.Object, _mockProyectoRepo.Object, _mockGrupoRepo.Object,
                _mockCriterioRepo.Object, _mockRolRepo.Object, _mockUow.Object);

            var evento = new Evento { Id = 1 };

            try
            {
                // Intentamos votar con una nota negativa (-5)
                await strategy.Votar(evento, 10, 100, -5, "test@test.com");

                // Si la línea anterior no explota, forzamos el fallo del test
                Assert.Fail("Se esperaba una ValidationException al enviar una nota negativa, pero no se lanzó ninguna.");
            }
            catch (ValidationException)
            {
                // Si entra al catch, la excepción se lanzó correctamente y el test pasa.
            }
        }

        [TestMethod]
        public async Task NotaStrategy_Votar_VotoExistente_ActualizaNota()
        {
            var strategy = new VotacionNotaStrategy(
                _mockVotoRepo.Object, _mockProyectoRepo.Object, _mockGrupoRepo.Object,
                _mockCriterioRepo.Object, _mockRolRepo.Object, _mockUow.Object);

            var votoExistente = new Voto { Id = 99, Nota = 5, IdProyectoGru = 10, IdCriterio = 100 };

            _mockProyectoRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new Proyecto { Id = 10, IdGrupo = 50 });
            _mockGrupoRepo.Setup(r => r.GetByIdAsync(50)).ReturnsAsync(new Grupo { Id = 50, IdCategoria = 5 });
            _mockCriterioRepo.Setup(r => r.GetByIdAsync(100)).ReturnsAsync(new Criterio { Id = 100, IdCategoria = 5 });
            _mockRolRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Rol, bool>>>()))
                .ReturnsAsync(new List<Rol> { new Rol { CorreoUsuario = "test@test.com", IdEvento = 1 } });

            _mockVotoRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Voto, bool>>>()))
                .ReturnsAsync(new List<Voto> { votoExistente });

            await strategy.Votar(new Evento { Id = 1 }, 10, 100, 8, "test@test.com");

            _mockVotoRepo.Verify(r => r.UpdateAsync(It.Is<Voto>(v => v.Nota == 8)), Times.Once);
            _mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task NotaStrategy_ObtenerInfoCriterio_DevuelveNotaDada()
        {
            var strategy = new VotacionNotaStrategy(null, null, null, null, null, null);
            var votosUsuario = new List<Voto> { new Voto { IdProyectoGru = 10, Nota = 7 } };

            var result = await strategy.ObtenerInfoCriterio(new Evento(), new Proyecto { Id = 10 }, new Criterio { Id = 100 }, votosUsuario);

            Assert.IsTrue(result.HaVotado);
            Assert.AreEqual(7, result.NotaDada);
        }

        [TestMethod]
        public void NotaStrategy_CalcularPuntuacionCriterio_CalculaMediaPonderada()
        {
            var strategy = new VotacionNotaStrategy(null, null, null, null, null, null);
            var result = strategy.CalcularPuntuacionCriterio(
                new List<Voto> { new Voto { Nota = 8 }, new Voto { Nota = 6 } },
                new List<Voto> { new Voto { Nota = 10 } },
                1.0, 0.7, 0.3);

            Assert.AreEqual(7.9, result.Puntuacion, 0.1);
            Assert.AreEqual(8.5, result.MediaNota); // (7 + 10) / 2
        }

        [TestMethod]
        public void NotaStrategy_CalcularValorCrudo_DevuelveMediaDeNotas()
        {
            var strategy = new VotacionNotaStrategy(null, null, null, null, null, null);
            var result = strategy.CalcularValorCrudo(new List<Voto> { new Voto { Nota = 10 }, new Voto { Nota = 5 } });
            Assert.AreEqual(7.5, result);
        }

        #endregion

        #region =================== TESTS DE VOTACION PUNTOS STRATEGY ===================

        [TestMethod]
        public async Task PuntosStrategy_Votar_SuperaLimitePuntos_LanzaExcepcion()
        {
            var strategy = new VotacionPuntosStrategy(
                _mockVotoRepo.Object, _mockProyectoRepo.Object, _mockGrupoRepo.Object,
                _mockCriterioRepo.Object, _mockRolRepo.Object, _mockUow.Object);

            var evento = new Evento { Id = 1, PuntosARepartir = 10 }; // Solo 10 puntos en total

            _mockProyectoRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new Proyecto { Id = 10, IdGrupo = 50 });
            _mockGrupoRepo.Setup(r => r.GetByIdAsync(50)).ReturnsAsync(new Grupo { Id = 50, IdCategoria = 5 });
            _mockCriterioRepo.Setup(r => r.GetByIdAsync(100)).ReturnsAsync(new Criterio { Id = 100, IdCategoria = 5 });
            _mockRolRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Rol, bool>>>()))
                .ReturnsAsync(new List<Rol> { new Rol { CorreoUsuario = "test@test.com", IdEvento = 1 } });

            // Simulamos que ya gastó 8 puntos en OTRO proyecto
            _mockVotoRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Voto, bool>>>()))
                .ReturnsAsync(new List<Voto> { new Voto { IdProyectoGru = 99, Nota = 8 } });

            try
            {
                // Intenta gastar 5 puntos más (Total 13 > 10)
                await strategy.Votar(evento, 10, 100, 5, "test@test.com");

                Assert.Fail("Se esperaba una ConflictException por superar los puntos, pero no se lanzó ninguna.");
            }
            catch (ConflictException)
            {
                // El test pasa correctamente al atrapar la excepción esperada
            }
        }

        [TestMethod]
        public async Task PuntosStrategy_ObtenerInfoCriterio_CalculaPuntosRestantes()
        {
            var strategy = new VotacionPuntosStrategy(null, null, null, null, null, null);
            var evento = new Evento { PuntosARepartir = 20 };
            var votosUsuario = new List<Voto>
            {
                new Voto { IdProyectoGru = 10, Nota = 5 },
                new Voto { IdProyectoGru = 11, Nota = 10 }
            };

            var result = await strategy.ObtenerInfoCriterio(evento, new Proyecto { Id = 10 }, new Criterio { Id = 100 }, votosUsuario);

            Assert.IsTrue(result.HaVotado);
            Assert.AreEqual(5, result.PuntosAsignados);
            Assert.AreEqual(5, result.PuntosRestantes); // 20 - 15 gastados = 5 restantes
        }

        [TestMethod]
        public void PuntosStrategy_CalcularPuntuacionCriterio_CalculaSumatoriaPonderada()
        {
            var strategy = new VotacionPuntosStrategy(null, null, null, null, null, null);
            var result = strategy.CalcularPuntuacionCriterio(
                new List<Voto> { new Voto { Nota = 5 }, new Voto { Nota = 3 } },
                new List<Voto> { new Voto { Nota = 10 } },
                1.0, 0.7, 0.3);

            Assert.AreEqual(8.6, result.Puntuacion, 0.1); // (8 * 0.7) + (10 * 0.3)
            Assert.AreEqual(18, result.TotalPuntos);    // 8 + 10
        }

        [TestMethod]
        public void PuntosStrategy_CalcularValorCrudo_DevuelveSumatoria()
        {
            var strategy = new VotacionPuntosStrategy(null, null, null, null, null, null);
            var result = strategy.CalcularValorCrudo(new List<Voto> { new Voto { Nota = 10 }, new Voto { Nota = 10 } });
            Assert.AreEqual(20.0, result);
        }

        #endregion
    }
}