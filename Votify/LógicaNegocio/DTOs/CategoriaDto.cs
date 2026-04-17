namespace Votify.LógicaNegocio.DTOs
{
    public class CategoriaDto
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public long IdEvento { get; set; }
        public long PonderacionPopular { get; set; }
        public long PonderacionExperto { get; set; }
        public List<CriterioDto> Criterios { get; set; } = new();
        public List<GrupoDto> Grupos { get; set; } = new();
    }
}
