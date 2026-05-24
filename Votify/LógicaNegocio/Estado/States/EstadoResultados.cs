using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Estado.Interfaz;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Estado.States
{
    public class EstadoResultados : IEstadoEvento
    {
        public FasesEvento Fase => FasesEvento.Resultados;

        public void ManejarFase(Evento evento)
        {
            evento.Estado = "Finalizado";
        }

        public bool PuedeEntregarProyecto() => false;
        public bool PuedeVotarPopular() => false;
        public bool PuedeVotarExperto() => false;
        public bool PuedeVerResultados() => true;
        public bool PuedeReclamar() => true;
    }
}