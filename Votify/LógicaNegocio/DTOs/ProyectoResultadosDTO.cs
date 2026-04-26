namespace Votify.LógicaNegocio.DTOs
{
    public class ProyectoResultadosDto
    {
        public string NombreProyecto { get; set; } = "";
        public string NombreGrupo { get; set; } = "";

        public int CantidadVotos { get; set; }

        public double PuntuacionMedia { get; set; }

        public List<ResultadoCriterioDto> ResultadosPorCriterio { get; set; } = new();
    }

    public class ResultadoCriterioDto
    {
        public string NombreCriterio { get; set; } = "";
        public long Ponderacion { get; set; }
        public int CantidadVotos { get; set; }
    }
}