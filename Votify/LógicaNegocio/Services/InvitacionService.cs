using Votify.Auth;
using Votify.LógicaNegocio.Factory.Roles;
using Votify.LógicaNegocio.Interfaces;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Services
{
    public class InvitacionService : IInvitacionService
    {
        private readonly IRepository<InvitacionPendiente> _invitacionRepo;
        private readonly IRepository<Evento> _eventoRepo;
        private readonly IRepository<Usuario> _usuarioRepo;
        private readonly ParticipanteCreator _participanteCreator;
        private readonly UsuarioService _usuarioService;
        private readonly JwtService _jwtService;

        public InvitacionService(IRepository<InvitacionPendiente> invitacionRepo,
            IRepository<Evento> eventoRepo,
            IRepository<Usuario> usuarioRepo,
            ParticipanteCreator participanteCreator,
            UsuarioService usuarioService,
            JwtService jwtService)
        {
            _invitacionRepo = invitacionRepo;
            _eventoRepo = eventoRepo;
            _usuarioRepo = usuarioRepo;
            _participanteCreator = participanteCreator;
            _usuarioService = usuarioService;
            _jwtService = jwtService;
        }

        public async Task<InvitacionPendiente?> ObtenerInvitacionValida(string token)
        {
            var lista = await _invitacionRepo.FindAsync(
                i => i.Token == token && !i.Aceptada && i.FechaExpiracion > DateTime.UtcNow);
            return lista.FirstOrDefault();
        }

        public async Task<string> AceptarInvitacionAsync(
            string token, string contraseña, string? nombre, string? contraseña2)
        {
            var invitacion = await ObtenerInvitacionValida(token)
                ?? throw new Exception("Invitación no válida o caducada.");

            var usuarios = await _usuarioRepo.FindAsync(u => u.Correo == invitacion.CorreoDestino);
            var existe = usuarios.Any();

            Usuario? usuario = existe
                ? await _usuarioService.Login(invitacion.CorreoDestino, contraseña)
                : await _usuarioService.Register(nombre!, invitacion.CorreoDestino, contraseña, contraseña2!);

            if (usuario == null)
                throw new Exception("No se pudo autenticar.");

            await _participanteCreator.AsignarAsync(
                usuario.Correo,
                invitacion.IdEvento,
                invitacion.IdGrupo);

            invitacion.Aceptada = true;
            await _invitacionRepo.UpdateAsync(invitacion);

            return _jwtService.GenerateToken(usuario);
        }
    }
}
