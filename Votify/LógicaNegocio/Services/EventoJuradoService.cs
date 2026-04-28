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

            var resultado = new List<CategoriaJuradoDto>();
            foreach(var c in categoriasAsignadas)
            {
                var datosProyectos = await ObtenerProyectosRevisadosPorJurado(correo, eventoId, c.Id);
                resultado.Add(new CategoriaJuradoDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion,
                    ProyectosRevisados = datosProyectos[0],
                    ProyectosTotales = datosProyectos[1]
                });
            }

            return resultado;
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

        var datosProyectos = await ObtenerProyectosRevisadosPorJurado(correo, eventoId, categoriaId);
            return new CategoriaJuradoDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                ProyectosRevisados = datosProyectos[0],
                ProyectosTotales = datosProyectos[1]
            };
        }

        public async Task<int[]> ObtenerProyectosRevisadosPorJurado(string correoJurado, long eventoId, long categoriaId)
        {
            var jurado = await _juradoRepo.FindAsync(j => j.CorreoUsuario == correoJurado && j.IdEvento == eventoId);
            var juradoEncontrado = jurado.FirstOrDefault();

            if (juradoEncontrado == null) return new int[] { 0, 0 };

            var comentarios = await _comentarioRepo.AsQueryable()
                                            .Where(c => c.IdRol == eventoId && 
                                                        c.CorreoRol == correoJurado &&
                                                        c.Proyecto != null && 
                                                        c.Proyecto.IdGrupoNavigation != null &&
                                                        c.Proyecto.IdGrupoNavigation.IdCategoria == categoriaId)
                                            .Select(c => c.IdProyectoGru)
                                            .Distinct()
                                            .ToListAsync();

            var proyectosTotales = await _categoriaRepo.AsQueryable()
                                            .Where(x => x.Id == categoriaId && x.IdEvento == eventoId)
                                            .SelectMany(x => x.Grupos)
                                            .Where(g => g.Proyecto != null)
                                            .CountAsync();

            return new int[] { comentarios.Count, proyectosTotales };
        }

        public async Task<List<ProyectoJuradoViewDto>> ObtenerProyectosCategoriaParaJurado(string correoJurado, long eventoId, long categoriaId)
        {
            var jurado = await _juradoRepo.FindAsync(j => j.CorreoUsuario == correoJurado && j.IdEvento == eventoId);
            var juradoEncontrado = jurado.FirstOrDefault();

            if (juradoEncontrado == null) return new List<ProyectoJuradoViewDto>();

            var grupos = await _categoriaRepo.AsQueryable()
                                            .Where(c => c.Id == categoriaId && c.IdEvento == eventoId)
                                            .Include(c => c.Grupos)
                                                .ThenInclude(g => g.Proyecto)
                                            .Include(c => c.Grupos)
                                                .ThenInclude(g => g.Participantes)
                                                    .ThenInclude(p => p.CorreoUsuarioNavigation)
                                            .SelectMany(c => c.Grupos)
                                            .Where(g => g.Proyecto != null)
                                            .ToListAsync();

            var comentariosJuradoIdsProyectos = await _comentarioRepo.AsQueryable()
                                            .Where(c => c.IdRol == eventoId && 
                                                        c.CorreoRol == correoJurado &&
                                                        c.Proyecto != null && 
                                                        c.Proyecto.IdGrupoNavigation != null &&
                                                        c.Proyecto.IdGrupoNavigation.IdCategoria == categoriaId)
                                            .Select(c => c.IdProyectoGru)
                                            .Distinct()
                                            .ToListAsync();

            var resultado = new List<ProyectoJuradoViewDto>();
            foreach (var g in grupos)
            {
                var p = g.Proyecto;
                resultado.Add(new ProyectoJuradoViewDto
                {
                    Id = p!.Id,
                    Nombre = p.Nombre,
                    DescripcionBreve = p.DescripcionBreve,
                    IdGrupo = p.IdGrupo,
                    Revisado = comentariosJuradoIdsProyectos.Contains(p.Id),
                    ParticipantesNombres = g.Participantes.Select(part => part.CorreoUsuarioNavigation.Nombre).ToList()
                });
            }

            return resultado;
        }
    }
}
