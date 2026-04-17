using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Enums;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Interfaces
{
    public interface IEventoAdministradorService
    {
        Task CambiarNombre(long idEvento, string nuevoNombre);
        Task CambiarDescripcion(long idEvento, string nuevaDescripcion);
        Task<List<Categorium>> VerCategorias(long idEvento);
        Task<List<Grupo>> VerGrupo(long idCategoria);
        Task<List<Proyecto>> VerProyectoDeGrupo(long idGrupo);
        Task<List<Participante>> VerParticipantes(long idGrupo);
        Task<List<Proyecto>> VerProyectosCategoria(long idCategoria);
        Task<int> ContarVotosProyecto(long idProyecto);
        Task<List<ProyectoResumenDTO>> ObtenerResultadosCategoria(long idCategoria);
        Task<int> TotalVotosCategoria(long idCategoria);
        Task AñadirGrupo(string nombreGrupo, long idCategoria, long idEvento);
        Task EliminarGrupo(long idEvento, long idGrupo);
        FasesEvento ObtenerFaseActual(long idEvento);
    }
}
