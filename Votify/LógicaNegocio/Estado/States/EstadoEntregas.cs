using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Estado.Interfaz;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Estado.States
{
    public class EstadoEntregas : IEstadoEvento
    {
        public FasesEvento Fase => FasesEvento.Entregas;

        public void ManejarFase(Evento evento)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);

            if (hoy > evento.FechaFinEntrega)
            {
                evento.SetEstado(new EstadoVotaciones());
            }
        }

        public bool PuedeEntregarProyecto() => true;
        public bool PuedeVotarPopular() => false;
        public bool PuedeVotarExperto() => false;
        public bool PuedeVerResultados() => false;
        public bool PuedeReclamar() => false;
        public bool PuedeVerResultadosPopularesJurado() => false;
    }
}