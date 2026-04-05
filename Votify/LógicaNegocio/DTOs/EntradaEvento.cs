using Votify.LógicaNegocio.Domain;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.DTOs
{
    public class EntradaEvento
    {
        public EventoBase Evento { get; }
        public Rol TipoRol { get; }

        public EntradaEvento(EventoBase evento, Rol tipoRol)
        {
            Evento = evento;
            TipoRol = tipoRol;
        }
    }
}