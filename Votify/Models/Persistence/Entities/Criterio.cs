using System;
using System.Collections.Generic;

namespace Votify.Models;

public partial class Criterio
{
    public string Nombre { get; set; } = null!;

    public long Id { get; set; }

    public string Descripcion { get; set; } = null!;

    public long IdCategoria { get; set; }

    public virtual Categorium IdCategoriaNavigation { get; set; } = null!;

    public virtual ICollection<Reclamacion> Reclamacions { get; set; } = new List<Reclamacion>();
}
