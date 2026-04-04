namespace Votify.Persistencia.Entities;

public class Evento
{
    public string Nombre { get; set; } = null!;

    public long Id { get; set; }

    public string Descripcion { get; set; } = null!;

    public DateOnly FechaFinInscripciones { get; set; }

    public DateOnly FechaFinEntrega { get; set; }

    public DateOnly FechaFinVotaciones { get; set; }

    public bool TieneGrupos { get; set; }

    public bool GruposCompletos { get; set; }

    public int ParticipantesPorGrupo { get; set; }

    public virtual ICollection<Categorium> Categorias { get; set; } = new List<Categorium>();

    public virtual ICollection<Rol> Roles { get; set; } = new List<Rol>();

    public Evento()
    {
        Roles = new List<Rol>();
        Categorias = new List<Categorium>();
    }
    public Evento(string nombre, string descripcion, DateOnly fechaFinInscripciones,
        DateOnly fechaFinEntrega,
        DateOnly fechaFinVotaciones,
        bool tieneGrupos,
        bool gruposCompletos,
        int participantesPorGrupo)
    {
        Nombre = nombre;
        Descripcion = descripcion;
        FechaFinInscripciones = fechaFinInscripciones;
        FechaFinEntrega = fechaFinEntrega;
        FechaFinVotaciones = fechaFinVotaciones;
        TieneGrupos = tieneGrupos;
        GruposCompletos = gruposCompletos;
        ParticipantesPorGrupo = participantesPorGrupo;
        Categorias = new List<Categorium>();
        Roles = new List<Rol>();
    }
}
