using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.Persistencia.Repositories
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(VotifyContext context) : base(context)
        {
        }
    }
}
