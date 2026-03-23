using System;
using System.Collections.Generic;
using Votify.Modelos.Persistence.Entities;

namespace Votify.Modelos;

public partial class Proyecto
{
    public string Nombre { get; set; } = null!;

    public long IdGrupo { get; set; }

    public long IdCategoria { get; set; }

    public string Descripcion { get; set; } = "";

    public string DescripcionBreve { get; set; } = "";

    public List<ImagenProyecto> Imagenes { get; set; } = new();

    public string? URLDescarga { get; set; }

    public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

    public virtual Categorium IdCategoriaNavigation { get; set; } = null!;

    public virtual Grupo IdGrupoNavigation { get; set; } = null!;

    public virtual ICollection<Voto> Votos { get; set; } = new List<Voto>();
}
