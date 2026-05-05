namespace Votify.LógicaNegocio.Interfaces
{
    public interface IObservadorEvento
    {
        Task ActualizarAsync(IEventoSujeto sujeto);
    }
}