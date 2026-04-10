using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Enums;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Domain
{
    public class EventoAdministrador : EventoBase
    {
        private readonly IRepository<Usuario> _usuarioRepo;
        private readonly IRepository<Proyecto> _proyectoRepo;
        private readonly IRepository<Evento> _eventoRepo;
        private readonly IRepository<Participante> _participanteRepo;
        private readonly IRepository<Grupo> _grupoRepo;
        private readonly IRepository<Categorium> _categoriumRepo;
        private readonly IRepository<Voto> _votoRepo;
        public EventoAdministrador(Evento evento, 
            Usuario usuario,
            IRepository<Usuario> usuarioRepo,
            IRepository<Evento> eventoRepo, 
            IRepository<Proyecto> proyectoRepo, 
            IRepository<Participante> participanteRepo,
            IRepository<Categorium> categoriumRepo,
            IRepository<Grupo> grupoRepo,
            IRepository<Voto> votoRepo) : base(evento, usuario)
        {
            _usuarioRepo = usuarioRepo;
            _proyectoRepo = proyectoRepo;
            _eventoRepo = eventoRepo;
            _participanteRepo = participanteRepo;
            _grupoRepo = grupoRepo;
            _categoriumRepo = categoriumRepo;
            _votoRepo = votoRepo;
        }

        public async Task CambiarNombre(string nuevoNombre)
        {
            evento.Nombre = nuevoNombre;
            await _eventoRepo.UpdateAsync(evento);
        }

        public async Task CambiarDescripcion(string nuevaDescripcion)
        {
            evento.Descripcion = nuevaDescripcion;
            await _eventoRepo.UpdateAsync(evento);
        }

        public async Task<List<Categorium>> VerCategoria()
        {
            return await _categoriumRepo.FindAsync(c => c.IdEvento == evento.Id);
        }

        public async Task<List<Grupo>> VerGrupo(long idCategoria)
        {
            return await _grupoRepo.FindAsync(g => g.IdCategoria == idCategoria);
            /*
            var proyectos = await _proyectoRepo.FindAsync(p => p.IdCategoria == idCategoria);

            var grupos = new List<Grupo>();

            foreach (var p in proyectos)
            {
                var grupo = await _grupoRepo.GetByIdAsync(p.IdGrupo);

                if (grupo != null && !grupos.Any(g => g.Id == grupo.Id)) //evitar repetidos
                {
                    grupos.Add(grupo);
                }
            }
            
            return grupos;*/

        }

        public async Task<List<Proyecto>> VerProyectoDeGrupo(long idGrupo)
        {
            return await _proyectoRepo.FindAsync(p => p.IdGrupo == idGrupo);
        }

        public async Task<List<Participante>> VerParticipantes(long idGrupo)
        {
            return await _participanteRepo.FindAsync(p => p.IdGrupo == idGrupo);
        }

        public async Task<List<Proyecto>> VerProyectosCategoria(long idCategoria)
        {
            var grupos = await _grupoRepo.FindAsync(g => g.IdCategoria == idCategoria);

            var idsGrupos = grupos.Select(g => g.Id).ToList();

            var proyectos = await _proyectoRepo.FindAsync(p => idsGrupos.Contains(p.IdGrupo));

            return proyectos;
        }
        public async Task<int> ContarVotosProyecto(long idProyecto)
        {
            var votos = await _votoRepo.FindAsync(v => v.IdProyectoGru == idProyecto);
            return votos.Count;
        }

        public async Task<List<ProyectoResumenDTO>> ObtenerResultadosCategoria(long idCategoria)
        {
            var grupos = await _grupoRepo.FindAsync(g => g.IdCategoria == idCategoria);

            var resultado = new List<ProyectoResumenDTO>();

            foreach (var g in grupos)
            {
                var proyecto = (await _proyectoRepo.FindAsync(p => p.IdGrupo == g.Id)).FirstOrDefault();

                if (proyecto == null)
                    continue;

                var votos = await _votoRepo.FindAsync(v => v.IdProyectoGru == proyecto.Id);

                resultado.Add(new ProyectoResumenDTO
                {
                    NombreProyecto = proyecto.Nombre,
                    NombreGrupo = g.Nombre,
                    CantidadVotos = votos.Count
                });
            }

            return resultado;
        }

        public async Task<int> TotalVotosCategoria(long idCategoria)
        {
            var grupos = await _grupoRepo.FindAsync(g => g.IdCategoria == idCategoria);
            var idsGrupos = grupos.Select(g => g.Id).ToList();

            var proyectos = await _proyectoRepo.FindAsync(p => idsGrupos.Contains(p.IdGrupo));
            var idsProyectos = proyectos.Select(p => p.Id).ToList();

            var votos = await _votoRepo.FindAsync(v => idsProyectos.Contains(v.IdProyectoGru));

            return votos.Count;
        }
        public async Task AñadirGrupo(string nombreGrupo, long idCategoria)
        {
            if (ObtenerFaseActual() != FasesEvento.Inscripciones)
                throw new Exception("No se pueden añadir grupos fuera de la fase de inscripción");

            var grupo = new Grupo
            {
                Nombre = nombreGrupo,
                HojaRutaTexto = ""
            };

            await _grupoRepo.AddAsync(grupo);

            var proyecto = new Proyecto
            {
                Nombre = "Sin nombre",
                IdGrupo = grupo.Id,
                Archivo = Array.Empty<byte>()
            };

            await _proyectoRepo.AddAsync(proyecto);
        }
        public async Task EliminarGrupo(long idGrupo)
        {
            if (ObtenerFaseActual() != FasesEvento.Inscripciones)
                throw new Exception("No se pueden eliminar grupos fuera de la fase de inscripción");

            var grupo = await _grupoRepo.GetByIdAsync(idGrupo);

            if (grupo == null)
                throw new Exception("Grupo no encontrado");

            //eliminar proyectos asociados a ese grupo
            var proyectos = await _proyectoRepo.FindAsync(p => p.IdGrupo == idGrupo);

            foreach (var pr in proyectos)
            {
                await _proyectoRepo.DeleteAsync(pr);
            }
            // eliminar participantes asociados a ese grupo
            var participantes = await _participanteRepo.FindAsync(p => p.IdGrupo == idGrupo);

            foreach (var p in participantes)
            {
                await _participanteRepo.DeleteAsync(p);
            }

            await _grupoRepo.DeleteAsync(grupo);
        }
        /*
        public async Task AñadirParticipante(string correo, long idGrupo)
        {
            if (ObtenerFaseActual() != FasesEvento.Inscripciones)
                throw new Exception("Solo se pueden añadir participantes en fase de inscripciones");

            var usuario = (await _usuarioRepo.FindAsync(u => u.Correo == correo))
                .FirstOrDefault();

            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            var participante = new Participante
            {
                Correo = correo,
                IdGrupo = idGrupo
            };

            await _participanteRepo.AddAsync(participante);
        }
        public async Task EliminarParticipante(long idParticipante)
        {
            if (ObtenerFaseActual() != FasesEvento.Inscripciones)
                throw new Exception("Solo se pueden eliminar participantes en fase de inscripciones");

            var participante = await _participanteRepo.GetByIdAsync(idParticipante);

            if (participante == null)
                throw new Exception("Participante no encontrado");

            await _participanteRepo.DeleteAsync(participante);
        }*/ //PARA EL SPRINT 2
    }
}
