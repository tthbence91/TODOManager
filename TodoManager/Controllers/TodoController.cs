using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using TodoManager.DataAccess;
using TodoManager.Models;

namespace TodoManager.Controllers
{
    [Route("api/v1/TODO")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ITodoRepository _repository;

        public TodoController(ITodoRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateTodoAsync([FromBody] TodoDto todo)
        {
            var response = await _repository.CreateTodoAsync(todo);
            return Ok(response);
        }

        [HttpGet]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> GetTodosOfUserAsync([FromQuery][Required] string user)
        {
            var todoElements = await _repository.GetTodosByUserAsync(user);
            return Ok(todoElements);
        }

        [HttpPut("{id}/setDone")]
        public async Task<IActionResult> SetTodoDoneAsync(string id, [FromQuery][Required] string user)
        {

            var response = await _repository.SetTodoDoneAsync(id, user);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetTodosByStatusAsync([FromQuery][Required] string user, [FromQuery][Required] bool isDone)
        {
            var todoElements = await _repository.GetTodosByStatusAsync(user, isDone);
            return Ok(todoElements);
        }

        [HttpPut("{id}/description")]
        public async Task<IActionResult> UpdateTodoElementDescriptionAsync(string id, [FromQuery][Required] string user, [FromBody][Required] string newDescription)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(newDescription) || string.IsNullOrEmpty(user))
            {
                return BadRequest("ID, user, and new description parameters are required.");
            }

            var response = await _repository.UpdateTodoDescriptionAsync(id, user, newDescription);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }
    }
}
