namespace Votify.Persistencia.Entities;

public class SintesisJuradoCriterio
{
    public long Id { get; set; }
    public long IdProyecto { get; set; }
    public long IdCriterio { get; set; }
    public string Sintesis { get; set; } = string.Empty;

    public virtual Proyecto Proyecto { get; set; } = null!;
}