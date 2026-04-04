namespace Votify.Persistencia.Entities;

public class Notificacion
{
    public long Id { get; set; }

    public string Correo { get; set; } = null!;

    public string Contenido { get; set; } = null!;

    public virtual Usuario CorreoNavigation { get; set; } = null!;

    public Notificacion()
    {

    }
    public Notificacion(string correo, string contenido, Usuario correoNavigation)
    {
        this.Correo = correo;
        this.Contenido = contenido;
        this.CorreoNavigation = correoNavigation;
    }
}
