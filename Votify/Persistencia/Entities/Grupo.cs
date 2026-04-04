namespace Votify.Persistencia.Entities;

public class Grupo
{
    public long Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? HojaRutaTexto { get; set; }

    public long IdCategoria { get; set; }

    public virtual Categorium IdCategoriaNavigation { get; set; } = null!;

    public virtual ICollection<Participante> Participantes { get; set; } = new List<Participante>();

    public virtual Proyecto? Proyecto { get; set; }

    public Grupo()
    {
        Participantes = new List<Participante>();
    }
    public Grupo(string nombre, string? hojaRutaTexto, long idCategoria, Categorium idCategoriaNavigation)
    {
        Nombre = nombre;
        HojaRutaTexto = hojaRutaTexto;
        IdCategoria = idCategoria;
        IdCategoriaNavigation = idCategoriaNavigation;
        Participantes = new List<Participante>();

    }
}
