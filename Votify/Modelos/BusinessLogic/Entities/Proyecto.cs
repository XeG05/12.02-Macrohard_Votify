using System;
using System.Collections.Generic;

namespace Votify.Modelos;

public partial class Proyecto
{
    public Proyecto()
    {
        Comentarios = new List<Comentario>();
        Votos = new List<Voto>();
    }
    public Proyecto(string nombre, long idGrupo, long idCategoria, Categorium idCategoriaNavigation, Grupo idGrupoNavigation)
    {
        Nombre = nombre;
        IdGrupo = idGrupo;
        IdCategoria = idCategoria;
        IdCategoriaNavigation = idCategoriaNavigation;
        IdGrupoNavigation = idGrupoNavigation;
        Comentarios = new List<Comentario>();
        Votos = new List<Voto>();
    }
}
