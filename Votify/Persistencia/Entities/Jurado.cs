namespace Votify.Persistencia.Entities;

public class Jurado : Rol
{
    public virtual ICollection<Categorium> Categorias { get; set; } = new List<Categorium>();

    public Jurado()
    {
        Categorias = new List<Categorium>();
    }
}
