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

        Task<List<(Evento Evento, string RolTexto, Grupo? Grupo)>> GetParticipacionesDetalladas(string correo);

        Task<Proyecto> RecuperarProyecto(long idProyecto);

        Task<List<Usuario>> ParticipantesGrupo(long idGrupo);

        Task<List<Proyecto>> ListaProyectos(long idEvento);

        Task<Grupo?> GrupoProyecto(long idGrupo);

        Task<List<Categorium>> CategoriasDelEvento(long idEvento);

        Task ComentarProyecto(long idEvento, string correo, long idProyecto, string comentario);

        Task<Comentario?> ObtenerComentarioUsuario(long idProyecto, string correo);

        Task<(int VotosTotales, bool HaVotado)> ObtenerInfoVotoSimple(long idEvento, long idProyecto, string correo);

        Task VotarProyectoSimple(long idEvento, long idProyecto, string correo);
    }
}
