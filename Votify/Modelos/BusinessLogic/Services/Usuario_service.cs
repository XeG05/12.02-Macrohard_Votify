using System.Security.Principal;
using Votify.Modelos.BusinessLogic.Interfaces;
using Votify.Modelos.Persistence.Interfaces;

namespace Votify.Modelos.BusinessLogic.Services
{
    public class Usuario_service : Service<Usuario>, I_Usuario_Service
    {
        private readonly IRepository<Usuario> repository;
        public Usuario_service(IRepository<Usuario> repository) : base(repository)
        {
            this.repository = repository;
        }

        public async  Task<Usuario?> Login(string correo, string contraseña)
        {
            var usuarios = await repository.GetAllAsync();

            var usuario = usuarios.FirstOrDefault(u => u.Correo == correo);

            if(usuario == null) { throw new Exception("Ese usuario no existe"); }

            if(usuario.Contraseña != contraseña){ throw new Exception("Contraseña incorrecta"); }

            return usuario;

        }

        public async Task<Usuario?> Register(string nombre, string correo, string contraseña, string contraseñaRepetida)
        {
            if(contraseña != contraseñaRepetida) { throw new Exception("Las contraseñas no coinciden"); }

            var usuarios = await repository.GetAllAsync();

            if (usuarios.Any(u => u.Correo == correo)) { throw new Exception("El correo ya está registrado"); }

            var usuario = new Usuario
            {
                Nombre = nombre,
                Correo = correo,
                Contraseña = contraseña
            };

            await repository.AddAsync(usuario);

            return usuario;

        }

    }
}
