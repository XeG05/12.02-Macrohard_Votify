using System;
using System.Collections.Generic;

namespace Votify.Models;

public partial class Jurado
{
    public Jurado()
    {

    }
    public Jurado(string correo, Rol rol)
    {
        Correo = correo;
        Rol = rol;
    }
}
