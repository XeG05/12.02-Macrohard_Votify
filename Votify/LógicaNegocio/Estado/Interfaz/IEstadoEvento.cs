using Votify.LógicaNegocio.Enums;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Estado.Interfaz
{
    public interface IEstadoEvento
    {
        FasesEvento Fase { get; }

        void ManejarFase(Evento evento);

        bool PuedeEntregarProyecto();
        bool PuedeVotarPopular();
        bool PuedeVotarExperto();
        bool PuedeVerResultados();
        bool PuedeReclamar();
        bool PuedeVerResultadosPopularesJurado();
    }
}

