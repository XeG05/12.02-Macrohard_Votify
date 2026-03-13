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

        public async  Task<Usuario> login(string email, string password)
        {
            var usuarios = await repository.GetAllAsync();

            return usuarios.FirstOrDefault(u = u => u.email == email && u.password == password);

        }

    }
}
