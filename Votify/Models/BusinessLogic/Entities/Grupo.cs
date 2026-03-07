using System;
using System.Collections.Generic;

namespace Votify.Models;

public partial class Grupo
{
    public Grupo()
    {
        Participantes = new List<Participante>();
        Proyectos = new List<Proyecto>();
    }
    public Grupo(string nombre, string? hojaRutaTexto, Participante participante1)
    {
        Nombre = nombre;
        HojaRutaTexto = hojaRutaTexto;
        Participantes = new List<Participante>();
        Proyectos = new List<Proyecto>();

        Participantes.Add(participante1); //añadimos elemento a la lista por la cardinalidad minima 1
    }
}
