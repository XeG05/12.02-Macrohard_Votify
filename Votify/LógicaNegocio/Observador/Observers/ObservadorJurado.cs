using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Interfaces;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Observador.Observers
{

    public class ObservadorJurado : IObservadorEvento
    {
        
        private readonly INotificacionService _notificacionService;

        

        public ObservadorJurado(
            
            INotificacionService notificacionService)
        {
            
            _notificacionService = notificacionService;
        }

        public async Task Actualizar(EstadoNotificacionEvento estado)
        {
            
            var evento = estado.Evento;

            if (estado.TipoCambio == TipoCambioEstado.CambioFase &&
                estado.FaseNueva == FasesEvento.EvaluacionExperta)
            {
                await NotificarInicioComentariosJurado(evento);
            }

            if (estado.TipoCambio == TipoCambioEstado.Recordatorio &&
                estado.TipoRecordatorio == TipoRecordatorio.FinComentariosJurado)
            {
                await NotificarFinComentariosJurado(evento);
            }

            if (estado.TipoCambio == TipoCambioEstado.CambioFase &&
                estado.FaseNueva == FasesEvento.VotacionExperta)
            {
                await NotificarInicioVotacionExperta(evento);
            }

            if (estado.TipoCambio == TipoCambioEstado.Recordatorio &&
                estado.TipoRecordatorio == TipoRecordatorio.FinVotacionExperta)
            {
                await NotificarFinVotacionExperta(evento);
            }
        }

        private async Task NotificarInicioComentariosJurado(Evento evento)
        {
            var jurados = evento.Roles.OfType<Jurado>().ToList();

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

        private async Task NotificarFinComentariosJurado(Evento evento)
        {
            var juradosSinComentar = evento.Roles
                .OfType<Jurado>()
                .Where(j => !j.Comentarios.Any())
                .ToList();

            foreach (var jurado in juradosSinComentar)
            {
                await _notificacionService.CrearSiNoExisteAsync(
                    jurado.CorreoUsuario,
                    evento.Id,
                    TipoNotificacion.RecordatorioFinComentariosJurado,
                   "La fase de comentarios está a punto de finalizar",
                    $"La fase de comentarios del evento \"{evento.Nombre}\" está a punto de finalizar y todavía no has comentado los proyectos."
                );
            }
        }

        private async Task NotificarInicioVotacionExperta(Evento evento)
        {
            var jurados = evento.Roles.OfType<Jurado>().ToList();

            foreach (var jurado in jurados)
            {
                await _notificacionService.CrearSiNoExisteAsync(
                    jurado.CorreoUsuario,
                    evento.Id,
                    TipoNotificacion.InicioVotacionExperta,
                    "Ha comenzado la votación experta",
                    $"Ya puedes votar los proyectos del evento \"{evento.Nombre}\" teniendo en cuenta los comentarios realizados."
                );
            }
        }

        private async Task NotificarFinVotacionExperta(Evento evento)
        {
            var juradosSinVotar = evento.Roles
                .OfType<Jurado>()
                .Where(j => !j.Votos.Any())
                .ToList();

            foreach (var jurado in juradosSinVotar)
            {
                await _notificacionService.CrearSiNoExisteAsync(
                    jurado.CorreoUsuario,
                    evento.Id,
                    TipoNotificacion.RecordatorioFinVotacionExperta,
                   "La votación experta está a punto de finalizar",
                    $"La votación experta del evento \"{evento.Nombre}\" está a punto de finalizar y todavía no has votado."
                );
            }
        }
    }
}
