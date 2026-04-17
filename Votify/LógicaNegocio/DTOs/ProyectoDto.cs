namespace Votify.LógicaNegocio.DTOs
{
    public class ProyectoDto
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public long IdGrupo { get; set; }
        public string DescripcionBreve { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
}
