using System;
using System.Collections.Generic;

namespace Votify.Models;

public partial class Proyecto
{
    public string Nombre { get; set; } = null!;

    public long IdGrupo { get; set; }

    public long IdCategoria { get; set; }

    public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

    public virtual Categorium IdCategoriaNavigation { get; set; } = null!;

    public virtual Grupo IdGrupoNavigation { get; set; } = null!;

    public virtual ICollection<Voto> Votos { get; set; } = new List<Voto>();
}
