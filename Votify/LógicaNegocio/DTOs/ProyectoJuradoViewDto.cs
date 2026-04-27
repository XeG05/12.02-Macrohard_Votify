namespace Votify.LógicaNegocio.DTOs
{
    public class ProyectoJuradoViewDto
    {
        public long Id { get; set; }
        public long IdGrupo { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string DescripcionBreve { get; set; } = string.Empty;
        public bool Revisado { get; set; }
        public List<string> ParticipantesNombres { get; set; } = new();
    }
}