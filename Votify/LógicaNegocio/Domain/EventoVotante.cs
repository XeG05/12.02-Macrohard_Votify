using System.Text.RegularExpressions;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Domain
{
    public class EventoVotante : EventoBase
    {
        private readonly IRepository<Proyecto> _proyectoRepo;
        private readonly IRepository<Participante> _participanteRepo;
        private readonly IRepository<Grupo> _grupoRepo;
        private readonly IRepository<Usuario> _usuarioRepo;
        public EventoVotante(Evento evento, Usuario usuario, 
            IRepository<Proyecto> proyectoRepo, 
            IRepository<Participante> participanteRepo, 
            IRepository<Grupo> grupoRepo,
            IRepository<Usuario> usuarioRepo) : base(evento, usuario)
        {
            _proyectoRepo = proyectoRepo;
            _participanteRepo = participanteRepo;
            _grupoRepo = grupoRepo;
            _usuarioRepo = usuarioRepo;
        }

        public async Task<List<Proyecto>> ListaProyectos(long idEvento)
        {
            return await _proyectoRepo.FindAsync(p => p.IdGrupoNavigation.IdCategoriaNavigation.IdEvento == idEvento);
        }

        public async Task<Grupo?> GrupoProyecto(long idGrupo)
        {
            return await _grupoRepo.GetByIdAsync(idGrupo);
        }

        public async Task<List<Usuario>> UsuariosParticipantesGrupo(long idGrupo)
        {
            List<Participante> participantes = await _participanteRepo.FindAsync(par => par.IdGrupo == idGrupo);
            List<Usuario> usuarios = new List<Usuario>();
            foreach (Participante par in participantes)
            {
                List<Usuario> usuario = await _usuarioRepo.FindAsync(us => us.Correo == par.Correo);
                usuarios.AddRange(usuario);
            }
            return usuarios;
        }
    }
}