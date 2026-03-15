using Votify.Modelos.BusinessLogic;
using Votify.Modelos.BusinessLogic.Domain;


namespace Votify.Modelos.BusinessLogic.Factory

{
    public abstract class EventoCreator
    {
        public abstract EventoBase CrearEvento(Evento evento, Usuario usuario);
    }
}
