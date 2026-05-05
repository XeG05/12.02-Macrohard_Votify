namespace Votify.LógicaNegocio.Observador.Interfaces
{
    public interface IObservadorEvento
    {
        Task ActualizarAsync(IEventoSujeto sujeto);
    }
}