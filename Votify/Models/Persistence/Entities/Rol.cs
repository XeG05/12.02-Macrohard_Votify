using System;
using System.Collections.Generic;

namespace Votify.Models;

public partial class Rol
{
    public string CorreoUsuario { get; set; } = null!;

    public long IdEvento { get; set; }

    public virtual Administrador? Administrador { get; set; }

    public virtual Usuario CorreoUsuarioNavigation { get; set; } = null!;

    public virtual Evento IdEventoNavigation { get; set; } = null!;

    public virtual Jurado? Jurado { get; set; }

    public virtual Participante? Participante { get; set; }

    public virtual Votante? Votante { get; set; }
}
