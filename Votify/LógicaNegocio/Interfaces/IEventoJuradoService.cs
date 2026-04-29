using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Enums;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Interfaces
{
    public interface IEventoJuradoService
    {
        Task<List<CategoriaJuradoDto>> ObtenerCategoriasJurado(string correo, long eventoId);

        Task<String> ObtenerNombreEvento(long eventoId);

        Task<CategoriaJuradoDto> ObtenerCategoria(long eventoId, long categoriaId, string correo);

        Task<int[]> ObtenerProyectosRevisadosPorJurado(string correoJurado, long eventoId, long categoriaId);

        Task<List<ProyectoJuradoViewDto>> ObtenerProyectosCategoriaParaJurado(string correoJurado, long eventoId, long categoriaId);

        Task<List<CriterioComentariosDto>> ObtenerComentariosDeOtrosJueces(string correoJurado, long eventoId, long categoriaId, long idGrupo);

        Task<List<CriterioComentariosDto>> ObtenerComentariosPopulares(long idProyecto);
    }
}
