using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Interfaces;

namespace Votify.LógicaNegocio.Observador.Observers
{

    public class ObservadorFinalizacion : IObservadorEvento
    {
        private readonly EventoSujeto _sujeto;
        private readonly INotificacionService _notificacionService;

        private EstadoNotificacionEvento? _estadoObservador;

        public ObservadorFinalizacion(
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
                estado.FaseNueva == FasesEvento.Resultados)
            {
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
}
