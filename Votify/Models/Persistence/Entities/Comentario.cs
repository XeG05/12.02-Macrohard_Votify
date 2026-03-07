using System;
using System.Collections.Generic;

namespace Votify.Models;

public partial class Comentario
{
    public long IdRol { get; set; }

    public string CorreoRol { get; set; } = null!;

    public string Contenido { get; set; } = null!;

    public long IdProyectoGru { get; set; }

    public long IdProyectoCat { get; set; }

    public virtual Proyecto Proyecto { get; set; } = null!;

    public virtual Rol Rol { get; set; } = null!;
}
