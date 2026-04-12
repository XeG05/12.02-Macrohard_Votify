using Votify.LógicaNegocio.DTOs;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Interfaces
{
    public interface IEventoService : IService<Evento>
    {
        Task<EntradaEventoDto> EntrarEvento(string correo, long eventoId);

        Task<Evento> CrearEvento(string correoCreador, EventoCreacionDto dto);

        Task<List<Evento>> GetEventosCreados(string correo);

        Task<List<Evento>> GetEventosParticipacion(string correo);

        Task<List<Evento>> GetEventosDisponibles(string correo);

        Task<Proyecto> RecuperarProyecto(long idProyecto);

        Task<List<Usuario>> ParticipantesGrupo(long idGrupo);

        Task ComentarProyecto(long idEvento, string correo, long idProyecto, string comentario);

        Task<Comentario?> ObtenerComentarioUsuario(long idProyecto, string correo);

        Task<(int VotosTotales, bool HaVotado)> ObtenerInfoVotoSimple(long idEvento, long idProyecto, string correo);

        Task VotarProyectoSimple(long idEvento, long idProyecto, string correo);
    }
}
