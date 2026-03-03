using System;
using System.Collections.Generic;

namespace Votify.Models;

public partial class Notificacion
{
    public long Id { get; set; }

    public string Correo { get; set; } = null!;

    public string Contenido { get; set; } = null!;

    public virtual Usuario CorreoNavigation { get; set; } = null!;
}
