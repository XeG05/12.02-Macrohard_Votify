using System.Security.Principal;
using Votify.LógicaNegocio.Exceptions;
using Votify.LógicaNegocio.Interfaces;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Services
{
    public class UsuarioService : Service<Usuario>, IUsuarioService
    {
        private readonly IRepository<Usuario> _repository;
        private readonly IUnitOfWork _uow;
        public UsuarioService(IRepository<Usuario> repository, IUnitOfWork uow) : base(repository, uow)
        {
            _repository = repository;
            _uow = uow;
        }

        public async  Task<Usuario?> Login(string correo, string contraseña)
        {
            var usuario = (await _repository.FindAsync(u => u.Correo == correo)).FirstOrDefault()
                ?? throw new NotFoundException("Ese usuario no existe");

            if (!BCrypt.Net.BCrypt.Verify(contraseña, usuario.Contraseña)) { throw new UnauthorizedException("Contraseña incorrecta"); }

            return usuario;

        }

        public async Task<Usuario?> Register(string nombre, string correo, string contraseña, string contraseñaRepetida)
        {
            if(contraseña != contraseñaRepetida) { throw new ValidationException("Las contraseñas no coinciden"); }

            var usuarioExiste = (await _repository.FindAsync(u => u.Correo == correo)).FirstOrDefault();

            if (usuarioExiste != null) { throw new ConflictException("El correo ya está registrado"); }
            var usuario = new Usuario
            {
                Nombre = nombre,
                Correo = correo,
                Contraseña = BCrypt.Net.BCrypt.HashPassword(contraseña)
            };

            await _repository.AddAsync(usuario);
            await _uow.SaveChangesAsync();

            return usuario;

        }
        public async Task<Usuario?> ObtenerPorCorreo(string correo)
        {
            return (await _repository.FindAsync(u => u.Correo == correo)).FirstOrDefault()
                ?? throw new NotFoundException("Usuario no encontrado");
        }

        public async Task<Usuario?> ActualizarPerfil(
            string correo,
            string nombre,
            string? nuevaContraseña,
            string? repetirContraseña,
            Microsoft.AspNetCore.Http.IFormFile? imagen
        )
        {
            var usuario = (await _repository.FindAsync(u => u.Correo == correo)).FirstOrDefault()
                ?? throw new NotFoundException("Usuario no encontrado");

            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ValidationException("El nombre no puede estar vacío");
            }

            usuario.Nombre = nombre.Trim();

            bool quiereCambiarContraseña =
                !string.IsNullOrWhiteSpace(nuevaContraseña) ||
                !string.IsNullOrWhiteSpace(repetirContraseña);

            if (quiereCambiarContraseña)
            {
                if (string.IsNullOrWhiteSpace(nuevaContraseña) ||
                    string.IsNullOrWhiteSpace(repetirContraseña))
                {
                    throw new ValidationException("Debes rellenar los dos campos de contraseña");
                }

                if (nuevaContraseña != repetirContraseña)
                {
                    throw new ValidationException("Las contraseñas no coinciden");
                }

                if (BCrypt.Net.BCrypt.Verify(nuevaContraseña, usuario.Contraseña))
                {
                    throw new ValidationException("La nueva contraseña no puede ser igual a la anterior");
                }

                usuario.Contraseña = BCrypt.Net.BCrypt.HashPassword(nuevaContraseña);
            }

            if (imagen != null)
            {
                var maxSize = 10 * 1024 * 1024; // 10MB limit
                if (imagen.Length > maxSize)
                {
                    throw new ValidationException("La imagen de perfil no puede pesar más de 10MB");
                }

                using var ms = new MemoryStream();
                await imagen.CopyToAsync(ms);
                usuario.Imagen = ms.ToArray();
            }

            await _repository.UpdateAsync(usuario);
            await _uow.SaveChangesAsync();

            return usuario;
        }

    }
}
