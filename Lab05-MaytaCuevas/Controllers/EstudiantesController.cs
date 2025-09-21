using Lab05_MaytaAlberth.UnitOfWork;
using Lab05_MaytaCuevas.Models; // Aún lo necesitas para la entidad
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab05_MaytaAlberth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstudiantesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public EstudiantesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Estudiantes -> Devuelve una lista resumida
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EstudianteSummaryDto>>> GetAllEstudiantes()
        {
            var estudiantes = await _unitOfWork.Repository<Estudiante>().GetAllAsync();
            
            var estudiantesDto = estudiantes.Select(e => new EstudianteSummaryDto
            {
                IdEstudiante = e.IdEstudiante,
                Nombre = e.Nombre,
                Correo = e.Correo,
                Telefono = e.Telefono,
                Edad = e.Edad,
                Direccion = e.Direccion,
            });

            return Ok(estudiantesDto);
        }

        // GET: api/Estudiantes/5 -> Devuelve el detalle completo
        [HttpGet("{id}")]
        public async Task<ActionResult<EstudianteDetailDto>> GetEstudiante(int id)
        {
            var estudiante = await _unitOfWork.Repository<Estudiante>().GetByIdAsync(id);

            if (estudiante == null)
            {
                return NotFound($"Estudiante con ID {id} no encontrado");
            }

            var responseDto = new EstudianteDetailDto
            {
                IdEstudiante = estudiante.IdEstudiante,
                Nombre = estudiante.Nombre,
                Edad = estudiante.Edad,
                Direccion = estudiante.Direccion,
                Telefono = estudiante.Telefono,
                Correo = estudiante.Correo
            };

            return Ok(responseDto);
        }
        
        // POST: api/Estudiantes -> Usa DTOs para la solicitud y respuesta
        [HttpPost]
        public async Task<ActionResult<EstudianteDetailDto>> CreateEstudiante([FromBody] CreateEstudianteRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var nuevoEstudiante = new Estudiante
            {
                Nombre = requestDto.Nombre,
                Edad = requestDto.Edad,
                Direccion = requestDto.Direccion,
                Telefono = requestDto.Telefono,
                Correo = requestDto.Correo
            };

            await _unitOfWork.Repository<Estudiante>().AddAsync(nuevoEstudiante);
            await _unitOfWork.Complete();

            var responseDto = new EstudianteDetailDto
            {
                IdEstudiante = nuevoEstudiante.IdEstudiante,
                Nombre = nuevoEstudiante.Nombre,
                Edad = nuevoEstudiante.Edad,
                Direccion = nuevoEstudiante.Direccion,
                Telefono = nuevoEstudiante.Telefono,
                Correo = nuevoEstudiante.Correo
            };

            return CreatedAtAction(nameof(GetEstudiante), new { id = responseDto.IdEstudiante }, responseDto);
        }

        // PUT: api/Estudiantes/5 -> Usa un DTO para actualizar
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEstudiante(int id, [FromBody] UpdateEstudianteRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var estudianteExistente = await _unitOfWork.Repository<Estudiante>().GetByIdAsync(id);

            if (estudianteExistente == null)
            {
                return NotFound($"Estudiante con ID {id} no encontrado");
            }

            // Mapear los datos del DTO a la entidad existente
            estudianteExistente.Nombre = requestDto.Nombre;
            estudianteExistente.Edad = requestDto.Edad;
            estudianteExistente.Direccion = requestDto.Direccion;
            estudianteExistente.Telefono = requestDto.Telefono;
            estudianteExistente.Correo = requestDto.Correo;

            _unitOfWork.Repository<Estudiante>().Update(estudianteExistente);
            await _unitOfWork.Complete();

            return NoContent(); // Devuelve 204 No Content, una práctica estándar para PUT
        }

        // DELETE: api/Estudiantes/5 -> No necesita DTOs, la lógica está bien
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEstudiante(int id)
        {
            var exists = await _unitOfWork.Repository<Estudiante>().ExistsAsync(id);
            if (!exists)
            {
                return NotFound($"Estudiante con ID {id} no encontrado");
            }

            await _unitOfWork.Repository<Estudiante>().DeleteAsync(id);
            await _unitOfWork.Complete();

            return NoContent(); // Devuelve 204 No Content, estándar para DELETE
        }
    }
}