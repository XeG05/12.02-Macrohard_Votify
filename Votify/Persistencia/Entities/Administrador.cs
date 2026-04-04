namespace Votify.Persistencia.Entities;

public class Administrador
{
    public string Correo { get; set; } = null!;

    public long Id { get; set; }

    public virtual Rol Rol { get; set; } = null!;

    public Administrador()
    {

    }
    public Administrador(string correo, Rol rol)
    {
        Correo = correo;
        Rol = rol;
    }
}
