using System;
using System.Collections.Generic;

namespace Votify.Models;

public partial class Categorium
{
    public long Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public long IdEvento { get; set; }

    public long PonderacionPopular { get; set; }

    public long PonderacionExperto { get; set; }

    public virtual ICollection<Criterio> Criterios { get; set; } = new List<Criterio>();

    public virtual Evento IdEventoNavigation { get; set; } = null!;

    public virtual ICollection<Proyecto> Proyectos { get; set; } = new List<Proyecto>();
}
