using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Interfaces;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Observador.Observers
{
    public class ObservadorParticipantes : IObservadorEvento
    {
        private readonly INotificacionService _notificacionService;

        public ObservadorParticipantes(INotificacionService notificacionService)
        {
            _notificacionService = notificacionService;
        }

        public async Task ActualizarAsync(IEventoSujeto sujeto)
        {
            if (sujeto.FaseActual != FasesEvento.Entregas)
            {
                return;
            }

            var evento = sujeto.Evento;

            var participantes = evento.Roles
                .OfType<Participante>()
                .ToList();

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
    }
}