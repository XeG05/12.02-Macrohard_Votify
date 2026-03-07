using System;
using System.Collections.Generic;

namespace Votify.Models;

public partial class Evento
{
    public Evento()
    {
        Roles = new List<Rol>();
        Categorias = new List<Categorium>();
    }
    public Evento(string nombre, string descripcion)
    {
        Nombre = nombre;
        Descripcion = descripcion;
        Categorias = new List<Categorium>();
        Roles = new List<Rol>();
    }
}
