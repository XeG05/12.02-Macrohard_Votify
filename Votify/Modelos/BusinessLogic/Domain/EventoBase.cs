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
    }
}
