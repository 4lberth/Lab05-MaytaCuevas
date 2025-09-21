using Lab05_MaytaAlberth.UnitOfWork;
using Lab05_MaytaCuevas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class MatriculasController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public MatriculasController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // GET: api/Matriculas -> Devuelve una lista optimizada con DTOs
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MatriculaDetailDto>>> GetAllMatriculas()
    {
        var matriculas = await _unitOfWork.Repository<Matricula>().GetQueryable()
            .Include(m => m.IdEstudianteNavigation)
            .Include(m => m.IdCursoNavigation)
            .ToListAsync();

        return Ok(matriculas.Select(MapToDetailDto));
    }

    // GET: api/Matriculas/5 -> Devuelve un DTO detallado
    [HttpGet("{id}")]
    public async Task<ActionResult<MatriculaDetailDto>> GetMatricula(int id)
    {
        var matricula = await _unitOfWork.Repository<Matricula>().GetQueryable()
            .Include(m => m.IdEstudianteNavigation)
            .Include(m => m.IdCursoNavigation)
            .FirstOrDefaultAsync(m => m.IdMatricula == id);

        if (matricula == null)
        {
            return NotFound($"Matrícula con ID {id} no encontrada");
        }

        return Ok(MapToDetailDto(matricula));
    }

    // POST: api/Matriculas -> Usa DTOs para la solicitud y la respuesta
    [HttpPost]
    public async Task<ActionResult<MatriculaDetailDto>> CreateMatricula([FromBody] CreateMatriculaRequestDto requestDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await _unitOfWork.Repository<Estudiante>().ExistsAsync(requestDto.IdEstudiante) ||
            !await _unitOfWork.Repository<Curso>().ExistsAsync(requestDto.IdCurso))
        {
            return BadRequest("El estudiante o el curso especificado no existen.");
        }

        var nuevaMatricula = new Matricula
        {
            IdEstudiante = requestDto.IdEstudiante,
            IdCurso = requestDto.IdCurso,
            Semestre = requestDto.Semestre
        };

        await _unitOfWork.Repository<Matricula>().AddAsync(nuevaMatricula);
        await _unitOfWork.Complete();

        var actionResult = await GetMatricula(nuevaMatricula.IdMatricula);
        var dto = (actionResult.Result as OkObjectResult)?.Value;

        return CreatedAtAction(nameof(GetMatricula), new { id = nuevaMatricula.IdMatricula }, dto);
    }

    // PUT: api/Matriculas/5 -> Usa DTO y devuelve una respuesta estándar
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMatricula(int id, [FromBody] UpdateMatriculaRequestDto requestDto)
    {
        var matricula = await _unitOfWork.Repository<Matricula>().GetByIdAsync(id);
        if (matricula == null)
        {
            return NotFound($"Matrícula con ID {id} no encontrada");
        }

        matricula.Semestre = requestDto.Semestre;
        
        _unitOfWork.Repository<Matricula>().Update(matricula);
        await _unitOfWork.Complete();

        return NoContent(); // Respuesta 204 No Content, estándar para PUT
    }

    // DELETE: api/Matriculas/5 -> Devuelve una respuesta estándar
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMatricula(int id)
    {
        var exists = await _unitOfWork.Repository<Matricula>().ExistsAsync(id);
        if (!exists)
        {
            return NotFound($"Matrícula con ID {id} no encontrada");
        }

        await _unitOfWork.Repository<Matricula>().DeleteAsync(id);
        await _unitOfWork.Complete();

        return NoContent(); // Respuesta 204 No Content, estándar para DELETE
    }

    // Método helper privado para evitar repetir el mapeo a DTO
    private MatriculaDetailDto MapToDetailDto(Matricula matricula)
    {
        return new MatriculaDetailDto
        {
            IdMatricula = matricula.IdMatricula,
            Semestre = matricula.Semestre,
            Estudiante = new EstudianteSummaryDto
            {
                IdEstudiante = matricula.IdEstudianteNavigation.IdEstudiante,
                Nombre = matricula.IdEstudianteNavigation.Nombre,
                Correo = matricula.IdEstudianteNavigation.Correo,
                Direccion = matricula.IdEstudianteNavigation.Direccion,
                Telefono = matricula.IdEstudianteNavigation.Telefono,
                Edad = matricula.IdEstudianteNavigation.Edad,
            },
            Curso = new CursoSummaryDto
            {
                IdCurso = matricula.IdCursoNavigation.IdCurso,
                Nombre = matricula.IdCursoNavigation.Nombre,
                Creditos = matricula.IdCursoNavigation.Creditos,
                Descripcion = matricula.IdCursoNavigation.Descripcion,
            }
        };
    }
}