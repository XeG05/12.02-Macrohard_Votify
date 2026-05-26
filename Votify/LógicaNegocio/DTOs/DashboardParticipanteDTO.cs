namespace Votify.LógicaNegocio.DTOs
{
    public class DashboardParticipanteDto
    {
        public string NombreProyecto { get; set; } = "";
        public string NombreGrupo { get; set; } = "";
        public long GrupoId { get; set; }

        public int TotalVotos { get; set; }
        public int Posicion { get; set; }

        public List<ComentarioDto> Comentarios { get; set; } = new();
        public List<CriterioComentariosDto> SintesisJuradosPorCriterio { get; set; } = new();
    }
}
