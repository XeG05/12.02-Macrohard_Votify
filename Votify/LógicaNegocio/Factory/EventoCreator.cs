using Votify.LógicaNegocio.Domain;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Factory

{
    public abstract class EventoCreator
    {
        public abstract EventoBase CrearEvento(Evento evento, Usuario usuario);
    }
}
