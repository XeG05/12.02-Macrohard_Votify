using System;
using System.Collections.Generic;

namespace Votify.Models;

public partial class Voto
{
    public Voto()
    {

    }
    public Voto(long idCriterio, long? nota, string correoRol, long idProyectoCat, long idProyectoGru, Criterio idCriterioNavigation, Proyecto proyecto, Rol rol)
    {
        IdCriterio = idCriterio;
        Nota = nota;
        CorreoRol = correoRol;
        IdProyectoCat = idProyectoCat;
        IdProyectoGru = idProyectoGru;
        IdCriterioNavigation = idCriterioNavigation;
        Proyecto = proyecto;
        Rol = rol;
    }
}
