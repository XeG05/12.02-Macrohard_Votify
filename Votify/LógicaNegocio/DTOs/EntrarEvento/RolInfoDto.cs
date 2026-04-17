namespace Votify.LógicaNegocio.DTOs
{
    public class RolInfoDto
    {
        public bool EsAdministrador { get; set; }
        public bool EsParticipante { get; set; }
        public bool EsVotante { get; set; }
        public bool EsJurado { get; set; }
    }
}
