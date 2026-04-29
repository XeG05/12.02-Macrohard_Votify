using Votify.LógicaNegocio.DTOs;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Interfaces
{
    public interface IEventoService : IService<Evento>
    {
        Task<EntradaEventoDto> EntrarEvento(string correo, long eventoId, long? grupoId = null);

        Task<Evento> CrearEvento(string correoCreador, EventoCreacionDto dto);

        Task<List<Evento>> GetEventosCreados(string correo);

        Task<List<Evento>> GetEventosParticipacion(string correo);

        Task<List<Evento>> GetEventosDisponibles(string correo);

        Task<Proyecto> RecuperarProyecto(long idProyecto);

        Task<List<Usuario>> ParticipantesGrupo(long idGrupo);

        Task<List<Proyecto>> ListaProyectos(long idEvento);

        Task<Grupo?> GrupoProyecto(long idGrupo);

        Task<List<Categorium>> CategoriasDelEvento(long idEvento);

        Task ComentarProyecto(long idEvento, string correo, long idProyecto, long idCriterio, string comentario);

        Task<List<ComentarioDto>> ObtenerComentariosUsuario(long idProyecto, string correo);

        Task<List<CriterioVotoInfoDto>> ObtenerInfoVotoPorCriterios(long idEvento, long idProyecto, string correo);

        Task VotarProyecto(long idEvento, long idProyecto, long idCriterio, int? valor, string correo);

        Task<List<Evento>> GetEventosJurados(string correo);
    }
}
