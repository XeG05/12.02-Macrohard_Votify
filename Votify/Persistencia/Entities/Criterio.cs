namespace Votify.Persistencia.Entities;

public class Criterio
{
    public string Nombre { get; set; } = null!;

    public long Id { get; set; }

    public string Descripcion { get; set; } = null!;

    public long IdCategoria { get; set; }

    public long Ponderacion { get; set; }

    public virtual Categorium IdCategoriaNavigation { get; set; } = null!;

    public virtual ICollection<Reclamacion> Reclamaciones { get; set; } = new List<Reclamacion>();

    public virtual ICollection<Voto> Votos { get; set; } = new List<Voto>();

    public Criterio()
    {
        Reclamaciones = new List<Reclamacion>();
        Votos = new List<Voto>();

    }
    public Criterio(string nombre, string descripcion, long idCategoria, long ponderacion, Categorium idCategoriaNavigation)
    {
        Nombre = nombre;
        Descripcion = descripcion;
        IdCategoria = idCategoria;
        Ponderacion = ponderacion;
        IdCategoriaNavigation = idCategoriaNavigation;
        Reclamaciones = new List<Reclamacion>();
        Votos = new List<Voto>();
    }
}
