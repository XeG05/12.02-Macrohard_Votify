using System;
using System.Collections.Generic;

namespace Votify.Modelos;

public partial class Evento
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
}
