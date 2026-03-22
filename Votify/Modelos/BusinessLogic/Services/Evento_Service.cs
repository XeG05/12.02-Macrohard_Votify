using Votify.Auth;
using Votify.Modelos.BusinessLogic.Interfaces;
using Votify.Modelos.Persistence.Interfaces;
using Votify.Modelos.BusinessLogic.Domain;
using Votify.Modelos.BusinessLogic.Factory;
using Votify.Modelos.BusinessLogic.DTOs;

namespace Votify.Modelos.BusinessLogic.Services
{
    public class Evento_Service : Service<Evento>, I_Evento_Service
    {
        private readonly IRepository<Evento> _eventoRepo;
        private readonly IRepository<Usuario> _usuarioRepo;
        private readonly IRepository<Proyecto> _proyectoRepo;
        private readonly IRepository<Participante> _participanteRepo;
        private readonly IRepository<Grupo> _grupoRepo;
        private readonly IRepository<Categorium> _categoriumRepo;
        private readonly IRepository<Administrador> _adminRepo;
        private readonly IRepository<Jurado> _juradoRepo;
        private readonly IRepository<Votante> _votanteRepo;
        private readonly IRepository<Rol> _rolRepo;
        private readonly IRepository<Criterio> _criterioRepo;
        private readonly IRepository<Voto> _votoRepo;
        private readonly IRepository<Comentario> _comentarioRepo;
        public Evento_Service(IRepository<Evento> eventoRepo, 
            IRepository<Usuario> usuarioRepo, 
            IRepository<Proyecto> proyectoRepo,
            IRepository<Participante> participanteRepo,
            IRepository<Grupo> grupoRepo, 
            IRepository<Categorium> categoriumRepo,
            IRepository<Administrador> adminRepo,
            IRepository<Jurado> juradoRepo,
            IRepository<Votante> votanteRepo,
            IRepository<Rol> rolRepo,
            IRepository<Criterio> criterioRepo,
            IRepository<Voto> votoRepo,
            IRepository<Comentario> comentarioRepo) : base(eventoRepo)
        {
            _eventoRepo = eventoRepo;
            _usuarioRepo = usuarioRepo;
            _proyectoRepo = proyectoRepo;
            _participanteRepo = participanteRepo;
            _grupoRepo = grupoRepo;
            _categoriumRepo = categoriumRepo;
            _adminRepo = adminRepo;
            _juradoRepo = juradoRepo;
            _votanteRepo = votanteRepo;
            _rolRepo = rolRepo;
            _criterioRepo = criterioRepo;
            _votoRepo = votoRepo;
            _comentarioRepo = comentarioRepo;
        }

        public async Task<EntradaEvento> EntrarEvento(string correo, long eventoId)
        {
            var usuario = (await _usuarioRepo
                .FindAsync(u => u.Correo == correo))
                .FirstOrDefault()
                ?? throw new Exception("Usuario no encontrado");

            var evento = await _eventoRepo.GetByIdAsync(eventoId)
                ?? throw new Exception("Evento no encontrado");

            var tipoRol = await DetectarRol(correo, eventoId);

            EventoCreator creator = tipoRol switch
            {
                "ADMIN" => new EventoAdministradorCreator(
                    _eventoRepo, _proyectoRepo,
                    _participanteRepo, _grupoRepo, _categoriumRepo, _votoRepo),

                "PARTICIPANTE" => new EventoParticipanteCreator(_participanteRepo,
            _grupoRepo,
            _proyectoRepo,
            _categoriumRepo,
            _usuarioRepo),
                /*
                "VOTANTE" => new EventoVotanteCreator(),

                // Jurado puede reutilizar VotanteCreator o tener el suyo propio
                "JURADO" => new EventoVotanteCreator(),*/

                _ => throw new Exception(
                    $"No tienes ningún rol asignado en el evento {eventoId}")
            };

            var eventoBase = creator.CrearEvento(evento, usuario);
            return new EntradaEvento(eventoBase, tipoRol);
        }

        private async Task<string> DetectarRol(string correo, long idEvento)
        {
            var admin = await _adminRepo
                .FindAsync(a => a.Correo == correo && a.Id == idEvento);
            if (admin.Any()) return "ADMIN";

            var jurado = await _juradoRepo
                .FindAsync(j => j.Correo == correo && j.Id == idEvento);
            if (jurado.Any()) return "JURADO";

            var participante = await _participanteRepo
                .FindAsync(p => p.Correo == correo && p.Id == idEvento);
            if (participante.Any()) return "PARTICIPANTE";

            var votante = await _votanteRepo
                .FindAsync(v => v.Correo == correo && v.Id == idEvento);
            if (votante.Any()) return "VOTANTE";

            throw new Exception("No tienes ningún rol asignado en este evento");
        }

        public async Task<Evento> CrearEvento(string correoCreador, EventoCreacionDto dto)
        {
            var (esValido, error) = dto.Validar();
            if (!esValido) throw new Exception(error);

            var usuario = (await _usuarioRepo.FindAsync(u => u.Correo == correoCreador))
                .FirstOrDefault()
                ?? throw new Exception("Usuario no encontrado");

            var todos = await _eventoRepo.GetAllAsync();
            long nuevoId = todos.Any() ? todos.Max(e => e.Id) + 1 : 1;

            var evento = new Evento(
                dto.Nombre,
                dto.Descripcion,
                dto.FechaFinInscripciones!.Value,
                dto.FechaFinEntrega!.Value,
                dto.FechaFinVotaciones!.Value,
                dto.TieneGrupos,
                dto.GruposCompletos,
                dto.NumParticipantes
            )
            {
            };

            await _eventoRepo.AddAsync(evento);

            var rol = new Rol
            {
                CorreoUsuario = correoCreador,
                IdEvento = nuevoId,
                CorreoUsuarioNavigation = usuario,
                IdEventoNavigation = evento
            };

            await _rolRepo.AddAsync(rol);


            var admin = new Administrador
            {
                Correo = correoCreador,
                Id = nuevoId,
                Rol = rol
            };

            await _adminRepo.AddAsync(admin);

            return evento;
        }
    }
}
