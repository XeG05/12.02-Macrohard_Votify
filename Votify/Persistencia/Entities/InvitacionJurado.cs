namespace Votify.Persistencia.Entities
{
    public class InvitacionJurado
    {
        public long Id { get; set; }
        public string Token { get; set; } = null!;
        public string CorreoDestino { get; set; } = null!;
        public long IdCategoria { get; set; }
        public long IdEvento { get; set; }
        public bool Aceptada { get; set; } = false;
        public DateTime FechaExpiracion { get; set; }
        public string CorreoCreador { get; set; } = null!;
        public virtual Categorium IdCategoriaNavigation { get; set; } = null!;
        public virtual Evento IdEventoNavigation { get; set; } = null!;
    }
}
