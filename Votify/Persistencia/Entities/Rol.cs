namespace Votify.Persistencia.Entities;

public class Rol
{
    public string CorreoUsuario { get; set; } = null!;

    public long IdEvento { get; set; }

    public virtual Administrador? Administrador { get; set; }

    public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

    public virtual Usuario CorreoUsuarioNavigation { get; set; } = null!;

    public virtual Evento IdEventoNavigation { get; set; } = null!;

    public virtual Jurado? Jurado { get; set; }

    public virtual ICollection<Participante> Participantes { get; set; } = new List<Participante>();

    public virtual ICollection<Reclamacion> Reclamaciones { get; set; } = new List<Reclamacion>();

    public virtual Votante? Votante { get; set; }

    public virtual ICollection<Voto> Votos { get; set; } = new List<Voto>();

    public Rol()
    {
        Comentarios = new List<Comentario>();
        Reclamaciones = new List<Reclamacion>();
        Votos = new List<Voto>();
    }
    public Rol(string correoUsuario, Administrador? administrador, Usuario correoUsuarioNavigation, Evento idEventoNavigation, Jurado? jurado, ICollection<Participante>? participantes, Votante? votante)
    {
        CorreoUsuario = correoUsuario;
        Administrador = administrador;
        CorreoUsuarioNavigation = correoUsuarioNavigation;
        IdEventoNavigation = idEventoNavigation;
        Jurado = jurado;
        if (participantes != null) Participantes = participantes;
        Votante = votante;
        Comentarios = new List<Comentario>();
        Reclamaciones = new List<Reclamacion>();
        Votos = new List<Voto>();
        Participantes = new List<Participante>();
    }
}
