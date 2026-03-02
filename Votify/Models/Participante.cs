using System;
using System.Collections.Generic;

namespace Votify.Models;

public partial class Participante
{
    public string Correo { get; set; } = null!;

    public long Id { get; set; }

    public long IdGrupo { get; set; }

    public virtual Grupo IdGrupoNavigation { get; set; } = null!;
}
