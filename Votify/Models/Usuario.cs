using System;
using System.Collections.Generic;

namespace Votify.Models;

public partial class Usuario
{
    public string Nombre { get; set; } = null!;

    public string Contraseña { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public virtual ICollection<Notificacion> Notificacions { get; set; } = new List<Notificacion>();

    public virtual ICollection<Rol> Rols { get; set; } = new List<Rol>();
}
