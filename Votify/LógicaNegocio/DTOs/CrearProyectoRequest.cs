namespace Votify.LógicaNegocio.DTOs
{
    public class CrearProyectoRequest
    {
        public string Correo { get; set; } = string.Empty;
        public long IdEvento { get; set; }
        public long? GrupoIdActivo { get; set; }

        public string NombreProyecto { get; set; } = string.Empty;
        public string DescripcionBreve { get; set; } = string.Empty;
        public string DescripcionCompleta { get; set; } = string.Empty;

        public byte[] Archivo { get; set; } = Array.Empty<byte>();
        public List<byte[]> Imagenes { get; set; } = new List<byte[]>();
    }
}
