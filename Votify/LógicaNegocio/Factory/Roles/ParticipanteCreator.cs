using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Factory.Roles
{
    public class ParticipanteCreator : RolCreator
    {
        private readonly IRepository<Participante> _participanteRepo;
        private readonly IRepository<Grupo> _grupoRepo;

        public ParticipanteCreator(
            IRepository<Rol> rolRepo, IRepository<Usuario> usuarioRepo,
            IRepository<Evento> eventoRepo,
            IRepository<Participante> participanteRepo, IRepository<Grupo> grupoRepo)
            : base(rolRepo, usuarioRepo, eventoRepo)
        {
            _participanteRepo = participanteRepo;
            _grupoRepo = grupoRepo;
        }

        protected override async Task ValidarAsignacionAsync(
            Usuario usuario, Evento evento, object? datosExtra)
        {
            if (datosExtra is not long idGrupo)
                throw new Exception("Para crear un participante se requiere idGrupo.");

            var grupo = await _grupoRepo.GetByIdAsync(idGrupo)
                ?? throw new Exception("Grupo no encontrado.");

            var categoria = await _grupoRepo.FindAsync(g => g.Id == idGrupo);
        }

        protected override async Task<AsignacionRol> CrearAsignacionAsync(
            Rol rol, Usuario usuario, Evento evento, object? datosExtra)
        {
            var idGrupo = (long)datosExtra!;
            var participante = new Participante
            {
                Correo = usuario.Correo,
                Id = evento.Id,
                IdGrupo = idGrupo
            };
            await _participanteRepo.AddAsync(participante);
            return new AsignacionParticipante(rol, participante);
        }
    }
}
