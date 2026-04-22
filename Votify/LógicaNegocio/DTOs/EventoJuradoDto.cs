namespace Votify.LógicaNegocio.DTOs
{
    public class EventoJuradoDto
    {
        public EventoDto Evento { get; set; } = null!;
        public string Rol { get; set; } = "Jurado";
    }
}
