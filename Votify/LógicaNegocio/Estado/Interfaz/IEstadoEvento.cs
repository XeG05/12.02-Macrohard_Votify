using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Estado.Interfaz
{
    public interface IEstadoEvento
    {
        void ManejarFase(Evento evento);
    }
}
