using Votify.Modelos.BusinessLogic.Domain;

namespace Votify.Modelos.BusinessLogic.Interfaces
{
    public interface I_Evento_Service : IService<Evento>
    {
        Task<EventoBase> EntrarEvento(string correo, int eventoId);
    }
}
