using Votify.LógicaNegocio.Enums;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Builder
{
    public abstract class EventoBuilder
    {
        public abstract void Reset();

        public abstract EventoBuilder ConIdentificacion(string nombre, string descripcion);

        public abstract EventoBuilder ConFechas(
            DateOnly fechaFinInscripciones,
            DateOnly fechaFinEntrega,
            DateOnly fechaFinVotaciones,
            DateOnly fechaFinEvaluacionExperta,
            DateOnly fechaFinVotacionExperta);

        public abstract EventoBuilder ConComentarios(TipoComentario tipoComentario);

        public abstract EventoBuilder ConVotacion(
            TipoVotacion tipoVotacion,
            bool participantesVotan,
            int numVotarProyectos,
            int puntosARepartir);

        public abstract EventoBuilder ConGrupos(
            bool tieneGrupos,
            bool gruposCompletos,
            int participantesPorGrupo);

        public abstract EventoBuilder ConPrivacidad(bool esPrivado, string? passwordAcceso);

        public abstract Evento GetResultado();
    }
}
