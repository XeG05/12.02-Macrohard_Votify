using System.Diagnostics;
using Votify.LógicaNegocio.DTOs;
using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Exceptions;
using Votify.LógicaNegocio.Strategy.Interfaces;
using Votify.Persistencia.Entities;
using Votify.Persistencia.Interfaces;

namespace Votify.LógicaNegocio.Strategy.Strategies
{
    public class VotacionNotaStrategy : IVotacionStrategy
    {
        private readonly IRepository<Voto> _votoRepo;
        private readonly IRepository<Proyecto> _proyectoRepo;
        private readonly IRepository<Grupo> _grupoRepo;
        private readonly IRepository<Criterio> _criterioRepo;
        private readonly IRepository<Rol> _rolRepo;
        private readonly IUnitOfWork _uow;

        public VotacionNotaStrategy(
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
        public async Task Votar(Evento evento, long idProyecto, long idCriterio, int? nota, string correo)
        {
            if (nota <= 0)
                throw new ValidationException("La nota debe ser mayor que 0.");

            Proyecto proyecto = await _proyectoRepo.GetByIdAsync(idProyecto)
                ?? throw new NotFoundException("Proyecto no encontrado.");

            Grupo grupo = await _grupoRepo.GetByIdAsync(proyecto.IdGrupo)
                ?? throw new NotFoundException("Grupo no encontrado.");

            Criterio criterio = await _criterioRepo.GetByIdAsync(idCriterio)
                ?? throw new NotFoundException("Criterio no encontrado.");

            if (criterio.IdCategoria != grupo.IdCategoria)
                throw new ValidationException("El criterio no pertenece a la categoría del proyecto.");

            Rol rol = (await _rolRepo.FindAsync(r => r.IdEvento == evento.Id && r.CorreoUsuario == correo)).FirstOrDefault()
                ?? throw new NotFoundException("Rol no encontrado para el usuario en este evento.");

            var votoExistente = (await _votoRepo.FindAsync(v =>
                v.IdRol == evento.Id &&
                v.CorreoRol == correo &&
                v.IdCriterio == idCriterio &&
                v.IdProyectoGru == idProyecto)).FirstOrDefault();

            if (votoExistente != null)
            {
                votoExistente.Nota = nota;
                await _votoRepo.UpdateAsync(votoExistente);
            }
            else
            {
                var nuevoVoto = new Voto(criterio.Id, nota, correo, idProyecto, criterio, proyecto, rol);
                nuevoVoto.IdRol = evento.Id;
                proyecto.Votos ??= new List<Voto>();
                rol.Votos ??= new List<Voto>();
                proyecto.Votos.Add(nuevoVoto);
                rol.Votos.Add(nuevoVoto);
                await _votoRepo.AddAsync(nuevoVoto);
                await _proyectoRepo.UpdateAsync(proyecto);
                await _rolRepo.UpdateAsync(rol);
            }

            await _uow.SaveChangesAsync();
        }

        public async Task<CriterioVotoInfoDto> ObtenerInfoCriterio(Evento evento, Proyecto proyecto, Criterio criterio, List<Voto> votosUsuario)
        {
            var dto = new CriterioVotoInfoDto
            {
                IdCriterio = criterio.Id
            };

            var votoNota = votosUsuario
                .FirstOrDefault(v => v.IdProyectoGru == proyecto.Id);

            dto.HaVotado = votoNota != null;
            dto.NotaDada = votoNota?.Nota;

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

                double valorCrudo = vCrit.Any() ? vCrit.Average(v => (double)(v.Nota ?? 0)) : 0;

                total += valorCrudo * ((double)crit.Ponderacion / 100.0);
            }
            return total;
        }

        public ResultadoCalculoDto CalcularPuntuacionCriterio(
             List<Voto> votosVotantes,
             List<Voto> votosJurados,
             double ponderacion,
             double pesoPopular,
             double pesoExperto)
        {
            double mediaVotantes = votosVotantes.Any() ? votosVotantes.Average(v =>
           (double)(v.Nota ?? 0)) : 0;
            double mediaJurados = votosJurados.Any() ? votosJurados.Average(v => (double)
           (v.Nota ?? 0)) : 0;
            double puntuacion = ponderacion * (
            mediaVotantes * pesoPopular +
            mediaJurados * pesoExperto
            );
            return new ResultadoCalculoDto
            {
                Puntuacion = puntuacion,
                MediaNota = (mediaVotantes + mediaJurados) / 2.0
            };
        }
        public double CalcularValorCrudo(List<Voto> votosCriterio)
        {
            return votosCriterio.Any() ? votosCriterio.Average(v => (double)(v.Nota ??
           0)) : 0;
        }
    }
}