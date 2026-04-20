using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Interfaces;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Services
{
    public class EventoJuradoService : IEventoJuradoService
    {
        private readonly IRepository<Evento> _eventoRepo;
        private readonly IRepository<Categorium> _categoriaRepo;
        private readonly IRepository<Jurado> _juradoRepo;

        public EventoJuradoService(IRepository<Evento> eventoRepo, IRepository<Categorium> categoriaRepo, IRepository<Jurado> juradoRepo)
        {
            _eventoRepo = eventoRepo;
            _categoriaRepo = categoriaRepo;
            _juradoRepo = juradoRepo;
        }
        public async Task<List<CategoriaDto>> ObtenerCategoriasJurado(string correo, long eventoId)
        {
            // Lógica para obtener las categorías del jurado para el evento especificado
            return await Task.FromResult(new List<CategoriaDto>());
        }
    }
}
