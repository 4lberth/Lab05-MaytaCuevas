// DTOs/EstudianteDtos.cs

// DTO para la lista de estudiantes (GET /api/estudiantes)
public class EstudianteSummaryDto
{
    public int IdEstudiante { get; set; }
    public string Nombre { get; set; }
    public string Correo { get; set; }
    public string Direccion { get; set; }
    public string Telefono { get; set; }
    public int Edad { get; set; }
}

// DTO para el detalle de un estudiante (GET /api/estudiantes/{id})
public class EstudianteDetailDto
{
    public int IdEstudiante { get; set; }
    public string Nombre { get; set; }
    public int Edad { get; set; }
    public string Direccion { get; set; }
    public string Telefono { get; set; }
    public string Correo { get; set; }
}

// DTO para crear un nuevo estudiante (POST /api/estudiantes)
public class CreateEstudianteRequestDto
{
    public string Nombre { get; set; }
    public int Edad { get; set; }
    public string Direccion { get; set; }
    public string Telefono { get; set; }
    public string Correo { get; set; }
}

// DTO para actualizar un estudiante (PUT /api/estudiantes/{id})
public class UpdateEstudianteRequestDto
{
    public string Nombre { get; set; }
    public int Edad { get; set; }
    public string Direccion { get; set; }
    public string Telefono { get; set; }
    public string Correo { get; set; }
}
