using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Factory.Roles
{
    public class AsignacionJurado :AsignacionRol
    {
        public Jurado Jurado { get; }
        public AsignacionJurado(Rol rol, Jurado j) : base(rol) { Jurado = j; }
        public override string Tipo => "JURADO";
        public override object EntidadEspecifica => Jurado;
    }
}
