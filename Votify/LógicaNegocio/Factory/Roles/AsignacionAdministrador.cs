using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Factory.Roles
{
    public class AsignacionAdministrador : AsignacionRol
    {
        public Administrador Administrador { get; }
        public AsignacionAdministrador(Rol rol, Administrador a) : base(rol) { Administrador = a; }
        public override string Tipo => "ADMIN";
        public override object EntidadEspecifica => Administrador;
    }
}
