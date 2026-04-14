using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Domain
{
    public class AsignacionParticipante : AsignacionRol
    {
        public Participante Participante { get; }
        public AsignacionParticipante(Rol rol, Participante p) : base(rol) { Participante = p; }
        public override string Tipo => "PARTICIPANTE";
        public override object EntidadEspecifica => Participante;
    }
}
