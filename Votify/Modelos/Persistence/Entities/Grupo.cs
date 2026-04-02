using System;
using System.Collections.Generic;

namespace Votify.Modelos;

public partial class Grupo
{
    public long Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? HojaRutaTexto { get; set; }

    public long IdCategoria { get; set; }

    public virtual Categorium IdCategoriaNavigation { get; set; } = null!;

    public virtual ICollection<Participante> Participantes { get; set; } = new List<Participante>();

    public virtual Proyecto? Proyecto { get; set; }
}
