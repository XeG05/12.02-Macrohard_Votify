namespace Votify.LógicaNegocio.DTOs
{
    public class InvitacionDto
    {
        public string CorreoDestino { get; set; } = string.Empty;
        public string NombreEvento { get; set; } = string.Empty;
        public bool UsuarioExiste { get; set; }
        public bool Aceptada { get; set; }
    }
}
