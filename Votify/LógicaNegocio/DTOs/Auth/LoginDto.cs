using System.ComponentModel.DataAnnotations;

namespace Votify.LógicaNegocio.DTOs
{
    public class LoginDto
    {
         [Required(ErrorMessage = "El correo es requerido")]
         [EmailAddress(ErrorMessage = "El correo no tiene formato válido")]
         [MaxLength(50)]
        public string Correo { get; set; } = string.Empty;

         [Required(ErrorMessage = "La contraseña es requerida")]
         [MaxLength(72)]
        public string Contraseña { get; set; } = string.Empty;
    }
}