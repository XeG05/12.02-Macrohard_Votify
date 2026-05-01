namespace Votify.Persistencia.Entities;

public class Notificacion
{
    public long Id { get; set; }

    public string Correo { get; set; } = null!;

    public long? IdEvento { get; set; }

    public string Titulo { get; set; } = null!;

    public string Contenido { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public bool Leida { get; set; } = false;

    public string ClaveUnica { get; set; } = null!;

    public virtual Usuario CorreoNavigation { get; set; } = null!;

    public virtual Evento? Evento { get; set; }

    public Notificacion()
    {
    }

    public Notificacion(
        string correo,
        long? idEvento,
        string titulo,
        string contenido,
        string tipo,
        string claveUnica)
    {
        Correo = correo;
        IdEvento = idEvento;
        Titulo = titulo;
        Contenido = contenido;
        Tipo = tipo;
        ClaveUnica = claveUnica;
        FechaCreacion = DateTime.UtcNow;
        Leida = false;
    }
}
