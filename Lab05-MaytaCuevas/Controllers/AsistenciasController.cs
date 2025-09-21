using Lab05_MaytaAlberth.UnitOfWork;
using Lab05_MaytaCuevas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AsistenciasController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public AsistenciasController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // GET: api/Asistencias -> Devuelve una lista optimizada con DTOs
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AsistenciaDetailDto>>> GetAllAsistencias()
    {
        var asistencias = await _unitOfWork.Repository<Asistencia>().GetQueryable()
            .Include(a => a.IdEstudianteNavigation)
            .Include(a => a.IdCursoNavigation)
            .ToListAsync();
        return Ok(asistencias.Select(MapToDetailDto));
    }

    // GET: api/Asistencias/5 -> Devuelve un DTO detallado
    [HttpGet("{id}")]
    public async Task<ActionResult<AsistenciaDetailDto>> GetAsistencia(int id)
    {
        var asistencia = await _unitOfWork.Repository<Asistencia>().GetQueryable()
            .Include(a => a.IdEstudianteNavigation)
            .Include(a => a.IdCursoNavigation)
            .FirstOrDefaultAsync(a => a.IdAsistencia == id);

        if (asistencia == null) return NotFound();
        return Ok(MapToDetailDto(asistencia));
    }

    // POST: api/Asistencias -> Usa DTOs y validaciones eficientes
    [HttpPost]
    public async Task<ActionResult<AsistenciaDetailDto>> CreateAsistencia([FromBody] CreateAsistenciaRequestDto requestDto)
    {
        // Validación optimizada para verificar si el estudiante está matriculado
        var estaMatriculado = await _unitOfWork.Repository<Matricula>().GetQueryable()
            .AnyAsync(m => m.IdEstudiante == requestDto.IdEstudiante && m.IdCurso == requestDto.IdCurso);
        if (!estaMatriculado) return BadRequest("El estudiante no está matriculado en el curso especificado.");

        var nuevaAsistencia = new Asistencia
        {
            IdEstudiante = requestDto.IdEstudiante,
            IdCurso = requestDto.IdCurso,
            Fecha = requestDto.Fecha,
            Estado = requestDto.Estado
        };

        await _unitOfWork.Repository<Asistencia>().AddAsync(nuevaAsistencia);
        await _unitOfWork.Complete();
        
        var actionResult = await GetAsistencia(nuevaAsistencia.IdAsistencia);
        var dto = (actionResult.Result as OkObjectResult)?.Value;
        return CreatedAtAction(nameof(GetAsistencia), new { id = nuevaAsistencia.IdAsistencia }, dto);
    }
    // PUT: api/Asistencias/5 -> Usa DTO y respuesta estándar
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsistencia(int id, [FromBody] UpdateAsistenciaRequestDto requestDto)
    {
        var asistencia = await _unitOfWork.Repository<Asistencia>().GetByIdAsync(id);
        if (asistencia == null) return NotFound();

        asistencia.Estado = requestDto.Estado;
        asistencia.Fecha = requestDto.Fecha;
        _unitOfWork.Repository<Asistencia>().Update(asistencia);
        await _unitOfWork.Complete();
        return NoContent();
    }

    // DELETE: api/Asistencias/5 -> Respuesta estándar
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsistencia(int id)
    {
        if (!await _unitOfWork.Repository<Asistencia>().ExistsAsync(id)) return NotFound();
        await _unitOfWork.Repository<Asistencia>().DeleteAsync(id);
        await _unitOfWork.Complete();
        return NoContent();
    }

    // Método helper privado para mapeo
    private AsistenciaDetailDto MapToDetailDto(Asistencia asistencia)
    {
        return new AsistenciaDetailDto
        {
            IdAsistencia = asistencia.IdAsistencia,
            Fecha = asistencia.Fecha.Value,
            Estado = asistencia.Estado,
            Estudiante = new EstudianteSummaryDto
            {
                IdEstudiante = asistencia.IdEstudianteNavigation.IdEstudiante,
                Nombre = asistencia.IdEstudianteNavigation.Nombre,
                Correo = asistencia.IdEstudianteNavigation.Correo,
                Direccion = asistencia.IdEstudianteNavigation.Direccion,
                Telefono = asistencia.IdEstudianteNavigation.Telefono,
                Edad = asistencia.IdEstudianteNavigation.Edad,
            },
            Curso = new CursoSummaryDto
            {
                IdCurso = asistencia.IdCursoNavigation.IdCurso,
                Nombre = asistencia.IdCursoNavigation.Nombre,
                Descripcion = asistencia.IdCursoNavigation.Descripcion,
                Creditos = asistencia.IdCursoNavigation.Creditos
            }
        };
    }
}