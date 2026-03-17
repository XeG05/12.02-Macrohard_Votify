namespace Votify.Modelos.BusinessLogic.Interfaces



{
    public interface IEventoService : IService<Evento>
    {
        Task<List<Evento>> ObtenerEventos();


    }
}
