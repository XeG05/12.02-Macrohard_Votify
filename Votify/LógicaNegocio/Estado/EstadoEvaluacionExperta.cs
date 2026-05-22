using Votify.LógicaNegocio.Estado.Interfaz;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Estado
{
    public class EstadoEvaluacionExperta : IEstadoEvento
    {
        public void ManejarFase(Evento evento)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);

            if (hoy > evento.FechaFinEvaluacionExperta)
            {
                evento.SetEstado(new EstadoVotacionExperta());
            }
        }

        public override string ToString() => "EvaluacionExperta";
    }
}
