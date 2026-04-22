using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Factory.Roles
{
    public class AdministradorCreator : RolCreator
    {
        public AdministradorCreator(IRepository<Rol> rolRepo) : base(rolRepo) { }

        public override async Task<Rol> CrearAsync(string correo, long idEvento)
        {
            var admin = new Administrador { CorreoUsuario = correo, IdEvento = idEvento };
            await _rolRepo.AddAsync(admin);
            return admin;
        }
    }
}
