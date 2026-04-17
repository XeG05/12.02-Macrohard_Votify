using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Interfaces
{
    public interface IEventoVotanteService
    {
        Task<List<Proyecto>> ListaProyectos(long idEvento);
        Task<Grupo?> GrupoProyecto(long idGrupo);
        Task<List<Usuario>> UsuariosParticipantesGrupo(long idGrupo);
        Task<List<Categorium>> CategoriasDelEvento(long idEvento);
    }
}
