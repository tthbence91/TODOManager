using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TodoManager.DataAccess;
using TodoManager.Models;

namespace TodoManager.Controllers;

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
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateTodoAsync([FromBody] TodoDto todo)
    {
        var response = await _repository.CreateTodoAsync(todo);
        if (response == null)
        {
            return UnprocessableEntity("Could not create TODO on the database.");
        }
        return Ok(response);
    }

    [HttpGet]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTodosOfUserAsync([FromQuery][Required] string user)
    {
        var todoElements = await _repository.GetTodosByUserAsync(user);
        return Ok(todoElements);
    }

    [HttpPut("{id}/setDone")]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
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
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTodosByStatusAsync([FromQuery][Required] string user, [FromQuery][Required] bool isDone)
    {
        var todoElements = await _repository.GetTodosByStatusAsync(user, isDone);
        return Ok(todoElements);
    }

    [HttpPut("{id}/description")]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
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