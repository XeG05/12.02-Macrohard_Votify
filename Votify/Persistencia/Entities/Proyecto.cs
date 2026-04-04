namespace Votify.Persistencia.Entities;

public class Proyecto
{
    public long Id { get; set; }
    public string Nombre { get; set; } = null!;

    public long IdGrupo { get; set; }

    public string Descripcion { get; set; } = "";

    public string DescripcionBreve { get; set; } = "";

    public List<ImagenProyecto> Imagenes { get; set; } = new();

    public byte[] Archivo { get; set; } = null!;

    public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

    public virtual Grupo IdGrupoNavigation { get; set; } = null!;

    public virtual ICollection<Voto> Votos { get; set; } = new List<Voto>();

    public Proyecto()
    {
        Comentarios = new List<Comentario>();
        Votos = new List<Voto>();
    }
    public Proyecto(string nombre, long idGrupo, Grupo idGrupoNavigation, string descripcionBreve, string descripcionCompleta)
    {
        Nombre = nombre;
        IdGrupo = idGrupo;

        IdGrupoNavigation = idGrupoNavigation;
        Comentarios = new List<Comentario>();
        Votos = new List<Voto>();

        // Asignar descripciones (no pueden ser null según la BD); asumimos cadenas no nulas
        DescripcionBreve = descripcionBreve.Trim();
        Descripcion = descripcionCompleta.Trim();
    }

    public override bool Equals(object? obj)
    {
        if (obj is Proyecto otro)
        {
            return (this.IdGrupo == otro.IdGrupo);
        }
        return false;
    }
}
