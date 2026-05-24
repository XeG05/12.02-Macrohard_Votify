using System.Diagnostics;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Exceptions;
using Votify.LógicaNegocio.Strategy.Interfaces;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Strategy.Interfaces
{
    public class VotacionSimpleStrategy : IVotacionStrategy
    {
        private readonly IRepository<Voto> _votoRepo;
        private readonly IRepository<Proyecto> _proyectoRepo;
        private readonly IRepository<Grupo> _grupoRepo;
        private readonly IRepository<Criterio> _criterioRepo;
        private readonly IRepository<Rol> _rolRepo;
        private readonly IUnitOfWork _uow;

        public VotacionSimpleStrategy(
            IRepository<Voto> votoRepo,
            IRepository<Proyecto> proyectoRepo,
            IRepository<Grupo> grupoRepo,
            IRepository<Criterio> criterioRepo,
            IRepository<Rol> rolRepo,
            IUnitOfWork uow)
        {
            _votoRepo = votoRepo;
            _proyectoRepo = proyectoRepo;
            _grupoRepo = grupoRepo;
            _criterioRepo = criterioRepo;
            _rolRepo = rolRepo;
            _uow = uow;
        }
        public async Task Votar(Evento evento, long idProyecto, long idCriterio, int? ph, string correo)
        {
            Proyecto proyecto = await _proyectoRepo.GetByIdAsync(idProyecto)
                    ?? throw new NotFoundException("Proyecto no encontrado.");

            Grupo grupo = await _grupoRepo.GetByIdAsync(proyecto.IdGrupo)
                ?? throw new NotFoundException("Grupo no encontrado.");

            Criterio criterio = await _criterioRepo.GetByIdAsync(idCriterio)
                ?? throw new NotFoundException("Criterio no encontrado.");

            if (criterio.IdCategoria != grupo.IdCategoria)
                throw new ValidationException("El criterio no pertenece a la categoría del proyecto.");

            var votosUsuario = await _votoRepo.FindAsync(v =>
                v.IdRol == evento.Id &&
                v.CorreoRol == correo &&
                v.IdCriterio == idCriterio);

            if (votosUsuario.Any(v => v.IdProyectoGru == idProyecto))
                throw new ConflictException("Ya has votado este proyecto en este criterio.");

            var proyectosVotadosEnCategoria = new HashSet<long>();

            foreach (var voto in votosUsuario)
            {
                var proyectoVotado = await _proyectoRepo.GetByIdAsync(voto.IdProyectoGru);
                if (proyectoVotado == null) continue;

                var grupoVotado = await _grupoRepo.GetByIdAsync(proyectoVotado.IdGrupo);
                if (grupoVotado?.IdCategoria == grupo.IdCategoria)
                    proyectosVotadosEnCategoria.Add(voto.IdProyectoGru);
            }

            if (proyectosVotadosEnCategoria.Count >= evento.NumVotarProyectos)
                throw new ConflictException($"Límite de {evento.NumVotarProyectos} votos alcanzado para este criterio.");

            Rol rol = (await _rolRepo.FindAsync(r => r.IdEvento == evento.Id && r.CorreoUsuario == correo)).FirstOrDefault()
                ?? throw new NotFoundException("Rol no encontrado para el usuario en este evento.");

            var nuevoVoto = new Voto(criterio.Id, 1, correo, idProyecto, criterio, proyecto, rol);
            nuevoVoto.IdRol = evento.Id;

            proyecto.Votos ??= new List<Voto>();
            rol.Votos ??= new List<Voto>();

            proyecto.Votos.Add(nuevoVoto);
            rol.Votos.Add(nuevoVoto);

            await _votoRepo.AddAsync(nuevoVoto);
            await _proyectoRepo.UpdateAsync(proyecto);
            await _rolRepo.UpdateAsync(rol);
            await _uow.SaveChangesAsync();
        }

        public async Task<CriterioVotoInfoDto> ObtenerInfoCriterio(Evento evento, Proyecto proyecto, Criterio criterio, List<Voto> votosUsuario)
        {
            var dto = new CriterioVotoInfoDto
            {
                IdCriterio = criterio.Id
            };

            var grupoActual = await _grupoRepo.GetByIdAsync(proyecto.IdGrupo);

            var proyectosVotadosEnCategoria = new HashSet<long>();

            foreach (var voto in votosUsuario)
            {
                var proyectoVotado = await _proyectoRepo.GetByIdAsync(voto.IdProyectoGru);

                if (proyectoVotado == null)
                    continue;

                var grupoVotado = await _grupoRepo.GetByIdAsync(proyectoVotado.IdGrupo);

                if (grupoVotado?.IdCategoria == grupoActual.IdCategoria)
                {
                    proyectosVotadosEnCategoria.Add(voto.IdProyectoGru);
                }
            }

            dto.HaVotado =
                votosUsuario.Any(v => v.IdProyectoGru == proyecto.Id);

            dto.VotosRestantes =
                Math.Max(
                    0,
                    evento.NumVotarProyectos - proyectosVotadosEnCategoria.Count);

            return dto;
        }

        public double CalcularValorCriterio(List<Voto> votos)
        {
            return votos.Count;
        }

        public double CalcularPuntajeTotal(long idPr, List<Criterio> criterios, List<Voto> votosTotales)
        {
            double total = 0;
            var votosProyecto = votosTotales.Where(v => v.IdProyectoGru == idPr).ToList();

            foreach (var crit in criterios)
            {
                var vCrit = votosProyecto.Where(v => v.IdCriterio == crit.Id).ToList();

                double valorCrudo = (double)vCrit.Count;

                total += valorCrudo * ((double)crit.Ponderacion / 100.0);
            }
            return total;
        }
    }
}