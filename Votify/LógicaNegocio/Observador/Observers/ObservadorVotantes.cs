using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Interfaces;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Observador.Observers
{

    public class ObservadorVotantes : IObservadorEvento
    {
        private readonly EventoSujeto _sujeto;
        private readonly INotificacionService _notificacionService;

        private EstadoNotificacionEvento? _estadoObservador;

        public ObservadorVotantes(
            EventoSujeto sujeto,
            INotificacionService notificacionService)
        {
            _sujeto = sujeto;
            _notificacionService = notificacionService;
        }

        public async Task Actualizar()
        {
            _estadoObservador = _sujeto.GetEstado();

            var estado = _estadoObservador;
            var evento = estado.Evento;

            if (estado.TipoCambio == TipoCambioEstado.CambioFase &&
                estado.FaseNueva == FasesEvento.Votaciones)
            {
                var votantes = evento.Roles.OfType<Votante>().ToList();

                foreach (var votante in votantes)
                {
                    await _notificacionService.CrearSiNoExisteAsync(
                        votante.CorreoUsuario,
                        evento.Id,
                        TipoNotificacion.InicioVotacionPopular,
                        "Ha comenzado la fase de votación",
                        $"Ya puedes votar en el evento \"{evento.Nombre}\"."
                    );
                }
            }

            if (estado.TipoCambio == TipoCambioEstado.Recordatorio &&
                estado.TipoRecordatorio == TipoRecordatorio.FinVotacionPopular)
            {
                var votantesSinVotar = evento.Roles
                    .OfType<Votante>()
                    .Where(v => !v.Votos.Any())
                    .ToList();

                foreach (var votante in votantesSinVotar)
                {
                    await _notificacionService.CrearSiNoExisteAsync(
                        votante.CorreoUsuario,
                        evento.Id,
                        TipoNotificacion.RecordatorioFinVotacionPopular,
                        "La votación termina mañana",
                        $"Mañana termina la votación del evento \"{evento.Nombre}\" y todavía no has votado."
                    );
                }
            }
        }
    }
}