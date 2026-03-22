using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Votify.Modelos;
using Votify.Modelos.BusinessLogic.Services;
using Votify.Modelos.Persistence.Repositories;

namespace Votify.Tests
{
    public abstract class UsuarioServiceTestsBase
    {
        protected VotifyContext Context = null!;
        protected Usuario_service UsuarioService = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<VotifyContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            Context = new VotifyContext(options);

            var usuarioRepository = new Repository<Usuario>(Context);
            UsuarioService = new Usuario_service(usuarioRepository);
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await Context.Database.EnsureDeletedAsync();
            await Context.DisposeAsync();
        }

        protected async Task CrearUsuarioAsync(
            string nombre = "Pablo",
            string correo = "pablo@gmail.com",
            string contraseña = "12345")
        {
            Context.Usuarios.Add(new Usuario
            {
                Nombre = nombre,
                Correo = correo,
                Contraseña = contraseña
            });

            await Context.SaveChangesAsync();
        }
    }
}