namespace Votify.LógicaNegocio.DTOs
{
    public class RegistroDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Contraseña { get; set; } = string.Empty;
        public string ConfirmarContraseña { get; set; } = string.Empty;
    }
}
