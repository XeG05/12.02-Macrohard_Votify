namespace Votify.LógicaNegocio.DTOs
{
    public class EventoParticipanteDto
    {
        public EventoDto Evento { get; set; } = new();
        public string Rol { get; set; } = string.Empty;
        public GrupoDto? Grupo { get; set; }
    }
}
