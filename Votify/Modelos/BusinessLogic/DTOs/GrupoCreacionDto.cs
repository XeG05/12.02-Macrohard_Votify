namespace Votify.Modelos.BusinessLogic.DTOs
{
    public class GrupoCreacionDto
    {
        public string Nombre { get; set; } = "";
        public List<string> CorreosParticipantes { get; set; } = new();
    }
}
