namespace Votify.LógicaNegocio.DTOs
{
    public class DatosCertificadosDto
    {
        public string NombreParticipante { get; set; } = null!;
        public string NombreEvento { get; set; } = null!;
        public string NombreCategoria { get; set; } = null!;
        public string NombreGrupo { get; set; } = null!;
        public int Posicion { get; set; }
        public DateOnly FechaFinalizacion { get; set; }
    }
}
