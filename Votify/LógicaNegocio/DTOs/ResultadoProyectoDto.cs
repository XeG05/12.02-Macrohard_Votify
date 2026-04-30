namespace Votify.LógicaNegocio.DTOs
{
    public class ResultadoProyectoDto
    {
        public int TotalVotos { get; set; }

        public long ResultadoVotos { get; set; }
        public int PosicionRanking { get; set; }

        public Dictionary<string, long> VotosPorCriterio { get; set; } = new Dictionary<string, long>();
    }
}
