namespace Votify.LógicaNegocio.DTOs
{
    public class DashboardParticipanteDTO
    {
        public string NombreProyecto { get; set; } = "";
        public string NombreGrupo { get; set; } = "";

        public int TotalVotos { get; set; }
        public int Posicion { get; set; }

        public List<string> Comentarios { get; set; } = new();
    }
}
