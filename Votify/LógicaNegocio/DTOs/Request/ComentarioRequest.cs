namespace Votify.LógicaNegocio.DTOs
{
    public class ComentarioRequest
    {
        public long IdEvento { get; set; }
        public long IdProyecto { get; set; }
        public string Comentario { get; set; } = string.Empty;
    }
}
