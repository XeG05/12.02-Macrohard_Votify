using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Factory.Roles
{
    public class AsignacionVotante : AsignacionRol
    {
        public Votante Votante { get; }
        public AsignacionVotante(Rol rol, Votante v) : base(rol) { Votante = v; }
        public override string Tipo => "VOTANTE";
        public override object EntidadEspecifica => Votante;
    }
}
