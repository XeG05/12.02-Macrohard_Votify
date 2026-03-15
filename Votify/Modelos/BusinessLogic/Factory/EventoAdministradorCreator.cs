using Votify.Modelos.BusinessLogic.Domain;
namespace Votify.Modelos.BusinessLogic.Factory
{
    public class EventoAdministradorCreator : EventoCreator
    {
        public override EventoBase CrearEvento(Evento evento, Usuario usuario)
        {
            return new EventoAdministrador(evento, usuario);
        }
    }
}
