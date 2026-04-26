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
        private readonly IRepository<Comentario> _comentarioRepo;

        public EventoJuradoService(IRepository<Evento> eventoRepo, IRepository<Categorium> categoriaRepo, IRepository<Jurado> juradoRepo, IRepository<Comentario> comentarioRepo)
        {
            _eventoRepo = eventoRepo;
            _categoriaRepo = categoriaRepo;
            _juradoRepo = juradoRepo;
            _comentarioRepo = comentarioRepo;
        }
        
        public async Task<List<CategoriaJuradoDto>> ObtenerCategoriasJurado(string correo, long eventoId)
        {
            var jurados = await _juradoRepo.FindAsync(j => j.CorreoUsuario == correo && j.IdEvento == eventoId);
            if (!jurados.Any()) return new List<CategoriaJuradoDto>();

            var categorias = await _categoriaRepo.AsQueryable()
                                            .Where(c => c.IdEvento == eventoId)
                                            .Include(c => c.Jurados)
                                            .ToListAsync();

            var categoriasAsignadas = categorias.Where(c => c.Jurados != null && c.Jurados.Any(j => j.CorreoUsuario == correo)).ToList();
            
            return categoriasAsignadas.Select(c => new CategoriaJuradoDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                ProyectosRevisados = 0,
                ProyectosTotales = 0
            }).ToList();
        }

        public async Task<String> ObtenerNombreEvento(long eventoId)
        {
            var evento = await _eventoRepo.FindAsync(e => e.Id == eventoId);
            if (!evento.Any()) return "";
            return evento.First().Nombre;
        }

        public async Task<CategoriaJuradoDto> ObtenerCategoria(long eventoId, long categoriaId, string correo)
        {
            var categoria = await _categoriaRepo.FindAsync(c => c.Id == categoriaId && c.IdEvento == eventoId);
            if (!categoria.Any()) return null;
            var c = categoria.First();
            var proyectosCategoria = await _categoriaRepo.AsQueryable()
                                        .Where(x => x.Id == categoriaId && x.IdEvento == eventoId)
                                        .Include(x => x.Grupos)
                                        .ThenInclude(g => g.Proyecto)
                                        .SelectMany(x => x.Grupos.Select(g => g.Proyecto))
                                        .Where(p => p != null)
                                        .ToListAsync();

        var comentariosJurado = await _comentarioRepo.AsQueryable()
                                        .Where(cm => cm.CorreoRol == correo && cm.Proyecto.IdGrupoNavigation.IdCategoria == categoriaId)
                                        .ToListAsync(); 
            return new CategoriaJuradoDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                ProyectosRevisados = comentariosJurado.Count(),
                ProyectosTotales = proyectosCategoria.Count
            };
        }

    }
}
