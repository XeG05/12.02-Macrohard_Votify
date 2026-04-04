namespace Votify.Persistencia.Entities;

public class Categorium
{
    public long Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public long IdEvento { get; set; }

    public long PonderacionPopular { get; set; }

    public long PonderacionExperto { get; set; }

    public virtual ICollection<Criterio> Criterios { get; set; } = new List<Criterio>();

    public virtual Evento IdEventoNavigation { get; set; } = null!;

    public virtual ICollection<Grupo> Grupos { get; set; } = new List<Grupo>();

    public Categorium()
    {
        Criterios = new List<Criterio>();
        Grupos = new List<Grupo>();
    }
    public Categorium(string nombre, string descripcion, long idEvento, long ponderacionPopular, long ponderacionExperto, Evento idEventoNavigation, Criterio criterio1)
    {
        Nombre = nombre;
        Descripcion = descripcion;
        IdEvento = idEvento;
        PonderacionPopular = ponderacionPopular;
        PonderacionExperto = ponderacionExperto;
        IdEventoNavigation = idEventoNavigation;
        Criterios = new List<Criterio>();
        Grupos = new List<Grupo>();

        Criterios.Add(criterio1); //añadimos elemento a la lista por la cardinalidad minima 1

    }
}
