using Votify.LógicaNegocio.Enums;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Domain
{
    public abstract class EventoBase : IEvento
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

        public FasesEvento ObtenerFaseActual()
        {
            var hoy = DateOnly.FromDateTime(DateTime.Now);

            if (hoy <= evento.FechaFinInscripciones)
                return FasesEvento.Inscripciones;

            if (hoy <= evento.FechaFinEntrega)
                return FasesEvento.Entregas;

            if (hoy <= evento.FechaFinVotaciones)
                return FasesEvento.Votaciones;

            return FasesEvento.Resultados;
        }
    }
}
