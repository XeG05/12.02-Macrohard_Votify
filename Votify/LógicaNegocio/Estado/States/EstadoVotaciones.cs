using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Estado.Interfaz;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Estado.States
{
    public class EstadoVotaciones : IEstadoEvento
    {
        public FasesEvento Fase => FasesEvento.Votaciones;

        public void ManejarFase(Evento evento)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);

            if (hoy > evento.FechaFinVotaciones)
            {
                evento.SetEstado(new EstadoEvaluacionExperta());
            }
        }

        public bool PuedeEntregarProyecto() => false;
        public bool PuedeVotarPopular() => true;
        public bool PuedeVotarExperto() => false;
        public bool PuedeVerResultados() => false;
        public bool PuedeReclamar() => false;
        public bool PuedeVerResultadosPopularesJurado() => true;
    }
}

