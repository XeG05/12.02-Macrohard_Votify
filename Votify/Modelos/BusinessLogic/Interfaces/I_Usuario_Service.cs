namespace Votify.Modelos.BusinessLogic.Interfaces



{
    public interface I_Usuario_Service : IService<Usuario>
    {
        Task<Usuario> login(Guid id);
    }
}
