using Votify.LógicaNegocio.Enums;
using Votify.Persistencia.Entities;

namespace Votify.LógicaNegocio.Observador
{

    public class EstadoNotificacionEvento
    {
        public Evento Evento { get; set; } = null!;

        public TipoCambioEstado TipoCambio { get; set; }

        public FasesEvento? FaseAnterior { get; set; }

        public FasesEvento? FaseNueva { get; set; }

        public TipoRecordatorio? TipoRecordatorio { get; set; }
    }
}