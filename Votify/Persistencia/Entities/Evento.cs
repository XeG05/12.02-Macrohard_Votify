using Humanizer;
using Votify.LógicaNegocio.Enums;

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

    public DateOnly FechaFinVotacionExperta { get; set; }

    public TipoComentario TipoComentario { get; set; }

    public bool ParticipantesVotan { get; set; }

    public TipoVotacion TipoVotacion { get; set; }
    public int NumVotarProyectos { get; set; }
    public int PuntosARepartir { get; set; }

    public bool TieneGrupos { get; set; }

    public bool GruposCompletos { get; set; }

    public int ParticipantesPorGrupo { get; set; }

    public string Estado { get; set; } = "Activo";

    public bool EsPrivado { get; set; }

    public string? PasswordAcceso { get; set; }

    public FasesEvento FaseActual { get; set; } = FasesEvento.Inscripciones;

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
        DateOnly fechaFinVotacionExperta,
        TipoComentario tipoComentario,
        bool participantesVotan,
        TipoVotacion tipoVotacion,
        int numVotarProyectos,
        int puntosARepartir,
        bool tieneGrupos,
        bool gruposCompletos,
        int participantesPorGrupo,
        bool esPrivado,
        string passwordAcceso)

    {
        Nombre = nombre;
        Descripcion = descripcion;
        FechaFinInscripciones = fechaFinInscripciones;
        FechaFinEntrega = fechaFinEntrega;
        FechaFinVotaciones = fechaFinVotaciones;
        FechaFinEvaluacionExperta = fechaFinEvaluacionExperta;
        FechaFinVotacionExperta = fechaFinVotacionExperta;
        TipoComentario = tipoComentario;
        ParticipantesVotan = participantesVotan;
        TipoVotacion = tipoVotacion;
        NumVotarProyectos = numVotarProyectos;
        PuntosARepartir = puntosARepartir;
        TieneGrupos = tieneGrupos;
        GruposCompletos = gruposCompletos;
        ParticipantesPorGrupo = participantesPorGrupo;
        EsPrivado = esPrivado;
        PasswordAcceso = passwordAcceso;
        Categorias = new List<Categorium>();
        Roles = new List<Rol>();
    }
}
