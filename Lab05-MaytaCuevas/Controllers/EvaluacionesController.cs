using Lab05_MaytaAlberth.UnitOfWork;
using Lab05_MaytaCuevas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab05_MaytaAlberth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EvaluacionesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public EvaluacionesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Evaluaciones -> Devuelve una lista detallada y optimizada
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EvaluacionDetailDto>>> GetAllEvaluaciones()
        {
            var evaluaciones = await _unitOfWork.Repository<Evaluacione>()
                .GetQueryable()
                .Include(e => e.IdEstudianteNavigation)
                .Include(e => e.IdCursoNavigation)
                .ToListAsync();

            var evaluacionesDto = evaluaciones.Select(e => MapToDetailDto(e));
            return Ok(evaluacionesDto);
        }

        // GET: api/Evaluaciones/5 -> Devuelve un detalle completo
        [HttpGet("{id}")]
        public async Task<ActionResult<EvaluacionDetailDto>> GetEvaluacion(int id)
        {
            var evaluacion = await _unitOfWork.Repository<Evaluacione>()
                .GetQueryable()
                .Include(e => e.IdEstudianteNavigation)
                .Include(e => e.IdCursoNavigation)
                .FirstOrDefaultAsync(e => e.IdEvaluacion == id);

            if (evaluacion == null)
            {
                return NotFound($"Evaluación con ID {id} no encontrada");
            }

            return Ok(MapToDetailDto(evaluacion));
        }

        // POST: api/Evaluaciones -> Usa DTOs y validaciones eficientes
        [HttpPost]
        public async Task<ActionResult<EvaluacionDetailDto>> CreateEvaluacion([FromBody] CreateEvaluacionRequestDto requestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Validar que el estudiante y curso existen
            if (!await _unitOfWork.Repository<Estudiante>().ExistsAsync(requestDto.IdEstudiante) ||
                !await _unitOfWork.Repository<Curso>().ExistsAsync(requestDto.IdCurso))
            {
                return BadRequest("El estudiante o el curso especificado no existen.");
            }

            // Validación optimizada: buscar si existe al menos una matrícula que coincida
            var estaMatriculado = await _unitOfWork.Repository<Matricula>()
                .GetQueryable()
                .AnyAsync(m => m.IdEstudiante == requestDto.IdEstudiante && m.IdCurso == requestDto.IdCurso);

            if (!estaMatriculado)
            {
                return BadRequest("El estudiante no está matriculado en este curso.");
            }

            var nuevaEvaluacion = new Evaluacione
            {
                IdEstudiante = requestDto.IdEstudiante,
                IdCurso = requestDto.IdCurso,
                Calificacion = requestDto.Calificacion,
                Fecha = requestDto.Fecha
            };

            await _unitOfWork.Repository<Evaluacione>().AddAsync(nuevaEvaluacion);
            await _unitOfWork.Complete();

            // Recargamos la entidad con sus navegaciones para devolver una respuesta completa
            var evaluacionCreada = await _unitOfWork.Repository<Evaluacione>()
                .GetQueryable()
                .Include(e => e.IdEstudianteNavigation)
                .Include(e => e.IdCursoNavigation)
                .FirstOrDefaultAsync(e => e.IdEvaluacion == nuevaEvaluacion.IdEvaluacion);
            
            return CreatedAtAction(nameof(GetEvaluacion), new { id = evaluacionCreada.IdEvaluacion }, MapToDetailDto(evaluacionCreada));
        }

        // PUT: api/Evaluaciones/5 -> Usa DTO y respuesta estándar
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvaluacion(int id, [FromBody] UpdateEvaluacionRequestDto requestDto)
        {
            var evaluacion = await _unitOfWork.Repository<Evaluacione>().GetByIdAsync(id);
            if (evaluacion == null)
            {
                return NotFound($"Evaluación con ID {id} no encontrada");
            }

            evaluacion.Calificacion = requestDto.Calificacion;
            evaluacion.Fecha = requestDto.Fecha;

            _unitOfWork.Repository<Evaluacione>().Update(evaluacion);
            await _unitOfWork.Complete();

            return NoContent();
        }

        // DELETE: api/Evaluaciones/5 -> Respuesta estándar
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvaluacion(int id)
        {
            if (!await _unitOfWork.Repository<Evaluacione>().ExistsAsync(id))
            {
                return NotFound($"Evaluación con ID {id} no encontrada");
            }

            await _unitOfWork.Repository<Evaluacione>().DeleteAsync(id);
            await _unitOfWork.Complete();

            return NoContent();
        }
        
        // Método helper privado para evitar repetir el mapeo
        private EvaluacionDetailDto MapToDetailDto(Evaluacione evaluacion)
        {
            return new EvaluacionDetailDto
            {
                IdEvaluacion = evaluacion.IdEvaluacion,
                Calificacion = evaluacion.Calificacion.Value,
                Fecha = evaluacion.Fecha.Value,
                Estudiante = new EstudianteSummaryDto
                {
                    IdEstudiante = evaluacion.IdEstudianteNavigation.IdEstudiante,
                    Nombre = evaluacion.IdEstudianteNavigation.Nombre,
                    Correo = evaluacion.IdEstudianteNavigation.Correo,
                    Direccion =  evaluacion.IdEstudianteNavigation.Direccion,
                    Telefono = evaluacion.IdEstudianteNavigation.Telefono,
                    Edad = evaluacion.IdEstudianteNavigation.Edad,
                },
                Curso = new CursoSummaryDto
                {
                    IdCurso = evaluacion.IdCursoNavigation.IdCurso,
                    Nombre = evaluacion.IdCursoNavigation.Nombre,
                    Descripcion = evaluacion.IdCursoNavigation.Descripcion,
                    Creditos = evaluacion.IdCursoNavigation.Creditos
                }
            };
        }
    }
}