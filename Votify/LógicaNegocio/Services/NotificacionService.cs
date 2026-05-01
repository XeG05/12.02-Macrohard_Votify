using Microsoft.EntityFrameworkCore;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Interfaces;
using Votify.Persistencia;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Services
{

    public class NotificacionService : INotificacionService
    {
        private readonly VotifyContext _context;

        public NotificacionService(VotifyContext context)
        {
            _context = context;
        }

        public async Task CrearSiNoExisteAsync(
            string correo,
            long idEvento,
            TipoNotificacion tipo,
            string titulo,
            string contenido)
        {
            var claveUnica = $"{idEvento}-{tipo}-{correo}";

            var existe = await _context.Notificacions
                .AnyAsync(n => n.ClaveUnica == claveUnica);

            if (existe)
            {
                return;
            }

            var notificacion = new Notificacion
            {
                Correo = correo,
                IdEvento = idEvento,
                Tipo = tipo.ToString(),
                Titulo = titulo,
                Contenido = contenido,
                FechaCreacion = DateTime.UtcNow,
                Leida = false,
                ClaveUnica = claveUnica
            };

            try
            {
                _context.Notificacions.Add(notificacion);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                _context.Entry(notificacion).State = EntityState.Detached;
            }
        }

        public async Task<List<NotificacionDto>> ObtenerPorUsuarioAsync(string correo)
        {
            return await _context.Notificacions
                .Where(n => n.Correo == correo)
                .Include(n => n.Evento)
                .OrderBy(n => n.Leida)
                .ThenByDescending(n => n.FechaCreacion)
                .Select(n => new NotificacionDto
                {
                    Id = n.Id,
                    IdEvento = n.IdEvento,
                    Titulo = n.Titulo,
                    Contenido = n.Contenido,
                    Tipo = n.Tipo,
                    FechaCreacion = n.FechaCreacion,
                    Leida = n.Leida,
                    NombreEvento = n.Evento != null ? n.Evento.Nombre : null
                })
                .ToListAsync();
        }

        public async Task MarcarComoLeidaAsync(long idNotificacion, string correo)
        {
            var notificacion = await _context.Notificacions
                .FirstOrDefaultAsync(n => n.Id == idNotificacion && n.Correo == correo);

            if (notificacion == null)
            {
                return;
            }

            notificacion.Leida = true;

            await _context.SaveChangesAsync();
        }

        public async Task MarcarTodasComoLeidasAsync(string correo)
        {
            var notificaciones = await _context.Notificacions
                .Where(n => n.Correo == correo && !n.Leida)
                .ToListAsync();

            foreach (var notificacion in notificaciones)
            {
                notificacion.Leida = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(long idNotificacion, string correo)
        {
            var notificacion = await _context.Notificacions
                .FirstOrDefaultAsync(n => n.Id == idNotificacion && n.Correo == correo);

            if (notificacion == null)
            {
                return;
            }

            _context.Notificacions.Remove(notificacion);

            await _context.SaveChangesAsync();
        }
    }
}