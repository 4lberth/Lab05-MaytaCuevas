// DTOs/MatriculaDtos.cs

// DTO para las respuestas detalladas (GET, POST)
public class MatriculaDetailDto
{
    public int IdMatricula { get; set; }
    public string Semestre { get; set; }
    public EstudianteSummaryDto Estudiante { get; set; }
    public CursoSummaryDto Curso { get; set; }
}

// DTO para crear una nueva matrícula (POST)
public class CreateMatriculaRequestDto
{
    public int IdEstudiante { get; set; }
    public int IdCurso { get; set; }
    public string Semestre { get; set; }
}

// DTO para actualizar una matrícula (PUT)
public class UpdateMatriculaRequestDto
{
    public string Semestre { get; set; }
}