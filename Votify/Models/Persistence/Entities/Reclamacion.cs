using System;
using System.Collections.Generic;

namespace Votify.Models;

public partial class Reclamacion
{
    public long IdCriterio { get; set; }

    public long IdRol { get; set; }

    public string Comentario { get; set; } = null!;

    public string CorreoRol { get; set; } = null!;

    public virtual Criterio IdCriterioNavigation { get; set; } = null!;

    public virtual Rol Rol { get; set; } = null!;
}
