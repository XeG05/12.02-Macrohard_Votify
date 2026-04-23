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
            var usuario = (await _repository.FindAsync(u => u.Correo == correo)).FirstOrDefault();

            if (usuario == null) { throw new NotFoundException("Ese usuario no existe"); }

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

    }
}
