namespace Votify.Modelos.BusinessLogic.Interfaces



{
    public interface I_Usuario_Service : IService<Usuario>
    {
        Task<Usuario?> login(string correo, string contraseña);

        Task<Usuario?> register(string nombre, string correo, string contraseña, string contraseñaRepetida);
    }
}
