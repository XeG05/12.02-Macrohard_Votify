using Votify.LógicaNegocio.Estado.Interfaz;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Estado
{
    public class EstadoVotacionExperta : IEstadoEvento
    {
        public void ManejarFase(Evento evento)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);

            if (hoy > evento.FechaFinVotacionExperta)
            {
                evento.SetEstado(new EstadoResultados());
            }
        }

        public override string ToString() => "VotacionExperta";
    }
}
