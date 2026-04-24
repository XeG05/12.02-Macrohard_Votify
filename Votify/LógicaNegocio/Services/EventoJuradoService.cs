using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Interfaces;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;
using Microsoft.EntityFrameworkCore;

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
        
        public async Task<List<CategoriaJuradoDto>> ObtenerCategoriasJurado(string correo, long eventoId)
        {
            var jurados = await _juradoRepo.FindAsync(j => j.CorreoUsuario == correo && j.IdEvento == eventoId);
            if (!jurados.Any()) return new List<CategoriaJuradoDto>();

            var categorias = await _categoriaRepo.AsQueryable()
                                            .Where(c => c.IdEvento == eventoId)
                                            // .Include(c => c.Jurados) // Comentado temporalmente para evitar el error 42P01 de la tabla CategoriumJurado inexistente
                                            .ToListAsync();

            var categoriasAsignadas = categorias.Where(c => c.Jurados != null && c.Jurados.Any(j => j.CorreoUsuario == correo)).ToList();
            
            // Si el mapeo de Jurados (N a M) todavia falla a novel base de datos, temporalmente daremos todas las categorias 
            // del evento como workaround hasta que se arreglen las DB Migrations y la tabla CategoriumJurado se cree con exito:
            if (!categoriasAsignadas.Any())
            {
               categoriasAsignadas = categorias.ToList();
            }
            
            return categoriasAsignadas.Select(c => new CategoriaJuradoDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                ProyectosRevisados = 0,
                ProyectosTotales = 0
            }).ToList();
        }
    }
}
