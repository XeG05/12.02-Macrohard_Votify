using Votify.Modelos.BusinessLogic.Interfaces;
using Votify.Modelos.Persistence.Interfaces;

namespace Votify.Modelos.BusinessLogic.Services
{
    public class UsuarioService : Service<Usuario>, IUsuarioService
    {
        public UsuarioService(IRepository<Usuario> repository) : base(repository) { }
        public async Task<Usuario> Login(long Id)
        {
            return await _repository.GetByIdAsync(Id) ?? throw new NotImplementedException();
        }
    }
}
