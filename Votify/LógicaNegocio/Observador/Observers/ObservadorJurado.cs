using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Interfaces;
using Votify.LógicaNegocio.Observador.Interfaces;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Observador.Observers
{
    public class ObservadorJurado : IObservadorEvento
    {
        private readonly INotificacionService _notificacionService;

        public ObservadorJurado(INotificacionService notificacionService)
        {
            _notificacionService = notificacionService;
        }

        public async Task ActualizarAsync(IEventoSujeto sujeto)
        {
            if (sujeto.FaseActual == FasesEvento.EvaluacionExperta)
            {
                await NotificarInicioComentariosJurado(sujeto.Evento);
            }

            if (sujeto.FaseActual == FasesEvento.VotacionExperta)
            {
                await NotificarInicioVotacionExperta(sujeto.Evento);
            }
        }

        private async Task NotificarInicioComentariosJurado(Evento evento)
        {
            var jurados = evento.Roles
                .OfType<Jurado>()
                .ToList();

            foreach (var jurado in jurados)
            {
                await _notificacionService.CrearSiNoExisteAsync(
                    jurado.CorreoUsuario,
                    evento.Id,
                    TipoNotificacion.InicioComentariosJurado,
                    "Ha comenzado la fase de comentarios del jurado",
                    $"Ya puedes revisar y comentar los proyectos del evento \"{evento.Nombre}\"."
                );
            }
        }

        private async Task NotificarInicioVotacionExperta(Evento evento)
        {
            var jurados = evento.Roles
                .OfType<Jurado>()
                .ToList();

            foreach (var jurado in jurados)
            {
                await _notificacionService.CrearSiNoExisteAsync(
                    jurado.CorreoUsuario,
                    evento.Id,
                    TipoNotificacion.InicioVotacionExperta,
                    "Ha comenzado la votación experta",
                    $"Ya puedes votar los proyectos del evento \"{evento.Nombre}\"."
                );
            }
        }
    }
}