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
        Task CrearProyecto(string nombreProyecto, string descripcionBreve, string descripcionCompleta, byte[] archivo, List<byte[]> imagenes, string correo, long idEvento, long? grupoIdActivo);
        Task EliminarProyecto(string correo, long idEvento, long? grupoIdActivo);
        Task<DashboardParticipanteDTO> ObtenerDashboard(long idCategoria, string correo, long idEvento);
        FasesEvento ObtenerFaseActual(long idEvento);
    }
}
