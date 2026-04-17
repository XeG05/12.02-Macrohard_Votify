namespace Votify.LógicaNegocio.DTOs
{
    public class ParticipanteDto
    {
        public string Correo { get; set; } = string.Empty;
        public long Id { get; set; }
        public long IdGrupo { get; set; }
    }
}
