using System;
using System.Collections.Generic;

namespace Votify.Modelos;

public partial class Votante
{
    public Votante()
    {

    }
    public Votante(string correo, Rol rol)
    {
        Correo = correo;
        Rol = rol;
    }
}