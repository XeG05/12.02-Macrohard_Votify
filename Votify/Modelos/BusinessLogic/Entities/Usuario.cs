using System;
using System.Collections.Generic;

namespace Votify.Modelos;

public partial class Usuario
{
    public Usuario()
    {
        Notificaciones = new List<Notificacion>();
        Roles = new List<Rol>();
    }
    public Usuario(string nombre, string contraseña, string correo)
    {
        this.Nombre = nombre;
        this.Contraseña = contraseña;
        this.Correo = correo;
        this.Notificaciones = new List<Notificacion>();
        this.Roles = new List<Rol>();
    }
}
