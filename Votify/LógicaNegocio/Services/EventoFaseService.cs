using Microsoft.EntityFrameworkCore;
using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Observador;
using Votify.LógicaNegocio.Observador.Observers;
using Votify.Persistencia;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Services
{

    public class EventoFaseService
    {
        private readonly VotifyContext _context;
        private readonly EventoSujeto _sujeto;

        public EventoFaseService(
            VotifyContext context,
            EventoSujeto sujeto,
            ObservadorParticipantes observadorParticipantes,
            ObservadorVotantes observadorVotantes,
            ObservadorJurado observadorJurado,
            ObservadorFinalizacion observadorFinalizacion)
        {
            _context = context;
            _sujeto = sujeto;

            _sujeto.Attach(observadorParticipantes);
            _sujeto.Attach(observadorVotantes);
            _sujeto.Attach(observadorJurado);
            _sujeto.Attach(observadorFinalizacion);
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
                if (faseNueva == FasesEvento.Resultados)
                {
                    await _sujeto.NotificarCambioFase(
                        evento,
                        faseAnterior,
                        faseNueva);
                }

                return;
            }

            evento.FaseActual = faseNueva;

            await _sujeto.NotificarCambioFase(
                evento,
                faseAnterior,
                faseNueva);
        }

        private async Task RevisarRecordatoriosAsync(Evento evento)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Now);

            if (hoy == evento.FechaFinEntrega.AddDays(-1))
            {
                await _sujeto.NotificarRecordatorio(
                    evento,
                    TipoRecordatorio.FinEntrega);
            }

            if (hoy == evento.FechaFinVotaciones.AddDays(-1))
            {
                await _sujeto.NotificarRecordatorio(
                    evento,
                    TipoRecordatorio.FinVotacionPopular);
            }

            if (hoy == evento.FechaFinEvaluacionExperta.AddDays(-1))
            {
                await _sujeto.NotificarRecordatorio(
                    evento,
                    TipoRecordatorio.FinComentariosJurado);
            }

            if (hoy == evento.FechaFinVotacionExperta.AddDays(-1))
            {
                await _sujeto.NotificarRecordatorio(
                    evento,
                    TipoRecordatorio.FinVotacionExperta);
            }
        }

        private FasesEvento CalcularFaseActual(Evento evento)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Now);

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