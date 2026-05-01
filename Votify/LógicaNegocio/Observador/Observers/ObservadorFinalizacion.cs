using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Interfaces;

namespace Votify.LógicaNegocio.Observador.Observers
{

    public class ObservadorFinalizacion : IObservadorEvento
    {
       
        private readonly INotificacionService _notificacionService;

       

        public ObservadorFinalizacion(
           
            INotificacionService notificacionService)
        {
            
            _notificacionService = notificacionService;
        }

        public async Task Actualizar(EstadoNotificacionEvento estado)
        {
            
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
