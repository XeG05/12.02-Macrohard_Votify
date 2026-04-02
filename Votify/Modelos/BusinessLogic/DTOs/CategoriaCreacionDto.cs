namespace Votify.Modelos.BusinessLogic.DTOs
{
    public class CategoriaCreacionDto
    {
        public string Nombre { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public long PonderacionPopular { get; set; }
        public long PonderacionExperto { get; set; }
        public List<CriterioCreacionDto> Criterios { get; set; } = new();
        public List<GrupoCreacionDto> Grupos { get; set; } = new();
    }
}
