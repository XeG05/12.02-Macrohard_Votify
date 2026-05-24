using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Estado.Interfaz;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Estado.States
{
    public class EstadoEvaluacionExperta : IEstadoEvento
    {
        public FasesEvento Fase => FasesEvento.EvaluacionExperta;

        public void ManejarFase(Evento evento)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);

            if (hoy > evento.FechaFinEvaluacionExperta)
            {
                evento.SetEstado(new EstadoVotacionExperta());
            }
        }

        public bool PuedeEntregarProyecto() => false;
        public bool PuedeVotarPopular() => false;
        public bool PuedeVotarExperto() => false;
        public bool PuedeVerResultados() => false;
        public bool PuedeReclamar() => false;
    }
}