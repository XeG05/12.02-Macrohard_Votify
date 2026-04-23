using Humanizer;

namespace Votify.Persistencia.Entities;

public class Evento
{
    public string Nombre { get; set; } = null!;

    public long Id { get; set; }

    public string Descripcion { get; set; } = null!;

    public DateOnly FechaFinInscripciones { get; set; }

    public DateOnly FechaFinEntrega { get; set; }

    public DateOnly FechaFinVotaciones { get; set; }

    public DateOnly FechaFinEvaluacionExperta { get; set; }

    public bool SinComentarios { get; set; }
    public bool ComentariosOpcionales { get; set; }
    public bool ComentariosObligatorios { get; set; }

    public bool ParticipantesVotan { get; set; }

    public bool VotarXProyectos { get; set; }
    public bool RepartirPuntos { get; set; }
    public bool CalificarNumericamente { get; set; }
    public int VotarProyectos { get; set; }
    public int PuntosARepartir { get; set; }

    public bool TieneGrupos { get; set; }

    public bool GruposCompletos { get; set; }

    public int ParticipantesPorGrupo { get; set; }

    public string Estado { get; set; } = "Activo";

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
        DateOnly fechaFinEvaluacionExperta,
        bool sinComentarios,
        bool comentariosOpcionales,
        bool comentariosObligatorios,
        bool participantesVotan,
        bool calificarNumericamente,
        bool votarXProyecto,
        int votarProyecto,
        bool repartirPuntos,
        int puntosARepartir,
        bool tieneGrupos,
        bool gruposCompletos,
        int participantesPorGrupo)
    {
        Nombre = nombre;
        Descripcion = descripcion;
        FechaFinInscripciones = fechaFinInscripciones;
        FechaFinEntrega = fechaFinEntrega;
        FechaFinVotaciones = fechaFinVotaciones;
        FechaFinEvaluacionExperta = fechaFinEvaluacionExperta;
        SinComentarios = sinComentarios;
        ComentariosOpcionales = comentariosOpcionales;
        ComentariosObligatorios = comentariosObligatorios;
        ParticipantesVotan = participantesVotan;
        CalificarNumericamente = calificarNumericamente;
        VotarXProyectos = votarXProyecto;
        VotarProyectos = votarProyecto;
        RepartirPuntos = repartirPuntos;
        PuntosARepartir = puntosARepartir;
        TieneGrupos = tieneGrupos;
        GruposCompletos = gruposCompletos;
        ParticipantesPorGrupo = participantesPorGrupo;
        Categorias = new List<Categorium>();
        Roles = new List<Rol>();
    }
}
