using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Interfaces;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Observador.Observers
{

    public class ObservadorVotantes : IObservadorEvento
    {
        
        private readonly INotificacionService _notificacionService;

        

        public ObservadorVotantes(
            
            INotificacionService notificacionService)
        {
            
            _notificacionService = notificacionService;
        }

        public async Task Actualizar(EstadoNotificacionEvento estado)
        {
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
                        "La votación está a punto de finalizar",
                        $"La votación del evento \"{evento.Nombre}\" está a punto de finalizar y todavía no has votado."
                    );
                }
            }
        }
    }
}