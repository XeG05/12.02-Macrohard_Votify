using Microsoft.AspNetCore.Mvc;
using Votify.Modelos.BusinessLogic.Interfaces;

namespace Votify.Components.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenericController<T> : ControllerBase where T : class
    {
        protected readonly IService<T> _service;

        public GenericController(IService<T> service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<T>>> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<T>> GetById(Guid id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public async Task<ActionResult<T>> Create([FromBody] T entity)
        {
            var created = await _service.CreateAsync(entity);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] T entity)
        {
            var prop = typeof(T).GetProperty("Id");
            if (prop == null || (Guid)prop.GetValue(entity)! != id)
                return BadRequest();

            await _service.UpdateAsync(entity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
