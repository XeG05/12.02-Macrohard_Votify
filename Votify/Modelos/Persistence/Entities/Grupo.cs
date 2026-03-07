using System;
using System.Collections.Generic;

namespace Votify.Modelos;

public partial class Grupo
{
    public long Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? HojaRutaTexto { get; set; }

    public virtual ICollection<Participante> Participantes { get; set; } = new List<Participante>();

    public virtual ICollection<Proyecto> Proyectos { get; set; } = new List<Proyecto>();
}
