using Votify.LógicaNegocio.Enums;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Interfaces
{
    public interface IEventoSujeto
    {
        Evento Evento { get; }

        FasesEvento? FaseAnterior { get; }

        FasesEvento FaseActual { get; }

        void Attach(IObservadorEvento observador);

        void Detach(IObservadorEvento observador);

        Task SetEstadoAsync(Evento evento);

        Task NotificarAsync();
    }
}