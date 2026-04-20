namespace Votify.LógicaNegocio.DTOs
{
    public class EventoCreacionDto
    {
        public string Nombre { get; set; } = "";
        public string Descripcion { get; set; } = "";

        public DateOnly? FechaFinInscripciones { get; set; }
        public DateOnly? FechaFinEntrega { get; set; }
        public DateOnly? FechaFinVotaciones { get; set; }

        public bool TieneGrupos { get; set; }
        public bool GruposCompletos { get; set; }
        public int NumParticipantes { get; set; } = 1;

        public List<CategoriaDto> Categorias { get; set; } = new();

        public (bool esValido, string error) Validar()
        {
            if (string.IsNullOrWhiteSpace(Nombre))
                return (false, "El nombre es obligatorio");

            if (Nombre.Length > 30)
                return (false, "El nombre no puede superar los 30 caracteres");

            if (string.IsNullOrWhiteSpace(Descripcion))
                return (false, "La descripción es obligatoria");

            if (Descripcion.Length > 120)
                return (false, "La descripción no puede superar los 120 caracteres");

            if (FechaFinInscripciones == null || FechaFinEntrega == null || FechaFinVotaciones == null)
                return (false, "Todas las fechas son obligatorias");

            if (FechaFinInscripciones >= FechaFinEntrega)
                return (false, "La fecha de fin de inscripciones debe ser anterior al fin de entrega");

            if (FechaFinEntrega >= FechaFinVotaciones)
                return (false, "La fecha de fin de entrega debe ser anterior al fin de votaciones");

            if (FechaFinInscripciones < DateOnly.FromDateTime(DateTime.Today))
                return (false, "La fecha de fin de inscripciones no puede ser en el pasado");

            if (TieneGrupos && NumParticipantes < 1)
                return (false, "El número de participantes por grupo debe ser al menos 1");

            foreach (var cat in Categorias)
            {
                if (cat.PonderacionPopular + cat.PonderacionExperto != 100)
                    return (false, $"La suma de voto popular y experto en la categoría '{cat.Nombre}' debe ser exactamente 100.");

                if (cat.Criterios.Count > 0 && cat.CriteriosSonPorcentaje)
                {
                    var sumCriterios = cat.Criterios.Sum(c => c.Ponderacion);
                    if (sumCriterios != 100)
                        return (false, $"Los criterios de la categoría '{cat.Nombre}' están configurados como porcentaje, por lo que deben sumar exactamente 100 y actualmente suman {sumCriterios}.");
                }
                else if (cat.Criterios.Count > 0 && !cat.CriteriosSonPorcentaje)
                {
                    if (cat.Criterios.Any(c => c.Ponderacion <= 0))
                        return (false, $"En la categoría '{cat.Nombre}', la evaluación por puntos requiere que todos los criterios valgan más de 0 puntos.");
                }
            }

            return (true, "");
        }
    }
}