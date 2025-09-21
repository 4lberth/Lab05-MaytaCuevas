// DTO para las respuestas detalladas
public class AsistenciaDetailDto
{
    public int IdAsistencia { get; set; }
    public DateOnly Fecha { get; set; }
    public string Estado { get; set; }
    public EstudianteSummaryDto Estudiante { get; set; }
    public CursoSummaryDto Curso { get; set; }
}

// DTO para crear un único registro de asistencia
public class CreateAsistenciaRequestDto
{
    public int IdEstudiante { get; set; }
    public int IdCurso { get; set; }
    public DateOnly Fecha { get; set; }
    public string Estado { get; set; }
}

// DTO para actualizar un registro de asistencia
public class UpdateAsistenciaRequestDto
{
    public DateOnly Fecha { get; set; }
    public string Estado { get; set; }
}
