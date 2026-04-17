namespace Votify.LógicaNegocio.DTOs
{
    public class GrupoDto
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = "";
        public long IdCategoria { get; set; }
        public List<string> CorreosParticipantes { get; set; } = new();
    }
}
