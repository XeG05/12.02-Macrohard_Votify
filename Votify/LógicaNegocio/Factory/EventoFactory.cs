using Votify.LógicaNegocio.Domain;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Factory
{
    public class EventoFactory
    {
        private readonly IRepository<Usuario> _usuarioRepo;
        private readonly IRepository<Evento> _eventoRepo;
        private readonly IRepository<Proyecto> _proyectoRepo;
        private readonly IRepository<Participante> _participanteRepo;
        private readonly IRepository<Grupo> _grupoRepo;
        private readonly IRepository<Categorium> _categoriumRepo;
        private readonly IRepository<Voto> _votoRepo;
        private readonly IRepository<Comentario> _comentarioRepo;
        private readonly IRepository<Rol> _rolRepo;
        private readonly IRepository<Criterio> _criterioRepo;

        public EventoFactory(
            IRepository<Usuario> usuarioRepo,
            IRepository<Evento> eventoRepo,
            IRepository<Proyecto> proyectoRepo,
            IRepository<Participante> participanteRepo,
            IRepository<Grupo> grupoRepo,
            IRepository<Categorium> categoriumRepo,
            IRepository<Voto> votoRepo,
            IRepository<Comentario> comentarioRepo,
            IRepository<Rol> rolRepo,
            IRepository<Criterio> criterioRepo)
        {
            _usuarioRepo = usuarioRepo;
            _eventoRepo = eventoRepo;
            _proyectoRepo = proyectoRepo;
            _participanteRepo = participanteRepo;
            _grupoRepo = grupoRepo;
            _categoriumRepo = categoriumRepo;
            _votoRepo = votoRepo;
            _comentarioRepo = comentarioRepo;
            _rolRepo = rolRepo;
            _criterioRepo = criterioRepo;
        }

        public EventoBase CrearEvento(Evento evento, Usuario usuario, Rol rol, long? grupoId = null)
        {
            if (rol.Administrador != null)
                return new EventoAdministrador(
                    evento, usuario,
                    _usuarioRepo, _eventoRepo, _proyectoRepo,
                    _participanteRepo, _categoriumRepo, _grupoRepo, _votoRepo);

            if (rol.Participantes != null && rol.Participantes.Any())
                return new EventoParticipante(
                    evento, usuario,
                    _participanteRepo, _grupoRepo, _proyectoRepo,
                    _categoriumRepo, _usuarioRepo, _votoRepo, _comentarioRepo, grupoId);

            if (rol.Votante != null || rol.Jurado != null)
                return new EventoVotante(
                    evento, usuario,
                    _eventoRepo, _proyectoRepo, _participanteRepo,
                    _categoriumRepo, _grupoRepo, _rolRepo, _criterioRepo,
                    _votoRepo, _comentarioRepo, _usuarioRepo);

            throw new InvalidOperationException(
                $"El usuario '{usuario.Correo}' no tiene un rol reconocido en el evento '{evento.Id}'.");
        }
    }
}
