namespace Votify.Persistencia.Entities;

public class Votante
{
    public string Correo { get; set; } = null!;

    public long Id { get; set; }

    public virtual Rol Rol { get; set; } = null!;

    public Votante()
    {

    }
    public Votante(string correo, Rol rol)
    {
        Correo = correo;
        Rol = rol;
    }
}
