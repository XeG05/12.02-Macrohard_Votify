using Votify.LógicaNegocio.Interfaces;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Services
{
    public class EventoVotanteService : IEventoVotanteService
    {
        private readonly IRepository<Proyecto> _proyectoRepo;
        private readonly IRepository<Participante> _participanteRepo;
        private readonly IRepository<Grupo> _grupoRepo;
        private readonly IRepository<Usuario> _usuarioRepo;
        private readonly IRepository<Categorium> _categoriumRepo;
        public EventoVotanteService(
            IRepository<Proyecto> proyectoRepo,
            IRepository<Participante> participanteRepo,
            IRepository<Grupo> grupoRepo,
            IRepository<Usuario> usuarioRepo,
            IRepository<Categorium> categoriumRepo)
        {
            _proyectoRepo = proyectoRepo;
            _participanteRepo = participanteRepo;
            _grupoRepo = grupoRepo;
            _usuarioRepo = usuarioRepo;
            _categoriumRepo = categoriumRepo;
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
                List<Usuario> usuario = await _usuarioRepo.FindAsync(us => us.Correo == par.CorreoUsuario);
                usuarios.AddRange(usuario);
            }
            return usuarios;
        }

        public async Task<List<Categorium>> CategoriasDelEvento(long idEvento)
        {
            return await _categoriumRepo.FindAsync(c => c.IdEvento == idEvento);
        }
    }
}
