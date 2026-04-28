namespace Votify.LógicaNegocio.DTOs
{
    public class CriterioComentariosDto
    {
        public long CriterioId { get; set; }
        public string NombreCriterio { get; set; } = string.Empty;
        public List<string> Comentarios { get; set; } = new List<string>();
    }
}
