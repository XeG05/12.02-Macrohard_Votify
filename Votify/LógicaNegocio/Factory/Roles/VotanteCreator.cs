using Votify.LógicaNegocio.Domain;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Factory.Roles
{
    public class VotanteCreator : RolCreator
    {
        private readonly IRepository<Votante> _votanteRepo;

        public VotanteCreator(
            IRepository<Rol> rolRepo, IRepository<Usuario> usuarioRepo,
            IRepository<Evento> eventoRepo, IRepository<Votante> votanteRepo)
            : base(rolRepo, usuarioRepo, eventoRepo)
        {
            _votanteRepo = votanteRepo;
        }

        protected override async Task<AsignacionRol> CrearAsignacionAsync(
            Rol rol, Usuario usuario, Evento evento, object? datosExtra)
        {
            var votante = new Votante
            {
                Correo = usuario.Correo,
                Id = evento.Id,
                Rol = rol
            };
            await _votanteRepo.AddAsync(votante);
            return new AsignacionVotante(rol, votante);
        }
    }
}
