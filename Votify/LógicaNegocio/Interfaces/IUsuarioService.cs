using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Interfaces

{
    public interface IUsuarioService : IService<Usuario>
    {
        Task<Usuario?> Login(string correo, string contraseña);

        Task<Usuario?> Register(string nombre, string correo, string contraseña, string contraseñaRepetida);

        Task<Usuario?> ObtenerPorCorreo(string correo);

        Task<Usuario?> ActualizarPerfil(
            string correo,
            string nombre,
            string? nuevaContraseña,
            string? repetirContraseña,
            Microsoft.AspNetCore.Http.IFormFile? imagen,
            bool eliminarImagen = false
        );
    }
}
