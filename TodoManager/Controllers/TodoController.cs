using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> CreateTodoElementAsync([FromBody] Todo todo)
        {
            await _repository.CreateTodoAsync(todo);
            return Ok();
        }
    }
}
