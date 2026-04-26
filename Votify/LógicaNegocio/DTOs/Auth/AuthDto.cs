namespace Votify.LógicaNegocio.DTOs
{
    public class AuthDto
    {
        public string Token { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string? ImagenBase64 { get; set; }
    }
}
