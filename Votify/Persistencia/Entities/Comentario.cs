namespace Votify.Persistencia.Entities;

public class Comentario
{
    public long IdRol { get; set; }

    public string CorreoRol { get; set; } = null!;

    public string Contenido { get; set; } = null!;

    public long IdProyectoGru { get; set; }

    public virtual Proyecto Proyecto { get; set; } = null!;

    public virtual Rol Rol { get; set; } = null!;

    public Comentario()
    {

    }
    public Comentario(long idRol, string correoRol, string contenido, long idProyectoGru, Proyecto proyecto, Rol rol)
    {
        IdRol = idRol;
        CorreoRol = correoRol;
        Contenido = contenido;
        IdProyectoGru = idProyectoGru;
        Proyecto = proyecto;
        Rol = rol;
    }
}
