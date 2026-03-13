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

        public async  Task<Usuario?> login(string correo, string contraseña)
        {
            var usuarios = await repository.GetAllAsync();

            var usuario = usuarios.FirstOrDefault(u => u.Correo == correo);

            if(usuario == null) { return null; }

            if(usuario.Contraseña != contraseña){ return null; }

            return usuario;

        }

    }
}
