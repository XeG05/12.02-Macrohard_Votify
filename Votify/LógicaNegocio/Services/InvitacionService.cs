using Microsoft.EntityFrameworkCore;
using Votify.Auth;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Exceptions;
using Votify.LógicaNegocio.Factory.Roles;
using Votify.LógicaNegocio.Interfaces;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Services
{
    public class InvitacionService : IInvitacionService
    {
        private readonly IRepository<InvitacionPendiente> _invitacionRepo;
        private readonly IRepository<Usuario> _usuarioRepo;
        private readonly ParticipanteCreator _participanteCreator;
        private readonly IUsuarioService _usuarioService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtService _jwtService;

        public InvitacionService(IRepository<InvitacionPendiente> invitacionRepo,
            IRepository<Usuario> usuarioRepo,
            ParticipanteCreator participanteCreator,
            IUsuarioService usuarioService,
            IUnitOfWork unitOfWork,
            JwtService jwtService)
        {
            _invitacionRepo = invitacionRepo;
            _usuarioRepo = usuarioRepo;
            _participanteCreator = participanteCreator;
            _usuarioService = usuarioService;
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
        }

        public async Task<InvitacionPendiente?> ObtenerInvitacionValida(string token)
        {
            var lista = await _invitacionRepo.FindAsync(
                i => i.Token == token && !i.Aceptada && i.FechaExpiracion > DateTime.UtcNow);
            return lista.FirstOrDefault();
        }

        public async Task<InvitacionDto?> ObtenerInvitacionInfoAsync(string token)
        {
            // Cargamos la invitación junto con el evento para obtener su nombre
            var invitacion = await _invitacionRepo.AsQueryable()
                .Include(i => i.IdEventoNavigation)
                .FirstOrDefaultAsync(i => i.Token == token && i.FechaExpiracion > DateTime.UtcNow);

            if (invitacion is null) return null;

            var usuarioExiste = (await _usuarioRepo.FindAsync(u => u.Correo == invitacion.CorreoDestino)).Any();

            return new InvitacionDto
            {
                CorreoDestino = invitacion.CorreoDestino,
                NombreEvento  = invitacion.IdEventoNavigation?.Nombre ?? "",
                UsuarioExiste = usuarioExiste,
                Aceptada      = invitacion.Aceptada
            };
        }

        public async Task<string> AceptarInvitacionAsync(
    string token, string contraseña, string? nombre, string? contraseña2)
        {
            var invitacion = await ObtenerInvitacionValida(token)
                ?? throw new UnauthorizedException("Invitación no válida o caducada.");

            var usuarios = await _usuarioRepo.FindAsync(u => u.Correo == invitacion.CorreoDestino);
            var existe = usuarios.Any();

            Usuario? usuario = existe
                ? await _usuarioService.Login(invitacion.CorreoDestino, contraseña)
                : await _usuarioService.Register(nombre!, invitacion.CorreoDestino, contraseña, contraseña2!);

            if (usuario == null)
                throw new UnauthorizedException("No se pudo autenticar.");

            await _participanteCreator.CrearAsync(
                usuario.Correo,
                invitacion.IdEvento,
                invitacion.IdGrupo);

            invitacion.Aceptada = true;
            await _invitacionRepo.UpdateAsync(invitacion);
            await _unitOfWork.SaveChangesAsync();
            return _jwtService.GenerateToken(usuario);
        }
    }
}
