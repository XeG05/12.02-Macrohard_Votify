using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Interfaces;

namespace Votify.LógicaNegocio.Observador.Observers
{
    public class ObservadorFinalizacion : IObservadorEvento
    {
        private readonly INotificacionService _notificacionService;

        public ObservadorFinalizacion(INotificacionService notificacionService)
        {
            _notificacionService = notificacionService;
        }

        public async Task ActualizarAsync(IEventoSujeto sujeto)
        {
            if (sujeto.FaseActual != FasesEvento.Resultados)
            {
                return;
            }

            var evento = sujeto.Evento;

            var correos = evento.Roles
                .Select(r => r.CorreoUsuario)
                .Distinct()
                .ToList();

            foreach (var correo in correos)
            {
                await _notificacionService.CrearSiNoExisteAsync(
                    correo,
                    evento.Id,
                    TipoNotificacion.EventoFinalizado,
                    "El evento ha finalizado",
                    $"El evento \"{evento.Nombre}\" ha finalizado. Ya puedes consultar los resultados."
                );
            }
        }
    }
}