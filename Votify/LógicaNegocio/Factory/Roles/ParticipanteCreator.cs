using Votify.LógicaNegocio.Domain;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Factory.Roles
{
    public class ParticipanteCreator : RolCreator
    {
        public ParticipanteCreator(IRepository<Rol> rolRepo) : base(rolRepo) { }

        public override Task<Rol> CrearAsync(string correo, long idEvento)
            => throw new NotSupportedException("Usa CrearAsync con idGrupo.");

        public async Task<Rol> CrearAsync(string correo, long idEvento, long idGrupo)
        {
            var participante = new Participante
            {
                CorreoUsuario = correo,
                IdEvento = idEvento,
                IdGrupo = idGrupo
            };
            await _rolRepo.AddAsync(participante);
            return participante;
        }
    }
}
