namespace Votify.LógicaNegocio.DTOs
{
    public class CategoriaJuradoDto
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public int ProyectosRevisados { get; set; }
        public int ProyectosTotales { get; set; }
    }
}
