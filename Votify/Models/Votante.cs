using System;
using System.Collections.Generic;

namespace Votify.Models;

public partial class Votante
{
    public string Correo { get; set; } = null!;

    public long Id { get; set; }
}
