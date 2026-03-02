using System;
using System.Collections.Generic;

namespace Votify.Models;

public partial class Rol
{
    public string Correo { get; set; } = null!;

    public long Id { get; set; }

    public virtual Usuario CorreoNavigation { get; set; } = null!;

    public virtual Evento IdNavigation { get; set; } = null!;

    public virtual Jurado? Jurado { get; set; }
}
