using System;
using System.Collections.Generic;

namespace Votify.Models;

public partial class Jurado
{
    public string Correo { get; set; } = null!;

    public long Id { get; set; }

    public virtual Rol Rol { get; set; } = null!;
}
