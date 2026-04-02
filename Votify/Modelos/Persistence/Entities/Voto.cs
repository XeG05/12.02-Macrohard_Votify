using System;
using System.Collections.Generic;

namespace Votify.Modelos;

public partial class Voto
{
    public long Id { get; set; }
    public long IdRol { get; set; }

    public long IdCriterio { get; set; }

    public long? Nota { get; set; }

    public string CorreoRol { get; set; } = null!;

    public long IdProyectoGru { get; set; }

    public virtual Criterio IdCriterioNavigation { get; set; } = null!;

    public virtual Proyecto Proyecto { get; set; } = null!;

    public virtual Rol Rol { get; set; } = null!;
}
