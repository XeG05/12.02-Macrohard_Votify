namespace Votify.Persistencia.Entities;

public class Rol
{
    public string CorreoUsuario { get; set; } = null!;

    public long IdEvento { get; set; }

    public virtual Usuario CorreoUsuarioNavigation { get; set; } = null!;

    public virtual Evento IdEventoNavigation { get; set; } = null!;

    public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

    public virtual ICollection<Reclamacion> Reclamaciones { get; set; } = new List<Reclamacion>();

    public virtual ICollection<Voto> Votos { get; set; } = new List<Voto>();
}
