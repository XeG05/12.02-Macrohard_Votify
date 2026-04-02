using System;
using System.Collections.Generic;

namespace Votify.Modelos;

public partial class Proyecto
{
    public Proyecto()
    {
        Comentarios = new List<Comentario>();
        Votos = new List<Voto>();
    }
    public Proyecto(string nombre, long idGrupo, Grupo idGrupoNavigation, string descripcionBreve, string descripcionCompleta)
    {
        Nombre = nombre;
        IdGrupo = idGrupo;
        
        IdGrupoNavigation = idGrupoNavigation;
        Comentarios = new List<Comentario>();
        Votos = new List<Voto>();

        // Asignar descripciones (no pueden ser null según la BD); asumimos cadenas no nulas
        DescripcionBreve = descripcionBreve.Trim();
        Descripcion = descripcionCompleta.Trim();
    }

    public override bool Equals(object? obj)
    {
        if (obj is Proyecto otro)
        {
            return (this.IdGrupo == otro.IdGrupo);
        }
        return false;
    }

}
