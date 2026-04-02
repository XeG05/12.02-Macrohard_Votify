using Votify.Modelos.BusinessLogic.Domain;

namespace Votify.Modelos.BusinessLogic.DTOs
{
    public class EntradaEvento
    {
        public EventoBase Evento { get; }
        public string TipoRol { get; }

        public EntradaEvento(EventoBase evento, string tipoRol)
        {
            Evento = evento;
            TipoRol = tipoRol;
        }
    }
}