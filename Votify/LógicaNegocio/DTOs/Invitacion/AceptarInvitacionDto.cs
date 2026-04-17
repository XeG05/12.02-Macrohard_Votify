namespace Votify.LógicaNegocio.DTOs
{
    public class AceptarInvitacionDto
    {
        public string Token { get; set; } = string.Empty;
        public string? Nombre { get; set; }
        public string Contraseña { get; set; } = string.Empty;
        public string? ConfirmarContraseña { get; set; }
    }
}
