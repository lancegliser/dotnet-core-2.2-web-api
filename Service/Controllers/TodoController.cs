using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Service.Models;
using Service.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        protected readonly ILogger<TodoController> _logger;
        private readonly TodoContext _context;
        private readonly TodoRepository _repository;

        public TodoController(TodoContext context, ILogger<TodoController> logger)
        {
            _context = context;
            _repository = new TodoRepository(_context);
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<TodoItem>>> GetAll()
        {
            return await _repository.GetAll();
        }

        [HttpGet("{id}", Name = nameof(GetById))]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<TodoItem>> GetById(long id)
        {
            var item = await _context.TodoItems.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }
        
        /// <summary>
        /// Creates a TodoItem.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/Todo
        ///     {
        ///        "id": 1,
        ///        "name": "Item1",
        ///        "isComplete": true
        ///     }
        ///
        /// </remarks>
        /// <param name="item"></param>
        /// <returns>A newly created TodoItem</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>            
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<TodoItem>> Post(TodoItem item)
        {
            await _context.TodoItems.AddAsync(item);
            await _context.SaveChangesAsync();

            return CreatedAtRoute(nameof(TodoController.GetById), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<TodoItem>> Put(long id, TodoItem item)
        {
            if (item == null || item.Id != id)
            {
                var details = ValidationUtility.GetValidationProblemDetails("id", "The path id does not match the id of the item.");
                return BadRequest(details);
            }

            var todo = await _context.TodoItems.FindAsync(id);

            if (todo == null)
            {
                return NotFound();
            }

            // TODO update with PUT behavior instead of PATCH.
            // We should create a blank object of TodoItem and apply the contents sent, all of them.
            todo.IsComplete = item.IsComplete;
            todo.Name = item.Name;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a specific TodoItem.
        /// </summary>
        /// <param name="id"></param>        
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> Delete(long id)
        {
            var todo = _context.TodoItems.Find(id);

            if (todo == null)
            {
                return NoContent();
            }

            _context.TodoItems.Remove(todo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

}