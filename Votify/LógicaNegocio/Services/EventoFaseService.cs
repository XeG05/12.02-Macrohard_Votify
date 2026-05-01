using Microsoft.EntityFrameworkCore;
using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Observador;
using Votify.LógicaNegocio.Interfaces;
using Votify.Persistencia;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Services
{

    public class EventoFaseService : IEventoFaseService
    {
        private readonly VotifyContext _context;
        private readonly IEventoSujeto _sujeto;

        public EventoFaseService(
            VotifyContext context,
            IEventoSujeto sujeto,
            IEnumerable<IObservadorEvento> observadores)
        {
            _context = context;
            _sujeto = sujeto;

            foreach (var observador in observadores)
            {
                _sujeto.Attach(observador);
            }
        }

        public async Task RevisarEventosAsync()
        {
            var eventos = await _context.Eventos
                .Include(e => e.Roles)
                    .ThenInclude(r => r.Votos)
                .Include(e => e.Roles)
                    .ThenInclude(r => r.Comentarios)
                .ToListAsync();
            var idsGrupo = eventos
                .SelectMany(e => e.Roles.OfType<Participante>())
                .Select(p => p.IdGrupo)
                .Distinct()
                .ToList();

            await _context.Grupos
                .Where(g => idsGrupo.Contains(g.Id))
                .Include(g => g.Proyecto)
                .LoadAsync();

            foreach (var evento in eventos)
            {
                await RevisarCambioFaseAsync(evento);
                await RevisarRecordatoriosAsync(evento);
            }

            await _context.SaveChangesAsync();
        }

        private async Task RevisarCambioFaseAsync(Evento evento)
        {
            var faseAnterior = evento.FaseActual;
            var faseNueva = CalcularFaseActual(evento);

            if (faseAnterior == faseNueva)
            {
                return;
            }

            evento.FaseActual = faseNueva;

            await _sujeto.Notificar(new EstadoNotificacionEvento
            {
                Evento = evento,
                TipoCambio = TipoCambioEstado.CambioFase,
                FaseAnterior = faseAnterior,
                FaseNueva = faseNueva
            });
        }

        private async Task RevisarRecordatoriosAsync(Evento evento)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);

            if (DebeEnviarRecordatorio(hoy, evento.FechaFinEntrega) &&
                evento.FaseActual == FasesEvento.Entregas)
            {
                await _sujeto.Notificar(new EstadoNotificacionEvento
                {
                    Evento = evento,
                    TipoCambio = TipoCambioEstado.Recordatorio,
                    TipoRecordatorio = TipoRecordatorio.FinEntrega
                });
            }

            if (DebeEnviarRecordatorio(hoy, evento.FechaFinVotaciones) &&
                evento.FaseActual == FasesEvento.Votaciones)
            {
                await _sujeto.Notificar(new EstadoNotificacionEvento
                {
                    Evento = evento,
                    TipoCambio = TipoCambioEstado.Recordatorio,
                    TipoRecordatorio = TipoRecordatorio.FinVotacionPopular
                });
            }

            if (DebeEnviarRecordatorio(hoy, evento.FechaFinEvaluacionExperta) &&
                evento.FaseActual == FasesEvento.EvaluacionExperta)
            {
                await _sujeto.Notificar(new EstadoNotificacionEvento
                {
                    Evento = evento,
                    TipoCambio = TipoCambioEstado.Recordatorio,
                    TipoRecordatorio = TipoRecordatorio.FinComentariosJurado
                });
            }

            if (DebeEnviarRecordatorio(hoy, evento.FechaFinVotacionExperta) &&
                evento.FaseActual == FasesEvento.VotacionExperta)
            {
                await _sujeto.Notificar(new EstadoNotificacionEvento
                {
                    Evento = evento,
                    TipoCambio = TipoCambioEstado.Recordatorio,
                    TipoRecordatorio = TipoRecordatorio.FinVotacionExperta
                });
            }
        }
        private static bool DebeEnviarRecordatorio(DateOnly hoy, DateOnly fechaFin)
        {
            var fechaInicioRecordatorio = fechaFin.AddDays(-1);

            return hoy >= fechaInicioRecordatorio && hoy <= fechaFin;
        }
        private FasesEvento CalcularFaseActual(Evento evento)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);

            if (hoy <= evento.FechaFinInscripciones)
            {
                return FasesEvento.Inscripciones;
            }

            if (hoy <= evento.FechaFinEntrega)
            {
                return FasesEvento.Entregas;
            }

            if (hoy <= evento.FechaFinVotaciones)
            {
                return FasesEvento.Votaciones;
            }

            if (hoy <= evento.FechaFinEvaluacionExperta)
            {
                return FasesEvento.EvaluacionExperta;
            }

            if (hoy <= evento.FechaFinVotacionExperta)
            {
                return FasesEvento.VotacionExperta;
            }

            return FasesEvento.Resultados;
        }
    }
}