using System;
using System.Collections.Generic;

namespace Votify.Modelos;

public partial class Grupo
{
    public Grupo()
    {
        Participantes = new List<Participante>();
        Proyectos = new List<Proyecto>();
    }
    public Grupo(string nombre, string? hojaRutaTexto)
    {
        Nombre = nombre;
        HojaRutaTexto = hojaRutaTexto;
        Participantes = new List<Participante>();
        Proyectos = new List<Proyecto>();

    }
}
