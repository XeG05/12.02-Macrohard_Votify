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
    }
}
