using Votify.Auth;
using Votify.Modelos.BusinessLogic.Domain;
using Votify.Modelos.BusinessLogic.DTOs;

namespace Votify.Modelos.BusinessLogic.Interfaces
{
    public interface I_Evento_Service : IService<Evento>
    {
        Task<EntradaEvento> EntrarEvento(string correo, long eventoId);

        Task<Evento> CrearEvento(string correoCreador, EventoCreacionDto dto);
    }
}
