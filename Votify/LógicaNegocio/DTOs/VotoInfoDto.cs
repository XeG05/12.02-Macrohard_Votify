namespace Votify.LógicaNegocio.DTOs
{
    public class VotoInfoDto
    {
        public int VotosRestantes { get; set; }
        public bool HaVotado { get; set; }
    }

    public class CriterioVotoInfoDto
    {
        public long IdCriterio { get; set; }
        public int VotosRestantes { get; set; }
        public bool HaVotado { get; set; }
    }
}