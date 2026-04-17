namespace Votify.LógicaNegocio.DTOs
{
    public class VotoRequest
    {
        public long IdEvento { get; set; }
        public long IdProyecto { get; set; }
        public long IdCriterio { get; set; }
        public int? Nota { get; set; }
    }
}
