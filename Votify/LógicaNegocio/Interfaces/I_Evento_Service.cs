using Votify.LógicaNegocio.DTOs;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Interfaces
{
    public interface I_Evento_Service : IService<Evento>
    {
        Task<EntradaEvento> EntrarEvento(string correo, long eventoId);

        Task<Evento> CrearEvento(string correoCreador, EventoCreacionDto dto);

        Task<List<Evento>> GetEventosCreados(string correo);

        Task<List<Evento>> GetEventosParticipacion(string correo);

        Task<List<Evento>> GetEventosDisponibles(string correo);
    }
}
