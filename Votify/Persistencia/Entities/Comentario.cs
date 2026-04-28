namespace Votify.Persistencia.Entities;

public class Comentario
{
    public long IdRol { get; set; }

    public string CorreoRol { get; set; } = null!;

    public string Contenido { get; set; } = null!;

    public long IdProyectoGru { get; set; }

    public long IdCriterio { get; set; }

    public virtual Proyecto Proyecto { get; set; } = null!;

    public virtual Rol Rol { get; set; } = null!;

    public virtual Criterio Criterio { get; set; } = null!;

    public Comentario()
    {

    }
    public Comentario(long idRol, string correoRol, string contenido, long idProyectoGru, long idCriterio, Proyecto proyecto, Rol rol, Criterio criterio)
    {
        IdRol = idRol;
        CorreoRol = correoRol;
        Contenido = contenido;
        IdProyectoGru = idProyectoGru;
        IdCriterio = idCriterio;
        Proyecto = proyecto;
        Rol = rol;
        Criterio = criterio;
    }
}
