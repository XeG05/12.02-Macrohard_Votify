using System.Security.Principal;
using Votify.LógicaNegocio.Interfaces;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Services
{
    public class UsuarioService : Service<Usuario>, IUsuarioService
    {
        private readonly IRepository<Usuario> repository;
        public UsuarioService(IRepository<Usuario> repository) : base(repository)
        {
            this.repository = repository;
        }

        public async  Task<Usuario?> Login(string correo, string contraseña)
        {
            var usuario = (await repository.FindAsync(u => u.Correo == correo)).FirstOrDefault();

            if (usuario == null) { throw new Exception("Ese usuario no existe"); }

            if (!BCrypt.Net.BCrypt.Verify(contraseña, usuario.Contraseña)) { throw new Exception("Contraseña incorrecta"); }

            return usuario;

        }

        public async Task<Usuario?> Register(string nombre, string correo, string contraseña, string contraseñaRepetida)
        {
            if(contraseña != contraseñaRepetida) { throw new Exception("Las contraseñas no coinciden"); }

            var usuarioExiste = (await repository.FindAsync(u => u.Correo == correo)).FirstOrDefault();

            if (usuarioExiste != null) { throw new Exception("El correo ya está registrado"); }

            var usuario = new Usuario
            {
                Nombre = nombre,
                Correo = correo,
                Contraseña = BCrypt.Net.BCrypt.HashPassword(contraseña)
            };

            await repository.AddAsync(usuario);

            return usuario;

        }

    }
}
