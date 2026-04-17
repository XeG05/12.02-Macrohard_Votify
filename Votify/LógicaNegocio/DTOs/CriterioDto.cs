namespace Votify.LógicaNegocio.DTOs
{
    public class CriterioDto
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public long Ponderacion { get; set; }
    }
}
