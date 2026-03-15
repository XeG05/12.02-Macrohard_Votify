using Votify.Modelos.BusinessLogic.Domain;
namespace Votify.Modelos.BusinessLogic.Factory
{
    public class EventoParticipanteCreator : EventoCreator
    {
        public override EventoBase CrearEvento(Evento evento, Usuario usuario)
        {
            return new EventoParticipante(evento, usuario);
        }
    }
}