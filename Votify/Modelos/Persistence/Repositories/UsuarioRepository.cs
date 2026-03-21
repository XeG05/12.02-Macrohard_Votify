using Votify.Modelos.Persistence.Interfaces;

namespace Votify.Modelos.Persistence.Repositories
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(VotifyContext context) : base(context)
        {
        }
    }
}
