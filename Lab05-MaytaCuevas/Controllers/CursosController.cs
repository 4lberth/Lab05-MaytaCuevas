using Lab05_MaytaAlberth.UnitOfWork;
using Lab05_MaytaCuevas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 

namespace Lab05_MaytaAlberth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CursosController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CursosController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Cursos -> Devuelve una lista resumida
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CursoSummaryDto>>> GetAllCursos()
        {
            var cursos = await _unitOfWork.Repository<Curso>().GetAllAsync();
            var cursosDto = cursos.Select(c => new CursoSummaryDto
            {
                IdCurso = c.IdCurso,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                Creditos = c.Creditos
            });
            return Ok(cursosDto);
        }

        // GET: api/Cursos/5 -> Devuelve el detalle completo
        [HttpGet("{id}")]
        public async Task<ActionResult<CursoDetailDto>> GetCurso(int id)
        {
            var curso = await _unitOfWork.Repository<Curso>().GetByIdAsync(id);

            if (curso == null)
            {
                return NotFound($"Curso con ID {id} no encontrado");
            }

            var cursoDto = new CursoDetailDto
            {
                IdCurso = curso.IdCurso,
                Nombre = curso.Nombre,
                Descripcion = curso.Descripcion,
                Creditos = curso.Creditos
            };
            return Ok(cursoDto);
        }

        // POST: api/Cursos -> Usa DTOs para crear y responder
        [HttpPost]
        public async Task<ActionResult<CursoDetailDto>> CreateCurso([FromBody] CreateCursoRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var nuevoCurso = new Curso
            {
                Nombre = requestDto.Nombre,
                Descripcion = requestDto.Descripcion,
                Creditos = requestDto.Creditos
            };

            await _unitOfWork.Repository<Curso>().AddAsync(nuevoCurso);
            await _unitOfWork.Complete();

            var responseDto = new CursoDetailDto
            {
                IdCurso = nuevoCurso.IdCurso,
                Nombre = nuevoCurso.Nombre,
                Descripcion = nuevoCurso.Descripcion,
                Creditos = nuevoCurso.Creditos
            };

            return CreatedAtAction(nameof(GetCurso), new { id = responseDto.IdCurso }, responseDto);
        }

        // PUT: api/Cursos/5 -> Usa un DTO para actualizar
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCurso(int id, [FromBody] UpdateCursoRequestDto requestDto)
        {
            var cursoExistente = await _unitOfWork.Repository<Curso>().GetByIdAsync(id);
            if (cursoExistente == null)
            {
                return NotFound($"Curso con ID {id} no encontrado");
            }

            cursoExistente.Nombre = requestDto.Nombre;
            cursoExistente.Descripcion = requestDto.Descripcion;
            cursoExistente.Creditos = requestDto.Creditos;

            _unitOfWork.Repository<Curso>().Update(cursoExistente);
            await _unitOfWork.Complete();

            return NoContent(); // Respuesta estándar para PUT exitoso
        }

        // DELETE: api/Cursos/5 -> Lógica correcta, respuesta estándar
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCurso(int id)
        {
            var exists = await _unitOfWork.Repository<Curso>().ExistsAsync(id);
            if (!exists)
            {
                return NotFound($"Curso con ID {id} no encontrado");
            }

            await _unitOfWork.Repository<Curso>().DeleteAsync(id);
            await _unitOfWork.Complete();

            return NoContent(); // Respuesta estándar para DELETE exitoso
        }

        
    }
}