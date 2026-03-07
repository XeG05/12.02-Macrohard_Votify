using System;
using System.Collections.Generic;

namespace Votify.Models;

public partial class Rol
{
    public Rol()
    {
        Comentarios = new List<Comentario>();
        Reclamaciones = new List<Reclamacion>();
        Votos = new List<Voto>();
    }
    public Rol(string correoUsuario, Administrador? administrador, Usuario correoUsuarioNavigation, Evento idEventoNavigation, Jurado? jurado, Participante? participante, Votante? votante)
    {
        CorreoUsuario = correoUsuario;
        Administrador = administrador;
        CorreoUsuarioNavigation = correoUsuarioNavigation;
        IdEventoNavigation = idEventoNavigation;
        Jurado = jurado;
        Participante = participante;
        Votante = votante;
        Comentarios = new List<Comentario>();
        Reclamaciones = new List<Reclamacion>();
        Votos = new List<Voto>();
    }
}
