using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Factory.Roles
{
    public class AdministradorCreator : RolCreator
    {
        private readonly IRepository<Administrador> _adminRepo;

        public AdministradorCreator(
            IRepository<Rol> rolRepo, IRepository<Usuario> usuarioRepo,
            IRepository<Evento> eventoRepo, IRepository<Administrador> adminRepo)
            : base(rolRepo, usuarioRepo, eventoRepo)
        {
            _adminRepo = adminRepo;
        }

        protected override async Task ValidarAsignacionAsync(
            Usuario usuario, Evento evento, object? datosExtra)
        {
            var existentes = await _adminRepo.FindAsync(a => a.Id == evento.Id);
            if (existentes.Any())
                throw new Exception("El evento ya tiene administrador.");
        }

        protected override async Task<AsignacionRol> CrearAsignacionAsync(
            Rol rol, Usuario usuario, Evento evento, object? datosExtra)
        {
            var admin = new Administrador
            {
                Correo = usuario.Correo,
                Id = evento.Id,
                Rol = rol
            };
            await _adminRepo.AddAsync(admin);
            return new AsignacionAdministrador(rol, admin);
        }
    }
}
