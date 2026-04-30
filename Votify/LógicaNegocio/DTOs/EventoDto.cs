using Votify.LógicaNegocio.Enums;

namespace Votify.LógicaNegocio.DTOs
{
    public class EventoDto
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateOnly FechaFinInscripciones { get; set; }
        public DateOnly FechaFinEntrega { get; set; }
        public DateOnly FechaFinVotaciones { get; set; }
        public DateOnly FechaFinEvaluacionExperta { get; set; }
        public DateOnly FechaFinVotExperta { get; set; }
        public bool TieneGrupos { get; set; }
        public bool GruposCompletos { get; set; }
        public int ParticipantesPorGrupo { get; set; }

        public string Estado { get; set; } = "Activo";
        public TipoComentario TipoComentario { get; set; }
        public TipoVotacion TipoVotacion { get; set; }
        public int NumVotarProyectos { get; set; }
        public int PuntosARepartir { get; set; }
    }
}
