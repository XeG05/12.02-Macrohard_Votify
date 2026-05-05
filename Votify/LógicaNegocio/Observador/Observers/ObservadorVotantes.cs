using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Interfaces;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Observador.Observers
{
    public class ObservadorVotantes : IObservadorEvento
    {
        private readonly INotificacionService _notificacionService;

        public ObservadorVotantes(INotificacionService notificacionService)
        {
            _notificacionService = notificacionService;
        }

        public async Task ActualizarAsync(IEventoSujeto sujeto)
        {
            if (sujeto.FaseActual != FasesEvento.Votaciones)
            {
                return;
            }

            var evento = sujeto.Evento;

            var votantes = evento.Roles
                .OfType<Votante>()
                .ToList();

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
    }
}