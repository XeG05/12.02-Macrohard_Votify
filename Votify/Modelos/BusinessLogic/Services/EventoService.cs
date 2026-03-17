using Microsoft.EntityFrameworkCore;
using Votify.Modelos.BusinessLogic.Interfaces;
using Votify.Modelos.Persistence.Interfaces;

namespace Votify.Modelos.BusinessLogic.Services
{
    public class EventoService : Service<Evento>, IEventoService
    {
        private readonly IRepository<Evento> repository;
        public EventoService(IRepository<Evento> repository) : base(repository)
        {
            this.repository = repository;
        }

        public async Task<List<Evento>> ObtenerEventos()
        {
            return await repository.GetAllAsync();
        }
    }
}
