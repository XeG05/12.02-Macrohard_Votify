using System.Security.Principal;
using Votify.Modelos.BusinessLogic.Interfaces;
using Votify.Modelos.Persistence.Interfaces;

namespace Votify.Modelos.BusinessLogic.Services
{
    public class Usuario_service : Service<Usuario>, I_Usuario_Service
    {
        public Usuario_service(IRepository<Usuario>repository) : base(repository) { }

        public async  Task<Usuario> login(Guid id)
        {
            return await _repository.GetByIdAsync(id);

        }

    }
}
