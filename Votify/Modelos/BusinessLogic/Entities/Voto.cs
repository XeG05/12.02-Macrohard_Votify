using System;
using System.Collections.Generic;

namespace Votify.Modelos;

public partial class Voto
{
    public Voto()
    {

    }
    public Voto(long idCriterio, long? nota, string correoRol, long idProyectoGru, Criterio idCriterioNavigation, Proyecto proyecto, Rol rol)
    {
        IdCriterio = idCriterio;
        Nota = nota;
        CorreoRol = correoRol;
        IdProyectoGru = idProyectoGru;
        IdCriterioNavigation = idCriterioNavigation;
        Proyecto = proyecto;
        Rol = rol;
    }
}
