using Votify.LógicaNegocio.DTOs;

namespace Votify.LógicaNegocio.Interfaces
{
    public interface ICertificadoService
    {
        byte[] Generar(DatosCertificadosDto datos);
    }
}
