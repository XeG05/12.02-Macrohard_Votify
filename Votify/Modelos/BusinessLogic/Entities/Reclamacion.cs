using System;
using System.Collections.Generic;

namespace Votify.Modelos;

public partial class Reclamacion
{
    public Reclamacion()
    {

    }
    public Reclamacion(long idCriterio, long idRol, string comentario, string correoRol, Criterio idCriterioNavigation, Rol rol)
    {
        IdCriterio = idCriterio;
        IdRol = idRol;
        Comentario = comentario;
        CorreoRol = correoRol;
        IdCriterioNavigation = idCriterioNavigation;
        Rol = rol;
    }
}
