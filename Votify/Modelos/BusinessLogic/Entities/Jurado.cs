using System;
using System.Collections.Generic;

namespace Votify.Modelos;

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
