using System;
using System.Collections.Generic;

namespace Votify.Models;

public partial class Evento
{
    public string Nombre { get; set; } = null!;

    public long Id { get; set; }

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<Categorium> Categorias { get; set; } = new List<Categorium>();

    public virtual ICollection<Rol> Roles { get; set; } = new List<Rol>();
}
