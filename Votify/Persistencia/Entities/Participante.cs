namespace Votify.Persistencia.Entities;

public class Participante
{
    public string Correo { get; set; } = null!;

    public long Id { get; set; }

    public long IdGrupo { get; set; }

    public virtual Grupo IdGrupoNavigation { get; set; } = null!;

    public virtual Rol Rol { get; set; } = null!;

    public Participante()
    {

    }
    public Participante(string correo, long idGrupo, Grupo idGrupoNavigation, Rol rol)
    {
        Correo = correo;
        IdGrupo = idGrupo;
        IdGrupoNavigation = idGrupoNavigation;
        Rol = rol;
    }
}
