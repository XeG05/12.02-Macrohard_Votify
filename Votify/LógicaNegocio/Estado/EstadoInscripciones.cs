using Votify.LógicaNegocio.Estado.Interfaz;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Estado
{
    public class EstadoInscripciones : IEstadoEvento
    {
        public void ManejarFase(Evento evento)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);

            // Si la fecha de inscripciones ha pasado, avanzamos al siguiente estado.
            if (hoy > evento.FechaFinInscripciones)
            {
                evento.SetEstado(new EstadoEntregas());
            }
        }

        public override string ToString() => "Inscripciones";
    }
}
