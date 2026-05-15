namespace Votify.LógicaNegocio.DTOs
{
    public class ProyectoResultadosDto
    {
        public long IdProyecto { get; set; } 
        public string NombreProyecto { get; set; } = "";
        public string NombreGrupo { get; set; } = "";

        public int CantidadVotos { get; set; }

        public double PuntuacionMedia { get; set; }

        public List<ResultadoCriterioDto> ResultadosPorCriterio { get; set; } = new();

        public List<VotoDetalleDto> VotosDetalle { get; set; } = new();


    }

    public class ResultadoCriterioDto
    {
        public string NombreCriterio { get; set; } = "";
        public long Ponderacion { get; set; }
        public int CantidadVotos { get; set; }
        public double? MediaNota { get; set; }
        public double? TotalPuntos { get; set; }
    }

    public class VotoDetalleDto
    {
        public string CorreoUsuario { get; set; } = "";

        public string NombreCriterio { get; set; } = "";

        public double Nota { get; set; }
    }
}