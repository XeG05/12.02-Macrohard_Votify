namespace Votify.Persistencia.Entities;

public class Participante : Rol
{
    public long IdGrupo { get; set; }

    public virtual Grupo IdGrupoNavigation { get; set; } = null!;
}
