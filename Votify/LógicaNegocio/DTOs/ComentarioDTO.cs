namespace Votify.LógicaNegocio.DTOs
{
    public class ComentarioDto
    {
        public long IdCriterio { get; set; }
        public string Autor { get; set; } = "";
        public string Contenido { get; set; } = "";
    }
}
