
public class CursoSummaryDto
{
    public int IdCurso { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public int Creditos { get; set; }
}

// DTO para el detalle de un curso (GET /api/cursos/{id})
public class CursoDetailDto
{
    public int IdCurso { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public int Creditos { get; set; }
}

// DTO para crear un nuevo curso (POST /api/cursos)
public class CreateCursoRequestDto
{
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public int Creditos { get; set; }
}

// DTO para actualizar un curso (PUT /api/cursos/{id})
public class UpdateCursoRequestDto
{
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public int Creditos { get; set; }
}
