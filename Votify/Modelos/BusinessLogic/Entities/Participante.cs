using System;
using System.Collections.Generic;

namespace Votify.Modelos;

public partial class Participante
{
    public Participante()
    {

    }
    public Participante(string correo, long idGrupo, Grupo idGrupoNavigation, Rol rol)
    {
        Correo = correo;
        IdGrupo = idGrupo;
        IdGrupoNavigation = idGrupoNavigation;
        Rol = rol;
    }
}
