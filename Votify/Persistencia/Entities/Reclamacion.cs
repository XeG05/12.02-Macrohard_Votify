namespace Votify.Persistencia.Entities;

public class Reclamacion
{
    public long IdCriterio { get; set; }

    public long IdRol { get; set; }

    public string Comentario { get; set; } = null!;

    public string CorreoRol { get; set; } = null!;

    public virtual Criterio IdCriterioNavigation { get; set; } = null!;

    public virtual Rol Rol { get; set; } = null!;

    public Reclamacion()
    {

    }
    public Reclamacion(long idCriterio, long idRol, string comentario, string correoRol, Criterio idCriterioNavigation, Rol rol)
    {
        IdCriterio = idCriterio;
        IdRol = idRol;
        Comentario = comentario;
        CorreoRol = correoRol;
        IdCriterioNavigation = idCriterioNavigation;
        Rol = rol;
    }
}
