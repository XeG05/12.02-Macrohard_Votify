using System.ComponentModel.DataAnnotations;

  namespace Votify.LógicaNegocio.DTOs
{
    public class RegistroDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "El correo no tiene formato válido")]
        [MaxLength(50)]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        [MaxLength(72)]
        public string Contraseña { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirmar contraseña es requerido")]
        [Compare(nameof(Contraseña), ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarContraseña { get; set; } = string.Empty;
    }
}