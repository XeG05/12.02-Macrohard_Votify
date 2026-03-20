namespace Votify.Modelos.BusinessLogic.Domain
{
    public abstract class EventoBase
    {
        protected Evento evento;
        protected Usuario usuario;

        public EventoBase(Evento evento, Usuario usuario)
        {
            this.evento = evento;
            this.usuario = usuario;
        }

        public string Nombre => evento.Nombre;
        public string Descripcion => evento.Descripcion;

        public DateOnly FechaFinInscripciones => evento.FechaFinInscripciones;
        public DateOnly FechaFinEntrega => evento.FechaFinEntrega;
        public DateOnly FechaFinVotaciones => evento.FechaFinVotaciones;

        public bool TieneGrupos => evento.TieneGrupos;
        public bool GruposCompletos => evento.GruposCompletos;
        public int ParticipantesPorGrupo => evento.ParticipantesPorGrupo;
    }
}
