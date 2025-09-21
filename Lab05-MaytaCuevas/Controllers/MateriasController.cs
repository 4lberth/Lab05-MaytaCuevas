using Lab05_MaytaAlberth.UnitOfWork;
using Lab05_MaytaCuevas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab05_MaytaAlberth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MateriasController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public MateriasController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Materias -> Devuelve lista optimizada con DTOs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MateriaDetailDto>>> GetAllMaterias()
        {
            var materias = await _unitOfWork.Repository<Materia>()
                .GetQueryable()
                .Include(m => m.IdCursoNavigation) // Optimización: Carga el curso relacionado
                .ToListAsync();

            return Ok(materias.Select(MapToDetailDto));
        }

        // GET: api/Materias/5 -> Devuelve un DTO detallado
        [HttpGet("{id}")]
        public async Task<ActionResult<MateriaDetailDto>> GetMateria(int id)
        {
            var materia = await _unitOfWork.Repository<Materia>()
                .GetQueryable()
                .Include(m => m.IdCursoNavigation) // Optimización: Carga el curso relacionado
                .FirstOrDefaultAsync(m => m.IdMateria == id);

            if (materia == null)
            {
                return NotFound($"Materia con ID {id} no encontrada");
            }

            return Ok(MapToDetailDto(materia));
        }

        // POST: api/Materias -> Usa DTOs para la solicitud y respuesta
        [HttpPost]
        public async Task<ActionResult<MateriaDetailDto>> CreateMateria([FromBody] CreateMateriaRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validar que el curso existe
            if (!await _unitOfWork.Repository<Curso>().ExistsAsync(requestDto.IdCurso))
            {
                return BadRequest($"El curso con ID {requestDto.IdCurso} no existe");
            }

            var nuevaMateria = new Materia
            {
                Nombre = requestDto.Nombre,
                Descripcion = requestDto.Descripcion,
                IdCurso = requestDto.IdCurso
            };

            await _unitOfWork.Repository<Materia>().AddAsync(nuevaMateria);
            await _unitOfWork.Complete();

            // Obtenemos la materia creada con sus datos relacionados para la respuesta
            var actionResult = await GetMateria(nuevaMateria.IdMateria);
            var dto = (actionResult.Result as OkObjectResult)?.Value;

            return CreatedAtAction(nameof(GetMateria), new { id = nuevaMateria.IdMateria }, dto);
        }

        // PUT: api/Materias/5 -> Usa DTO y devuelve una respuesta estándar
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMateria(int id, [FromBody] UpdateMateriaRequestDto requestDto)
        {
            var materia = await _unitOfWork.Repository<Materia>().GetByIdAsync(id);
            if (materia == null)
            {
                return NotFound($"Materia con ID {id} no encontrada");
            }

            materia.Nombre = requestDto.Nombre;
            materia.Descripcion = requestDto.Descripcion;

            _unitOfWork.Repository<Materia>().Update(materia);
            await _unitOfWork.Complete();

            return NoContent(); // Respuesta 204 No Content, estándar para PUT
        }

        // DELETE: api/Materias/5 -> Devuelve una respuesta estándar
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMateria(int id)
        {
            var exists = await _unitOfWork.Repository<Materia>().ExistsAsync(id);
            if (!exists)
            {
                return NotFound($"Materia con ID {id} no encontrada");
            }

            await _unitOfWork.Repository<Materia>().DeleteAsync(id);
            await _unitOfWork.Complete();

            return NoContent(); // Respuesta 204 No Content, estándar para DELETE
        }
        
        // Método helper privado para no repetir el mapeo a DTO
        private MateriaDetailDto MapToDetailDto(Materia materia)
        {
            return new MateriaDetailDto
            {
                IdMateria = materia.IdMateria,
                Nombre = materia.Nombre,
                Descripcion = materia.Descripcion,
                Curso = new CursoSummaryDto
                {
                    IdCurso = materia.IdCursoNavigation.IdCurso,
                    Nombre = materia.IdCursoNavigation.Nombre,
                    Descripcion = materia.IdCursoNavigation.Descripcion,
                    Creditos = materia.IdCursoNavigation.Creditos
                }
            };
        }
    }
}