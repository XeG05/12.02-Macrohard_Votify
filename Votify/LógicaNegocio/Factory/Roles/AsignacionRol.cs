using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Factory.Roles
{
    public abstract class AsignacionRol
    {
        public Rol Rol { get; }
        public string Correo => Rol.CorreoUsuario;
        public long IdEvento => Rol.IdEvento;

        protected AsignacionRol(Rol rol) { Rol = rol; }

        public abstract string Tipo { get; }
        public abstract object EntidadEspecifica { get; }
    }
}
