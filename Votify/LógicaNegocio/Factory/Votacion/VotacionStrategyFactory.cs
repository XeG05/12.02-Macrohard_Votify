using Votify.LógicaNegocio.Enums;
using Votify.LógicaNegocio.Strategy.Interfaces;
using Votify.LógicaNegocio.Strategy.Strategies;

namespace Votify.LógicaNegocio.Factory.Votacion
{
    public class VotacionStrategyFactory
    {
        private readonly IServiceProvider _provider;

        public VotacionStrategyFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IVotacionStrategy Crear(TipoVotacion tipo)
        {
            return tipo switch
            {
                TipoVotacion.Simple =>
                    _provider.GetRequiredService<VotacionSimpleStrategy>(),

                TipoVotacion.Nota =>
                    _provider.GetRequiredService<VotacionNotaStrategy>(),

                TipoVotacion.Puntos =>
                    _provider.GetRequiredService<VotacionPuntosStrategy>(),

                _ => throw new Exception("Tipo de votación no soportado")
            };
        }
    }
}