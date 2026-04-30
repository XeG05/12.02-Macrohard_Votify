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
        private readonly IRepository<InvitacionJurado> _invitacionJuradoRepo;
        private readonly IRepository<Usuario> _usuarioRepo;
        private readonly IRepository<Categorium> _categoriumRepo;
        private readonly IRepository<Jurado> _juradoRepo;
        private readonly ParticipanteCreator _participanteCreator;
        private readonly JuradoCreator _juradoCreator;
        private readonly IUsuarioService _usuarioService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtService _jwtService;

        public InvitacionService(IRepository<InvitacionPendiente> invitacionRepo,
            IRepository<InvitacionJurado> invitacionJuradoRepo,
            IRepository<Usuario> usuarioRepo,
            IRepository<Categorium> categoriumRepo,
            IRepository<Jurado> juradoRepo,
            ParticipanteCreator participanteCreator,
            JuradoCreator juradoCreator,
            IUsuarioService usuarioService,
            IUnitOfWork unitOfWork,
            JwtService jwtService)
        {
            _invitacionRepo = invitacionRepo;
            _invitacionJuradoRepo = invitacionJuradoRepo;
            _usuarioRepo = usuarioRepo;
            _categoriumRepo = categoriumRepo;
            _juradoRepo = juradoRepo;
            _participanteCreator = participanteCreator;
            _juradoCreator = juradoCreator;
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

        public async Task<InvitacionDto?> ObtenerInvitacionJuradoInfoAsync(string token)
        {
            var invitacion = await _invitacionJuradoRepo.AsQueryable()
                .Include(i => i.IdEventoNavigation)
                .FirstOrDefaultAsync(i => i.Token == token && i.FechaExpiracion > DateTime.UtcNow);

            if (invitacion is null) return null;

            var usuarioExiste = (await _usuarioRepo.FindAsync(u => u.Correo == invitacion.CorreoDestino)).Any();

            return new InvitacionDto
            {
                CorreoDestino = invitacion.CorreoDestino,
                NombreEvento = invitacion.IdEventoNavigation?.Nombre ?? "",
                UsuarioExiste = usuarioExiste,
                Aceptada = invitacion.Aceptada
            };
        }

        public async Task<string> AceptarInvitacionJuradoAsync(
            string token, string contraseña, string? nombre, string? contraseña2)
        {
            var invitacion = await _invitacionJuradoRepo.AsQueryable()
                .FirstOrDefaultAsync(i => i.Token == token && !i.Aceptada && i.FechaExpiracion > DateTime.UtcNow)
                ?? throw new UnauthorizedException("Invitación de jurado no válida o caducada.");

            var usuarios = await _usuarioRepo.FindAsync(u => u.Correo == invitacion.CorreoDestino);
            var existe = usuarios.Any();

            Usuario? usuario = existe
                ? await _usuarioService.Login(invitacion.CorreoDestino, contraseña)
                : await _usuarioService.Register(nombre!, invitacion.CorreoDestino, contraseña, contraseña2!);

            if (usuario == null)
                throw new UnauthorizedException("No se pudo autenticar.");

            // Crear rol Jurado si no existe ya en este evento
            var jurados = await _juradoRepo.FindAsync(
                j => j.CorreoUsuario == usuario.Correo && j.IdEvento == invitacion.IdEvento);
            var juradoExistente = jurados.FirstOrDefault();

            Jurado jurado;
            if (juradoExistente == null)
                jurado = (Jurado)await _juradoCreator.CrearAsync(usuario.Correo, invitacion.IdEvento);
            else
                jurado = juradoExistente;

            // Vincular jurado a la categoría
            var categoria = await _categoriumRepo.AsQueryable()
                .Include(c => c.Jurados)
                .FirstOrDefaultAsync(c => c.Id == invitacion.IdCategoria)
                ?? throw new NotFoundException("Categoría no encontrada.");

            if (!categoria.Jurados.Any(j => j.CorreoUsuario == jurado.CorreoUsuario && j.IdEvento == jurado.IdEvento))
            {
                categoria.Jurados.Add(jurado);
                await _categoriumRepo.UpdateAsync(categoria);
            }

            invitacion.Aceptada = true;
            await _invitacionJuradoRepo.UpdateAsync(invitacion);
            await _unitOfWork.SaveChangesAsync();
            return _jwtService.GenerateToken(usuario);
        }
    }
}
