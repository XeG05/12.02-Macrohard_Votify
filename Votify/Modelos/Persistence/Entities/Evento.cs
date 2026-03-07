using System;
using System.Collections.Generic;

namespace Votify.Modelos;

public partial class Evento
{
    public string Nombre { get; set; } = null!;

    public long Id { get; set; }

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<Categorium> Categoria { get; set; } = new List<Categorium>();

    public virtual ICollection<Rol> Rols { get; set; } = new List<Rol>();
}
