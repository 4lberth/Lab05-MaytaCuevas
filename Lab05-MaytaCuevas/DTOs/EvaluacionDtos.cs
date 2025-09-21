
public class EvaluacionDetailDto
{
    public int IdEvaluacion { get; set; }
    public decimal Calificacion { get; set; }
    public DateOnly Fecha { get; set; }
    public EstudianteSummaryDto Estudiante { get; set; }
    public CursoSummaryDto Curso { get; set; }
}

// DTO para crear una nueva evaluación
public class CreateEvaluacionRequestDto
{
    public int IdEstudiante { get; set; }
    public int IdCurso { get; set; }
    public decimal Calificacion { get; set; }
    public DateOnly Fecha { get; set; }
}

// DTO para actualizar una evaluación
public class UpdateEvaluacionRequestDto
{
    public decimal Calificacion { get; set; }
    public DateOnly Fecha { get; set; }
}
