using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Interfaces



{
    public interface I_Usuario_Service : IService<Usuario>
    {
        Task<Usuario?> Login(string correo, string contraseña);

        Task<Usuario?> Register(string nombre, string correo, string contraseña, string contraseñaRepetida);
    }
}
