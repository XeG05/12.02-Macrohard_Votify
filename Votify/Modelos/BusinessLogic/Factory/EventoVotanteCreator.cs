using Votify.Modelos.BusinessLogic.Domain;
namespace Votify.Modelos.BusinessLogic.Factory
{
    public class EventoVotanteCreator : EventoCreator
    {
        public override EventoBase CrearEvento(Evento evento, Usuario usuario)
        {
            return new EventoVotante(evento, usuario);
        }
    }
}