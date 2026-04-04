using Votify.LógicaNegocio.Domain;

namespace Votify.LógicaNegocio.DTOs
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