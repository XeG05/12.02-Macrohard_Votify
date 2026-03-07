using System;
using System.Collections.Generic;

namespace Votify.Models;

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
