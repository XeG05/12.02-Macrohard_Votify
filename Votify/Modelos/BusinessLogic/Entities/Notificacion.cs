using System;
using System.Collections.Generic;

namespace Votify.Modelos;

public partial class Notificacion
{
    public Notificacion()
    {

    }
    public Notificacion(string correo, string contenido, Usuario correoNavigation)
    { 
        this.Correo = correo;
        this.Contenido = contenido;
        this.CorreoNavigation = correoNavigation;
    }
}
