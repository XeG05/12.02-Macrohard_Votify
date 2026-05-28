using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Votify.LógicaNegocio.Templates.EmailTemplates;

namespace Votify.Tests
{
    [TestClass]
    public class PlantillaTests
    {
        private IConfiguration _config = null!;

        [TestInitialize]
        public void Setup()
        {
            var inMemorySettings = new Dictionary<string, string?>
            {
                {"Email:Host", "smtp.test.com"},
                {"Email:Port", "587"},
                {"Email:Usuario", "test@test.com"},
                {"Email:Contraseña", "1234"}
            };

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        [TestMethod]
        public void InvitacionParticipante_GeneraAsuntoCorrecto()
        {
            var plantilla = new InvitacionParticipanteEmail(_config);

            var asunto = plantilla.GenerarAsunto("Hackathon IA");

            Assert.AreEqual(
                "Invitación al evento: Hackathon IA",
                asunto);
        }

        [TestMethod]
        public void InvitacionParticipante_GeneraBodyCorrecto()
        {
            var plantilla = new InvitacionParticipanteEmail(_config);

            var body = plantilla.GenerarBody(
                "Hackathon IA",
                "Pablo",
                "http://test.com");

            Assert.IsTrue(body.Contains("Pablo"));
            Assert.IsTrue(body.Contains("Hackathon IA"));
            Assert.IsTrue(body.Contains("http://test.com"));
        }

        [TestMethod]
        public void InvitacionJurado_GeneraAsuntoCorrecto()
        {
            var plantilla = new InvitacionJuradoEmail(
                _config,
                "Inteligencia Artificial");

            var asunto = plantilla.GenerarAsunto("Hackathon IA");

            Assert.AreEqual(
                "Invitación como jurado: Hackathon IA",
                asunto);
        }

        [TestMethod]
        public void InvitacionJurado_GeneraBodyConCategoria()
        {
            var plantilla = new InvitacionJuradoEmail(
                _config,
                "Ciberseguridad");

            var body = plantilla.GenerarBody(
                "Hackathon IA",
                "Laura",
                "http://test.com");

            Assert.IsTrue(body.Contains("Laura"));
            Assert.IsTrue(body.Contains("Hackathon IA"));
            Assert.IsTrue(body.Contains("Ciberseguridad"));
            Assert.IsTrue(body.Contains("http://test.com"));
        }

        [TestMethod]
        public void Plantillas_HeredanDeInvitacionEmailTemplate()
        {
            var participante =
                new InvitacionParticipanteEmail(_config);

            var jurado =
                new InvitacionJuradoEmail(
                    _config,
                    "IA");

            Assert.IsInstanceOfType(
                participante,
                typeof(InvitacionEmailTemplate));

            Assert.IsInstanceOfType(
                jurado,
                typeof(InvitacionEmailTemplate));
        }

    }
}
