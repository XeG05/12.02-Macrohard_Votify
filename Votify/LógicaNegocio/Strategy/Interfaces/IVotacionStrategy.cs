using Votify.LógicaNegocio.DTOs;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Strategy.Interfaces
{
    public interface IVotacionStrategy
    {
        Task Votar(
            Evento evento,
            long idProyecto,
            long idCriterio,
            int? valor,
            string correo);

        Task<CriterioVotoInfoDto> ObtenerInfoCriterio(
            Evento evento,
            Proyecto proyecto,
            Criterio criterio,
            List<Voto> votosUsuario);

        double CalcularValorCriterio(
            List<Voto> votos);

        double CalcularPuntajeTotal(
            long idProyecto,
            List<Criterio> criterios,
            List<Voto> votosTotales);

        ResultadoCalculoDto CalcularPuntuacionCriterio(
             List<Voto> votosVotantes,
             List<Voto> votosJurados,
             double ponderacion,
             double pesoPopular,
             double pesoExperto);
        double CalcularValorCrudo(List<Voto> votosCriterio);
    }
}