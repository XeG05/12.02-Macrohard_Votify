using System;
using System.Collections.Generic;

namespace Votify.Modelos;

public partial class Categorium
{
   public Categorium()
    {
        Criterios = new List<Criterio>();
        Grupos = new List<Grupo>();
    }
    public Categorium(string nombre, string descripcion, long idEvento, long ponderacionPopular, long ponderacionExperto, Evento idEventoNavigation, Criterio criterio1)
    {
        Nombre = nombre;
        Descripcion = descripcion;
        IdEvento = idEvento;
        PonderacionPopular = ponderacionPopular;
        PonderacionExperto = ponderacionExperto;
        IdEventoNavigation = idEventoNavigation;
        Criterios = new List<Criterio>();
        Grupos = new List<Grupo>();

        Criterios.Add(criterio1); //añadimos elemento a la lista por la cardinalidad minima 1

    }
}
