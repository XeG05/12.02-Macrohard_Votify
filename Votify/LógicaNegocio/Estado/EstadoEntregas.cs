using Votify.LógicaNegocio.Estado.Interfaz;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Estado
{
    public class EstadoEntregas : IEstadoEvento
    {
        public void ManejarFase(Evento evento)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);

            if (hoy > evento.FechaFinEntrega)
            {
                evento.SetEstado(new EstadoVotaciones());
            }
        }

        public override string ToString() => "Entregas";
    }
}
