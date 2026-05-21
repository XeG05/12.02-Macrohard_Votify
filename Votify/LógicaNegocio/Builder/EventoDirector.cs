using Votify.LógicaNegocio.DTOs;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Builder
{
    public class EventoDirector
    {
        public Evento Construir(EventoBuilder builder, EventoCreacionDto dto)
        {
            builder.Reset();

            builder
                .ConIdentificacion(dto.Nombre, dto.Descripcion)
                .ConFechas(
                    dto.FechaFinInscripciones!.Value,
                    dto.FechaFinEntrega!.Value,
                    dto.FechaFinVotaciones!.Value,
                    dto.FechaFinExperta!.Value,
                    dto.FechaFinVotExperta!.Value)
                .ConComentarios(dto.TipoComentario)
                .ConVotacion(
                    dto.TipoVotacion,
                    dto.ParticipantesVotan,
                    dto.NumVotarProyectos,
                    dto.PuntosARepartir)
                .ConGrupos(
                    dto.TieneGrupos,
                    dto.GruposCompletos,
                    dto.NumParticipantes)
                .ConPrivacidad(dto.EsPrivado, dto.PasswordAcceso);

            return builder.GetResultado();
        }
    }
}
