using System;
using System.Collections.Generic;

namespace Votify.Modelos;

public partial class Evento
{
    public Evento()
    {
        Roles = new List<Rol>();
        Categorias = new List<Categorium>();
    }
    public Evento(string nombre, string descripcion, DateOnly fechaFinInscripciones,
        DateOnly fechaFinEntrega,
        DateOnly fechaFinVotaciones,
        bool tieneGrupos,
        bool gruposCompletos,
        int participantesPorGrupo)
    {
        Nombre = nombre;
        Descripcion = descripcion;
        FechaFinInscripciones = fechaFinInscripciones;
        FechaFinEntrega = fechaFinEntrega;
        FechaFinVotaciones = fechaFinVotaciones;
        TieneGrupos = tieneGrupos;
        GruposCompletos = gruposCompletos;
        ParticipantesPorGrupo = participantesPorGrupo;
        Categorias = new List<Categorium>();
        Roles = new List<Rol>();
    }
}
