using Votify.LógicaNegocio.Estado.Interfaz;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Estado
{
    public class EstadoResultados : IEstadoEvento
    {
        public void ManejarFase(Evento evento)
        {
            // Estado final. No hay transición posible.
            // Marcamos el evento como finalizado
            evento.Estado = "Finalizado";
        }

        public override string ToString() => "Resultados";
    }
}
