using Votify.LógicaNegocio.Estado.Interfaz;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Estado
{
    public class EstadoVotaciones : IEstadoEvento
    {
        public void ManejarFase(Evento evento)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);

            if (hoy > evento.FechaFinVotaciones)
            {
                evento.SetEstado(new EstadoEvaluacionExperta());
            }
        }

        public override string ToString() => "Votaciones";
    }
}
