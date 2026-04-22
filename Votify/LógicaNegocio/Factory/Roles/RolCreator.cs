using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Factory.Roles
{
    public abstract class RolCreator
    {
        protected readonly IRepository<Rol> _rolRepo;

        protected RolCreator(
            IRepository<Rol> rolRepo)
        {
            _rolRepo = rolRepo;
        }

        public abstract Task<Rol> CrearAsync(string correo, long idEvento);
    }
}
