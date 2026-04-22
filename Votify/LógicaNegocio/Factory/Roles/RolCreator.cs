using Votify.LógicaNegocio.Domain;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Factory.Roles
{
    public abstract class RolCreator
    {
        protected readonly IRepository<Rol> _rolRepo;
        protected readonly IRepository<Usuario> _usuarioRepo;
        protected readonly IRepository<Evento> _eventoRepo;

        protected RolCreator(
            IRepository<Rol> rolRepo,
            IRepository<Usuario> usuarioRepo,
            IRepository<Evento> eventoRepo)
        {
            _rolRepo = rolRepo;
            _usuarioRepo = usuarioRepo;
            _eventoRepo = eventoRepo;
        }

        public async Task<AsignacionRol> AsignarAsync(
            string correo, long idEvento, object? datosExtra = null)
        {
            var usuario = (await _usuarioRepo.FindAsync(u => u.Correo == correo)).FirstOrDefault()
                ?? throw new Exception("Usuario no encontrado");

            var evento = await _eventoRepo.GetByIdAsync(idEvento)
                ?? throw new Exception("Evento no encontrado");

            await ValidarAsignacionAsync(usuario, evento, datosExtra);   

            var rolExistente = (await _rolRepo.FindAsync(
                r => r.CorreoUsuario == correo && r.IdEvento == idEvento)).FirstOrDefault();

            var rol = rolExistente ?? new Rol
            {
                CorreoUsuario = correo,
                IdEvento = idEvento,
                CorreoUsuarioNavigation = usuario,
                IdEventoNavigation = evento
            };

            if (rolExistente == null)
                await _rolRepo.AddAsync(rol);
            var asignacion = await CrearAsignacionAsync(rol, usuario, evento, datosExtra);

            await PostAsignacionAsync(asignacion);

            return asignacion;
        }

        protected abstract Task<AsignacionRol> CrearAsignacionAsync(
            Rol rol, Usuario usuario, Evento evento, object? datosExtra);

        protected virtual Task ValidarAsignacionAsync(
            Usuario usuario, Evento evento, object? datosExtra) => Task.CompletedTask;

        protected virtual Task PostAsignacionAsync(AsignacionRol asignacion) => Task.CompletedTask;
    }
}
