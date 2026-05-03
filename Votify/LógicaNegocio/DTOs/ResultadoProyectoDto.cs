namespace Votify.LógicaNegocio.DTOs
{
    public class ResultadoProyectoDto
    {
        public int TotalVotos { get; set; }

        public double ResultadoVotos { get; set; }
        public int PosicionRanking { get; set; }

        public Dictionary<string, double> VotosPorCriterio { get; set; } = new Dictionary<string, double>();
    }
}
