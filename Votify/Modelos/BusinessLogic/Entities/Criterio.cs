using System;
using System.Collections.Generic;

namespace Votify.Modelos;

public partial class Criterio
{
    public Criterio()
    {
        Reclamaciones = new List<Reclamacion>();
        Votos = new List<Voto>();

    }
    public Criterio(string nombre, string descripcion, long idCategoria, long ponderacion, Categorium idCategoriaNavigation)
    {
        Nombre = nombre;
        Descripcion = descripcion;
        IdCategoria = idCategoria;
        Ponderacion = ponderacion;
        IdCategoriaNavigation = idCategoriaNavigation;
        Reclamaciones = new List<Reclamacion>();
        Votos = new List<Voto>();
    }
}
