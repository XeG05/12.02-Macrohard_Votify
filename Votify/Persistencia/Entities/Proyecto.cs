namespace Votify.Persistencia.Entities;

public class Proyecto
{
    public long Id { get; set; }
    public string Nombre { get; set; } = null!;

    public long IdGrupo { get; set; }

    public string Descripcion { get; set; } = "";

    public string DescripcionBreve { get; set; } = "";

    public List<ImagenProyecto> Imagenes { get; set; } = new();

    public byte[] Archivo { get; set; } = null!;

    public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

    public virtual Grupo IdGrupoNavigation { get; set; } = null!;

    public virtual ICollection<Voto> Votos { get; set; } = new List<Voto>();

    public Proyecto()
    {
        Comentarios = new List<Comentario>();
        Votos = new List<Voto>();
    }
    public Proyecto(string nombre, long idGrupo, Grupo idGrupoNavigation, string descripcionBreve, string descripcionCompleta)
    {
        Nombre = nombre;
        IdGrupo = idGrupo;

        IdGrupoNavigation = idGrupoNavigation;
        Comentarios = new List<Comentario>();
        Votos = new List<Voto>();

        // Asignar descripciones (no pueden ser null según la BD); asumimos cadenas no nulas
        DescripcionBreve = descripcionBreve.Trim();
        Descripcion = descripcionCompleta.Trim();
    }

    public override bool Equals(object? obj)
    {
        if (obj is Proyecto otro)
        {
            return (this.IdGrupo == otro.IdGrupo);
        }
        return false;
    }

    public async Task CrearProyecto(
            string nombreProyecto,
            string descripcionBreve,
            string descripcionCompleta,
            byte[] archivo,
            List<byte[]> imagenes,
            string correo, long idEvento, long? grupoIdActivo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombreProyecto))
                    throw new ValidationException("El nombre del proyecto es obligatorio.");

                var participante = await ObtenerRegistroParticipante(correo, idEvento, grupoIdActivo);
                var grupo = await _grupoRepo.GetByIdAsync(participante.IdGrupo)
                    ?? throw new NotFoundException("No se ha encontrado el grupo del participante.");

                var categoria = (await _categoriumRepo.FindAsync(c =>
                    c.Id == grupo.IdCategoria &&
                    c.IdEvento == idEvento))
                    .FirstOrDefault() ?? throw new ConflictException("La categoría del grupo no pertenece al evento.");

                var proyectoExistente = (await _proyectoRepo.FindAsync(p =>
                    p.IdGrupo == grupo.Id
                )).FirstOrDefault();

                if (proyectoExistente != null)
                    throw new ConflictException("Ya existe un proyecto entregado para este grupo. Elimínalo antes de volver a entregarlo.");


                string descBreve;
                if (descripcionBreve == null)
                    descBreve = string.Empty;
                else
                    descBreve = descripcionBreve.Trim();

                string descCompleta;
                if (descripcionCompleta == null)
                    descCompleta = string.Empty;
                else
                    descCompleta = descripcionCompleta.Trim();

                var nuevoProyecto = new Proyecto(
                    nombreProyecto.Trim(),
                    grupo.Id,
                    null!, // No asignar la navegación aquí para evitar que EF intente insertar el grupo
                    descBreve,
                    descCompleta
                )
                {
                    Archivo = archivo,
                    Imagenes = new List<ImagenProyecto>()
                };

                if (imagenes != null && imagenes.Any())
                {
                    var imagenesProyecto = imagenes.Select(img => new ImagenProyecto
                    {
                        Archivo = img
                    }).ToList();

                    nuevoProyecto.Imagenes.AddRange(imagenesProyecto);
                }

                await _proyectoRepo.AddAsync(nuevoProyecto);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error interno: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }
}
