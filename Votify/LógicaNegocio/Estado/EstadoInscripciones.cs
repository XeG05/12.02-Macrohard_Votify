using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Estado.Interfaz;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Estado
{
    public class EstadoInscripciones : IEstadoEvento
    {
        public FasesEvento Fase => FasesEvento.Inscripciones;

        public void ManejarFase(Evento evento)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);

            if (hoy > evento.FechaFinInscripciones)
            {
                evento.SetEstado(new EstadoEntregas());
            }
        }

        public bool PuedeEntregarProyecto() => false;
        public bool PuedeVotarPopular() => false;
        public bool PuedeVotarExperto() => false;
        public bool PuedeVerResultados() => false;
        public bool PuedeReclamar() => false;
    }
}