namespace Votify.Persistencia.Entities;

public class Jurado
{
    public string Correo { get; set; } = null!;

    public long Id { get; set; }

    public virtual Rol Rol { get; set; } = null!;

    public Jurado()
    {

    }
    public Jurado(string correo, Rol rol)
    {
        Correo = correo;
        Rol = rol;
    }
}
