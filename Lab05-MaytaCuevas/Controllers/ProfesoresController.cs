using Lab05_MaytaAlberth.UnitOfWork;
using Lab05_MaytaCuevas.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lab05_MaytaAlberth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfesoresController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProfesoresController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Profesores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Profesore>>> GetAllProfesores()
        {
            var profesores = await _unitOfWork.Repository<Profesore>().GetAllAsync();
            return Ok(profesores);
        }

        // GET: api/Profesores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Profesore>> GetProfesor(int id)
        {
            var profesor = await _unitOfWork.Repository<Profesore>().GetByIdAsync(id);

            if (profesor == null)
            {
                return NotFound($"Profesor con ID {id} no encontrado");
            }

            return Ok(profesor);
        }

        // POST: api/Profesores
        [HttpPost]
        public async Task<ActionResult<Profesore>> CreateProfesor([FromBody] Profesore profesor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _unitOfWork.Repository<Profesore>().AddAsync(profesor);
            await _unitOfWork.Complete();

            return CreatedAtAction(nameof(GetProfesor), new { id = profesor.IdProfesor }, profesor);
        }

        // PUT: api/Profesores/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfesor(int id, [FromBody] Profesore profesor)
        {
            if (id != profesor.IdProfesor)
            {
                return BadRequest("El ID no coincide con el profesor");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var exists = await _unitOfWork.Repository<Profesore>().ExistsAsync(id);
            if (!exists)
            {
                return NotFound($"Profesor con ID {id} no encontrado");
            }

            _unitOfWork.Repository<Profesore>().Update(profesor);
            await _unitOfWork.Complete();

            return Ok(profesor);
        }

        // DELETE: api/Profesores/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfesor(int id)
        {
            var exists = await _unitOfWork.Repository<Profesore>().ExistsAsync(id);
            if (!exists)
            {
                return NotFound($"Profesor con ID {id} no encontrado");
            }

            await _unitOfWork.Repository<Profesore>().DeleteAsync(id);
            await _unitOfWork.Complete();

            return Ok($"Profesor con ID {id} eliminado exitosamente");
        }
    }
}