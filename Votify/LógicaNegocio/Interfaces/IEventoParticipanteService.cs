using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Enums;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Interfaces
{
    public interface IEventoParticipanteService
    {
        Task<Participante> ObtenerRegistroParticipante(string correo, long idEvento, long? grupoIdActivo = null);
        Task<Grupo?> ObtenerMiGrupo(string correo, long idEvento, long? grupoIdActivo = null);
        Task<List<(string Nombre, string Correo)>> ObtenerMiembrosGrupo(string correo, long idEvento, long? grupoIdActivo = null);
        Task<Categorium?> ObtenerCategoriaGrupo(string correo, long idEvento, long? grupoIdActivo = null);
        Task<List<Categorium>> ObtenerCategoriasEvento(long idEvento);
        Task<Proyecto?> ObtenerMiProyecto(string correo, long idEvento, long? grupoIdActivo);
        Task<bool> TieneProyecto(string correo, long idEvento, long? grupoIdActivo);
        Task CrearProyecto(CrearProyectoRequest request);
        Task EliminarProyecto(string correo, long idEvento, long? grupoIdActivo);
        Task<DashboardParticipanteDto> ObtenerDashboard(long idCategoria, string correo, long idEvento);
        FasesEvento ObtenerFaseActual(long idEvento);
        Task EnviarReclamacion(string correoParticipante, long idEvento, string mensaje);
    }
}
