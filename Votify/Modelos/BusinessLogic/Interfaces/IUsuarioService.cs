namespace Votify.Modelos.BusinessLogic.Interfaces
{
    public interface IUsuarioService : IService<Usuario>
    {
        Task<Usuario> Login(long Id);
    }
}
