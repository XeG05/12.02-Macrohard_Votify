namespace Votify.LógicaNegocio.DTOs
{
    public class RankingCategoriaDto
    {
        public long IdCategoria { get; set; }
        public string NombreCategoria { get; set; } = "";

        public List<ProyectoResultadosDto> Proyectos { get; set; } = new();
    }
}
