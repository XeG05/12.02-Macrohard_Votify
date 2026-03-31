namespace Votify.Modelos.Persistence.Entities
{
    public class ImagenProyecto
    {
        public long Id { get; set; }

        public string Url { get; set; } = "";

        public long ProyectoIdGrupo { get; set; }
        public long ProyectoIdCategoria { get; set; }

        public Proyecto Proyecto { get; set; }
    }
}
