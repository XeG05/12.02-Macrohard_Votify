namespace Votify.Modelos
{
    public class InvitacionPendiente
    {
        public long Id { get; set; }
        public string Token { get; set; } = null!;
        public string CorreoDestino { get; set; } = null!;
        public long IdGrupo { get; set; }
        public long IdEvento { get; set; }
        public bool Aceptada { get; set; } = false;
        public DateTime FechaExpiracion { get; set; }
        public string CorreoCreador { get; set; } = null!;
        public virtual Grupo IdGrupoNavigation { get; set; } = null!;
        public virtual Evento IdEventoNavigation { get; set; } = null!;
    }
}
