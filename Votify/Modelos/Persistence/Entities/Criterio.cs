using System;
using System.Collections.Generic;

namespace Votify.Modelos;

public partial class Criterio
{
    public string Nombre { get; set; } = null!;

    public long Id { get; set; }

    public string Descripcion { get; set; } = null!;

    public long IdCategoria { get; set; }

    public long Ponderacion { get; set; }

    public virtual Categorium IdCategoriaNavigation { get; set; } = null!;

    public virtual ICollection<Reclamacion> Reclamaciones { get; set; } = new List<Reclamacion>();

    public virtual ICollection<Voto> Votos { get; set; } = new List<Voto>();
}
