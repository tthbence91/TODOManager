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
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateTodoElementAsync([FromBody] TodoDto todo)
        {
            var response = await _repository.CreateTodoAsync(todo);
            return Ok(response);
        }
    }
}
