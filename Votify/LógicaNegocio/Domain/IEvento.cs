using Votify.LógicaNegocio.Enums;

namespace Votify.LógicaNegocio.Domain
{
    public interface IEvento
    {
        string Nombre { get; }
        string Descripcion { get; }
        FasesEvento ObtenerFaseActual();
    }
}
