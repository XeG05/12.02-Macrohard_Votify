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
        public bool HaVotado { get; set; }
        public int? VotosRestantes { get; set; }
        public long? NotaDada { get; set; }
        public long? PuntosAsignados { get; set; }
        public int? PuntosRestantes { get; set; }
    }
}