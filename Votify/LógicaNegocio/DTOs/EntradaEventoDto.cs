using Votify.LógicaNegocio.Domain;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.DTOs
{
    public class EntradaEventoDto
    {
        public Evento Evento { get; }
        public Rol TipoRol { get; }

        public EntradaEventoDto(Evento evento, Rol tipoRol)
        {
            Evento = evento;
            TipoRol = tipoRol;
        }
    }
}