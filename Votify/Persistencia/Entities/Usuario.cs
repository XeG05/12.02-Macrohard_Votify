namespace Votify.Persistencia.Entities;

public class Usuario
{
    public string Nombre { get; set; } = null!;

    public string Contraseña { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public virtual ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();

    public virtual ICollection<Rol> Roles { get; set; } = new List<Rol>();

    public Usuario()
    {
        Notificaciones = new List<Notificacion>();
        Roles = new List<Rol>();
    }
    public Usuario(string nombre, string contraseña, string correo)
    {
        this.Nombre = nombre;
        this.Contraseña = contraseña;
        this.Correo = correo;
        this.Notificaciones = new List<Notificacion>();
        this.Roles = new List<Rol>();
    }
}
