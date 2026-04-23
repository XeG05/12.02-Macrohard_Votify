using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Enums;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Interfaces
{
    public interface IEventoJuradoService
    {
        Task<List<CategoriaDto>> ObtenerCategoriasJurado(string correo, long eventoId);


    }
}
