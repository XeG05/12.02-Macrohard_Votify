using Votify.LógicaNegocio.Enums;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Builder
{
    public class EventoConcretoBuilder : EventoBuilder
    {
        private Evento _evento = new();

        public EventoConcretoBuilder()
        {
            Reset();
        }

        public override void Reset()
        {
            _evento = new Evento
            {
                Estado = "Activo",
                FaseActual = FasesEvento.Inscripciones
            };
        }

        public override EventoBuilder ConIdentificacion(string nombre, string descripcion)
        {
            _evento.Nombre = nombre;
            _evento.Descripcion = descripcion;
            return this;
        }

        public override EventoBuilder ConFechas(
            DateOnly fechaFinInscripciones,
            DateOnly fechaFinEntrega,
            DateOnly fechaFinVotaciones,
            DateOnly fechaFinEvaluacionExperta,
            DateOnly fechaFinVotacionExperta)
        {
            _evento.FechaFinInscripciones = fechaFinInscripciones;
            _evento.FechaFinEntrega = fechaFinEntrega;
            _evento.FechaFinVotaciones = fechaFinVotaciones;
            _evento.FechaFinEvaluacionExperta = fechaFinEvaluacionExperta;
            _evento.FechaFinVotacionExperta = fechaFinVotacionExperta;
            return this;
        }

        public override EventoBuilder ConComentarios(TipoComentario tipoComentario)
        {
            _evento.TipoComentario = tipoComentario;
            return this;
        }

        public override EventoBuilder ConVotacion(
            TipoVotacion tipoVotacion,
            bool participantesVotan,
            int numVotarProyectos,
            int puntosARepartir)
        {
            _evento.TipoVotacion = tipoVotacion;
            _evento.ParticipantesVotan = participantesVotan;
            _evento.NumVotarProyectos = numVotarProyectos;
            _evento.PuntosARepartir = puntosARepartir;
            return this;
        }

        public override EventoBuilder ConGrupos(
            bool tieneGrupos,
            bool gruposCompletos,
            int participantesPorGrupo)
        {
            _evento.TieneGrupos = tieneGrupos;
            _evento.GruposCompletos = gruposCompletos;
            _evento.ParticipantesPorGrupo = participantesPorGrupo;
            return this;
        }

        public override EventoBuilder ConPrivacidad(bool esPrivado, string? passwordAcceso)
        {
            _evento.EsPrivado = esPrivado;
            _evento.PasswordAcceso = esPrivado ? passwordAcceso : null;
            return this;
        }

        public override Evento GetResultado()
        {
            var resultado = _evento;
            Reset();
            return resultado;
        }
    }
}
