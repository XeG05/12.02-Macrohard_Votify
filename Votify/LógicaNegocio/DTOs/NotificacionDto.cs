namespace Votify.LógicaNegocio.DTOs
{

    public class NotificacionDto
    {
        public long Id { get; set; }

        public long? IdEvento { get; set; }

        public string Titulo { get; set; } = null!;

        public string Contenido { get; set; } = null!;

        public string Tipo { get; set; } = null!;

        public DateTime FechaCreacion { get; set; }

        public bool Leida { get; set; }

        public string? NombreEvento { get; set; }
    }
}