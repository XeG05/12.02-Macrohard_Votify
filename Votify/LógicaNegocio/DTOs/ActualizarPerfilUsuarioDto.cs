namespace Votify.LógicaNegocio.DTOs
{
    public class ActualizarPerfilUsuarioDto
    {
        public string Nombre { get; set; } = string.Empty;

        public string? NuevaContraseña { get; set; }

        public string? RepetirContraseña { get; set; }

        public Microsoft.AspNetCore.Http.IFormFile? Imagen { get; set; }
    }
}
