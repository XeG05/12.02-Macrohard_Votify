namespace Votify.LógicaNegocio.DTOs
{
    public class EntradaEventoRespuestaDto
    {
        public EventoDto Evento { get; set; } = new();
        public RolInfoDto Rol { get; set; } = new();
    }
}
