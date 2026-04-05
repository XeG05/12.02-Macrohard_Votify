using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Interfaces

{
    public interface IUsuarioService : IService<Usuario>
    {
        Task<Usuario?> Login(string correo, string contraseña);

        Task<Usuario?> Register(string nombre, string correo, string contraseña, string contraseñaRepetida);
    }
}
