using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Estado.Interfaz;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Estado.States
{
    public class EstadoVotacionExperta : IEstadoEvento
    {
        public FasesEvento Fase => FasesEvento.VotacionExperta;

        public void ManejarFase(Evento evento)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);

            if (hoy > evento.FechaFinVotacionExperta)
            {
                evento.SetEstado(new EstadoResultados());
            }
        }

        public bool PuedeEntregarProyecto() => false;
        public bool PuedeVotarPopular() => false;
        public bool PuedeVotarExperto() => true;
        public bool PuedeVerResultados() => false;
        public bool PuedeReclamar() => false;
    }
}