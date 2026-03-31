namespace Votify.Modelos.Persistence.Entities
{
    public class ImagenProyecto
    {
        public int Id { get; set; }

        public byte[] Archivo { get; set; } = null!;


        public int ProyectoIdGrupo { get; set; }
        public int ProyectoIdCategoria { get; set; }

        public Proyecto Proyecto { get; set; } = null!;
    }
}
