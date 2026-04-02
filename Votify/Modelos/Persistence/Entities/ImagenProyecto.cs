namespace Votify.Modelos.Persistence.Entities
{
    public class ImagenProyecto
    {
        public int Id { get; set; }

        public byte[] Archivo { get; set; } = null!;


        public long ProyectoIdGrupo { get; set; }

        public Proyecto Proyecto { get; set; } = null!;
    }
}
