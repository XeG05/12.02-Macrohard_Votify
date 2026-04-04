namespace Votify.Persistencia.Entities;

public class Voto
{
    public long Id { get; set; }
    public long IdRol { get; set; }

    public long IdCriterio { get; set; }

    public long? Nota { get; set; }

    public string CorreoRol { get; set; } = null!;

    public long IdProyectoGru { get; set; }

    public virtual Criterio IdCriterioNavigation { get; set; } = null!;

    public virtual Proyecto Proyecto { get; set; } = null!;

    public virtual Rol Rol { get; set; } = null!;

    public Voto()
    {

    }
    public Voto(long idCriterio, long? nota, string correoRol, long idProyectoGru, Criterio idCriterioNavigation, Proyecto proyecto, Rol rol)
    {
        IdCriterio = idCriterio;
        Nota = nota;
        CorreoRol = correoRol;
        IdProyectoGru = idProyectoGru;
        IdCriterioNavigation = idCriterioNavigation;
        Proyecto = proyecto;
        Rol = rol;
    }
}
