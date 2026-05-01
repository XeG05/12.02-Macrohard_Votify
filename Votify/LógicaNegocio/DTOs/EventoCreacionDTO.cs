using Votify.LógicaNegocio.Enums;

namespace Votify.LógicaNegocio.DTOs
{
    public class EventoCreacionDto
    {
        public string Nombre { get; set; } = "";
        public string Descripcion { get; set; } = "";

        public DateOnly? FechaFinInscripciones { get; set; }
        public DateOnly? FechaFinEntrega { get; set; }
        public DateOnly? FechaFinVotaciones { get; set; }
        public DateOnly? FechaFinExperta { get; set; }
        public DateOnly? FechaFinVotExperta { get; set; }

        public TipoComentario TipoComentario { get; set; } = TipoComentario.SinComentarios;
        public TipoVotacion TipoVotacion { get; set; } = TipoVotacion.Simple;

        public bool ParticipantesVotan { get; set; }
        public int NumVotarProyectos { get; set; } = 3;
        public int PuntosARepartir { get; set; } = 100;

        public bool TieneGrupos { get; set; }
        public bool GruposCompletos { get; set; }
        public int NumParticipantes { get; set; } = 1;

        public bool EsPrivado { get; set; }

        public string? PasswordAcceso { get; set; }

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

            if (FechaFinInscripciones == null || FechaFinEntrega == null || FechaFinVotaciones == null || FechaFinExperta == null)
                return (false, "Todas las fechas son obligatorias");

            if (FechaFinInscripciones >= FechaFinEntrega)
                return (false, "La fecha de fin de inscripciones debe ser anterior al fin de entrega");

            if (FechaFinEntrega >= FechaFinVotaciones)
                return (false, "La fecha de fin de entrega debe ser anterior al fin de votaciones (votación popular)");

            if (FechaFinVotaciones >= FechaFinExperta)
                return (false, "La fecha de fin de votaciones debe ser anterior al final de la evaluación experta");

            if (FechaFinInscripciones < DateOnly.FromDateTime(DateTime.Today))
                return (false, "La fecha de fin de inscripciones no puede ser en el pasado");

            if (TieneGrupos && NumParticipantes < 1)
                return (false, "El número de participantes por grupo debe ser al menos 1");

            if (TipoVotacion == TipoVotacion.Simple && NumVotarProyectos < 1)
                return (false, "El número de proyectos a votar debe ser al menos 1");

            if (TipoVotacion == TipoVotacion.Puntos && PuntosARepartir < 1)
                return (false, "Los puntos a repartir deben ser al menos 1");

            foreach (var cat in Categorias)
            {
                if (cat.PonderacionPopular + cat.PonderacionExperto != 100)
                    return (false, $"La suma de voto popular y experto en la categoría '{cat.Nombre}' debe ser exactamente 100.");

                if (cat.Criterios.Count > 0)
                {
                    cat.CriteriosSonPorcentaje = true;

                    var sumCriterios = cat.Criterios.Sum(c => c.Ponderacion);

                    if (sumCriterios != 100)
                        return (false, $"Los criterios de la categoría '{cat.Nombre}' deben sumar exactamente 100% y actualmente suman {sumCriterios}%.");

                    if (cat.Criterios.Any(c => c.Ponderacion < 0 || c.Ponderacion > 100))
                        return (false, $"Los criterios de la categoría '{cat.Nombre}' deben tener valores entre 0 y 100.");
                }
            }

            return (true, "");
        }
    }
}