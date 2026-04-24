using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Exceptions;
using Votify.LógicaNegocio.Interfaces;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Services
{
    public class EventoAdministradorService : IEventoAdministradorService
    {
        private readonly IRepository<Evento> _eventoRepo;
        private readonly IRepository<Categorium> _categoriumRepo;
        private readonly IRepository<Grupo> _grupoRepo;
        private readonly IRepository<Proyecto> _proyectoRepo;
        private readonly IRepository<Voto> _votoRepo;
        private readonly IRepository<Participante> _participanteRepo;
        private readonly IUnitOfWork _unitOfWork;
        public EventoAdministradorService(
            IRepository<Evento> eventoRepo,
            IRepository<Categorium> categoriumRepo,
            IRepository<Grupo> grupoRepo,
            IRepository<Proyecto> proyectoRepo,
            IRepository<Voto> votoRepo,
            IRepository<Participante> participanteRepo,
            IUnitOfWork unitOfWork)
        {
            _eventoRepo = eventoRepo;
            _categoriumRepo = categoriumRepo;
            _grupoRepo = grupoRepo;
            _proyectoRepo = proyectoRepo;
            _votoRepo = votoRepo;
            _participanteRepo = participanteRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task CambiarNombre(long idEvento, string nuevoNombre)
        {
            var evento = await _eventoRepo.GetByIdAsync(idEvento)
                ?? throw new NotFoundException("Evento no encontrado.");
            evento.Nombre = nuevoNombre;
            await _eventoRepo.UpdateAsync(evento);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CambiarDescripcion(long idEvento, string nuevaDescripcion)
        {
            var evento = await _eventoRepo.GetByIdAsync(idEvento)
                ?? throw new NotFoundException("Evento no encontrado.");
            evento.Descripcion = nuevaDescripcion;
            await _eventoRepo.UpdateAsync(evento);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<Categorium>> VerCategorias(long idEvento)
        => await _categoriumRepo.FindAsync(c => c.IdEvento == idEvento);

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

        public async Task<List<ProyectoResultadosDto>> ObtenerResultadosCategoria(long idCategoria)
        {
            var grupos = await _grupoRepo.FindAsync(g => g.IdCategoria == idCategoria);

            var resultado = new List<ProyectoResultadosDto>();
            foreach (var g in grupos)
            {
                var proyecto = (await _proyectoRepo.FindAsync(p => p.IdGrupo == g.Id)).FirstOrDefault();

                if (proyecto == null)
                    continue;

                var votos = await _votoRepo.FindAsync(v => v.IdProyectoGru == proyecto.Id);

                resultado.Add(new ProyectoResultadosDto
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
        public async Task AñadirGrupo(string nombreGrupo, long idCategoria, long idEvento)
        {

            if (ObtenerFaseActual(idEvento) != FasesEvento.Inscripciones)
                throw new ValidationException("No se pueden añadir grupos fuera de la fase de inscripción");

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
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task EliminarGrupo(long idEvento,long idGrupo)
        {

            if (ObtenerFaseActual(idEvento) != FasesEvento.Inscripciones)
                throw new ValidationException("No se pueden eliminar grupos fuera de la fase de inscripción");

            var grupo = await _grupoRepo.GetByIdAsync(idGrupo) ??
                throw new NotFoundException("Grupo no encontrado");

            var proyectos = await _proyectoRepo.FindAsync(p => p.IdGrupo == idGrupo);

            foreach (var pr in proyectos)
            {
                await _proyectoRepo.DeleteAsync(pr);
            }

            var participantes = await _participanteRepo.FindAsync(p => p.IdGrupo == idGrupo);

            foreach (var p in participantes)
            {
                await _participanteRepo.DeleteAsync(p);
            }

            await _grupoRepo.DeleteAsync(grupo);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ActualizarFechas(long eventoId,
        DateTime finInscripciones,
        DateTime finEntrega,
        DateTime finVotaciones,
        DateTime finExperta)
        {
            DateOnly finInscripcionesOnly = DateOnly.FromDateTime(finInscripciones);
            DateOnly finEntregaOnly = DateOnly.FromDateTime(finEntrega);
            DateOnly finVotacionesOnly = DateOnly.FromDateTime(finVotaciones);
            DateOnly finExpertaOnly = DateOnly.FromDateTime(finExperta);

            var evento = await _eventoRepo.GetByIdAsync(eventoId);

            if (evento == null)
                throw new NotFoundException("Evento no encontrado");

            var hoy = DateOnly.FromDateTime(DateTime.UtcNow);

            //  Validar orden
            if (!(finInscripcionesOnly < finEntregaOnly && finEntregaOnly < finVotacionesOnly && finVotacionesOnly < finExpertaOnly))
                throw new ValidationException("Las fechas deben estar en orden lógico");

            /*//  No fechas pasadas
            if (finInscripcionesOnly < hoy || finEntregaOnly < hoy || finVotacionesOnly < hoy || finExpertaOnly < hoy)
        throw new ValidationException("No puedes poner fechas en el pasado");*/ //POR EL MOMENTO DESHABILITADO PARA HACER PRUEBAS
            //  Restricción por fase
            var faseActual = ObtenerFaseActual(eventoId);

            // 🔹 INSCRIPCIONES → puedes modificar todo
            if (faseActual == FasesEvento.Inscripciones)
            {
                // no bloqueas nada
            }

            // 🔹 ENTREGAS → no puedes tocar inscripciones
            else if (faseActual == FasesEvento.Entregas)
            {
                if (finInscripcionesOnly != evento.FechaFinInscripciones)
                    throw new ValidationException("No puedes modificar la fase de inscripciones");
            }

            // 🔹 VOTACIONES → solo puedes tocar votaciones
            else if (faseActual == FasesEvento.Votaciones)
            {
                if (finInscripcionesOnly != evento.FechaFinInscripciones ||
                    finEntregaOnly != evento.FechaFinEntrega)
                    throw new ValidationException("No puedes modificar la fase de votaciones ni de entregas");
            }

            // 🔹 Evaluacion Experta → solo puedes tocar votaciones
            else if (faseActual == FasesEvento.EvaluacionExperta)
            {
                if (finInscripcionesOnly != evento.FechaFinInscripciones ||
                    finEntregaOnly != evento.FechaFinEntrega ||
                    finVotacionesOnly != evento.FechaFinVotaciones)
                    throw new ValidationException("Solo puedes modificar la fase de evaluación experta");
            }

            // 🔹 FINALIZADO → nada
            else
            {
                throw new ValidationException("El evento ya ha finalizado");
            }

            //  actualizar
            evento.FechaFinInscripciones = finInscripcionesOnly;
            evento.FechaFinEntrega = finEntregaOnly;
            evento.FechaFinVotaciones = finVotacionesOnly;
            evento.FechaFinEvaluacionExperta = finExpertaOnly;

            await _eventoRepo.UpdateAsync(evento);
            await _unitOfWork.SaveChangesAsync();
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
        public FasesEvento ObtenerFaseActual(long idEvento)
        {
            var evento = _eventoRepo.GetByIdAsync(idEvento).Result
                ?? throw new NotFoundException("Evento no encontrado");

            var hoy = DateOnly.FromDateTime(DateTime.Now);
            if (hoy <= evento.FechaFinInscripciones) return FasesEvento.Inscripciones;
            if (hoy <= evento.FechaFinEntrega) return FasesEvento.Entregas;
            if (hoy <= evento.FechaFinVotaciones) return FasesEvento.Votaciones;
            if (hoy <= evento.FechaFinEvaluacionExperta) return FasesEvento.EvaluacionExperta;
            return FasesEvento.Resultados;
        }
        public async Task PausarEvento(long eventoId)
        {
            var evento = await _eventoRepo.GetByIdAsync(eventoId);
            if (evento == null) throw new Exception("Evento no encontrado");

            if (evento.Estado == "Cerrado")
                throw new Exception("No se puede pausar un evento cerrado");

            evento.Estado = "Pausado";
            await _eventoRepo.UpdateAsync(evento);
        }

        public async Task ReanudarEvento(long eventoId)
        {
            var evento = await _eventoRepo.GetByIdAsync(eventoId);
            if (evento == null) throw new Exception("Evento no encontrado");

            if (evento.Estado == "Cerrado")
                throw new Exception("No se puede reanudar un evento cerrado");

            evento.Estado = "Activo";
            await _eventoRepo.UpdateAsync(evento);
        }

        public async Task CerrarEvento(long eventoId)
        {
            var evento = await _eventoRepo.GetByIdAsync(eventoId);
            if (evento == null) throw new Exception("Evento no encontrado");

            evento.Estado = "Cerrado";
            await _eventoRepo.UpdateAsync(evento);
        }
    }
}
