namespace Votify.LógicaNegocio.DTOs
{
    public class PerfilUsuarioDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string? ImagenBase64 { get; set; }
    }
}
