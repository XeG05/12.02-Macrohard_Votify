using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Interfaces;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Observador.Observers
{
    public class ObservadorParticipantes : IObservadorEvento
    {
        private readonly EventoSujeto _sujeto;
        private readonly INotificacionService _notificacionService;

        private EstadoNotificacionEvento? _estadoObservador;

        public ObservadorParticipantes(
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
                estado.FaseNueva == FasesEvento.Entregas)
            {
                var participantes = evento.Roles.OfType<Participante>().ToList();

                foreach (var participante in participantes)
                {
                    await _notificacionService.CrearSiNoExisteAsync(
                        participante.CorreoUsuario,
                        evento.Id,
                        TipoNotificacion.InicioEntrega,
                        "Ha comenzado la fase de entrega",
                        $"Ya puedes entregar tu proyecto en el evento \"{evento.Nombre}\"."
                    );
                }
            }

            if (estado.TipoCambio == TipoCambioEstado.Recordatorio &&
                estado.TipoRecordatorio == TipoRecordatorio.FinEntrega)
            {
                var participantesSinEntrega = evento.Roles
                .OfType<Participante>()
                .Where(p => p.IdGrupoNavigation == null || p.IdGrupoNavigation.Proyecto == null)
                .ToList();

                foreach (var participante in participantesSinEntrega)
                {
                    await _notificacionService.CrearSiNoExisteAsync(
                        participante.CorreoUsuario,
                        evento.Id,
                        TipoNotificacion.RecordatorioFinEntrega,
                        "La fase de entrega termina mañana",
                        $"Mañana termina el plazo de entrega del evento \"{evento.Nombre}\" y todavía no has entregado tu proyecto."
                    );
                }
            }
        }
    }
}
