using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Factory.Roles
{
    public class JuradoCreator : RolCreator
    {
        public JuradoCreator(IRepository<Rol> rolRepo) : base(rolRepo) { }

        public override async Task<Rol> CrearAsync(string correo, long idEvento)
        {
            var jurado = new Jurado { CorreoUsuario = correo, IdEvento = idEvento };
            await _rolRepo.AddAsync(jurado);
            return jurado;
        }
    }
}
