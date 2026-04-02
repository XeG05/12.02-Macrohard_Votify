using System;
using System.Collections.Generic;

namespace Votify.Modelos;

public partial class Comentario
{
    public Comentario()
    {

    }
    public Comentario(long idRol, string correoRol, string contenido, long idProyectoGru, Proyecto proyecto, Rol rol)
    {
        IdRol = idRol;
        CorreoRol = correoRol;
        Contenido = contenido;
        IdProyectoGru = idProyectoGru;
        Proyecto = proyecto;
        Rol = rol;
    }
}
