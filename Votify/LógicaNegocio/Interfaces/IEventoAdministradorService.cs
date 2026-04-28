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
        Task<List<MiembroGrupoDto>> VerParticipantes(long idGrupo);
        Task<List<Proyecto>> VerProyectosCategoria(long idCategoria);
        Task<int> ContarVotosProyecto(long idProyecto);
        Task<List<ProyectoResultadosDto>> ObtenerResultadosCategoria(long idCategoria);
        Task<int> TotalVotosCategoria(long idCategoria);
        Task AñadirGrupo(string nombreGrupo, long idCategoria);
        Task EliminarGrupo(long idGrupo);
        Task ActualizarGrupo(long grupoId, GrupoDto dto);
        FasesEvento ObtenerFaseActual(long idEvento);
        Task ActualizarFechas(long eventoId, DateTime finInscripciones, DateTime finEntrega, DateTime finVotaciones, DateTime finExperta);
        Task PausarEvento(long eventoId);
        Task ReanudarEvento(long eventoId);
        Task CerrarEvento(long eventoId);
        Task ActualizarParticipantesGrupo(long eventoId, GrupoDto dto);
        Task AceptarInvitacion(string token, string correoUsuario);
        Task<int> ContarInvitacionesPendientes(long idGrupo);

    }
}
