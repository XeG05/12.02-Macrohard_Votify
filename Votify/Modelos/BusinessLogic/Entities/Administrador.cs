using System;
using System.Collections.Generic;

namespace Votify.Modelos;

public partial class Administrador
{
    public Administrador()
    {

    }
    public Administrador(string correo, Rol rol)
    {
        Correo = correo;
        Rol = rol;
    }
}
