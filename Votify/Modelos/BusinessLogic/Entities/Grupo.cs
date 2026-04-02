using System;
using System.Collections.Generic;

namespace Votify.Modelos;

public partial class Grupo
{
    public Grupo()
    {
        Participantes = new List<Participante>();
    }
    public Grupo(string nombre, string? hojaRutaTexto, long idCategoria, Categorium idCategoriaNavigation)
    {
        Nombre = nombre;
        HojaRutaTexto = hojaRutaTexto;
        IdCategoria = idCategoria;
        IdCategoriaNavigation = idCategoriaNavigation;
        Participantes = new List<Participante>();

    }
}
