using Votify.LógicaNegocio.Domain;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.DTOs
{
    public class EntradaEventoDto
    {
        public EventoBase Evento { get; }
        public Rol TipoRol { get; }

        public EntradaEventoDto(EventoBase evento, Rol tipoRol)
        {
            Evento = evento;
            TipoRol = tipoRol;
        }
    }
}