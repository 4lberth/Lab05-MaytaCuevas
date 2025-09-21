// DTO para las respuestas detalladas (usado en GET y POST)
public class MateriaDetailDto
{
    public int IdMateria { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public CursoSummaryDto Curso { get; set; }
}

// DTO para crear una nueva materia (usado en POST)
public class CreateMateriaRequestDto
{
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public int IdCurso { get; set; }
}

// DTO para actualizar una materia (usado en PUT)
public class UpdateMateriaRequestDto
{
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
}
