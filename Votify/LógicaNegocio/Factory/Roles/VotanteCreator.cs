using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Factory.Roles
{
    public class VotanteCreator : RolCreator
    {
        public VotanteCreator(IRepository<Rol> rolRepo) : base(rolRepo) { }

        public override async Task<Rol> CrearAsync(string correo, long idEvento)
        {
            var votante = new Votante { CorreoUsuario = correo, IdEvento = idEvento };
            await _rolRepo.AddAsync(votante);
            return votante;
        }
    }
}
