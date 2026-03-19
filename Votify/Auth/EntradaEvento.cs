using Votify.Modelos.BusinessLogic.Domain;

namespace Votify.Auth
{
    public record EntradaEvento(EventoBase Evento, string Rol);
}