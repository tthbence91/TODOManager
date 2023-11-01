using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TodoManager.DataAccess;
using TodoManager.Models;

namespace TodoManager.Controllers;

/// <summary>
/// Controller for managing TODOs.
/// </summary>
[Route("api/v1/TODO")]
[ApiController]
public class TodoController : ControllerBase
{
    private readonly ITodoRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoController"/> class.
    /// </summary>
    /// <param name="repository">The repository for interacting with TODOs.</param>
    public TodoController(ITodoRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Creates a new Todoitem.
    /// </summary>
    /// <param name="todoDto">The TODOitem to create.</param>
    /// <returns>The created TODOitem.</returns>
    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateTodoAsync([FromBody] TodoDto todoDto)
    {
        var response = await _repository.CreateTodoAsync(todoDto);
        if (response == null)
        {
            return UnprocessableEntity("Could not create TODO in the database.");
        }
        return Ok(response);
    }

    /// <summary>
    /// Gets TODOitems for a specific user.
    /// </summary>
    /// <param name="user">The user for whom to retrieve TODOitems.</param>
    /// <returns>A list of TODOitems for the user.</returns>
    [HttpGet]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTodosOfUserAsync([FromQuery][Required] string user)
    {
        var todoElements = await _repository.GetTodosByUserAsync(user);
        return Ok(todoElements);
    }

    /// <summary>
    /// Sets a TODOitem as done.
    /// </summary>
    /// <param name="id">The ID of the TODOitem.</param>
    /// <param name="user">The user associated with the TODOitem.</param>
    /// <returns>The updated TODOitem, or NotFound if the item was not found.</returns>
    [HttpPut("{id}/setDone")]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SetTodoDoneAsync(string id, [FromQuery][Required] string user)
    {

        var response = await _repository.SetTodoDoneAsync(id, user);
        if (response == null)
        {
            return NotFound("The TODO was not found in the database");
        }
        return Ok(response);
    }

    /// <summary>
    /// Gets TODOitems by their status (done or not done) for a specific user.
    /// </summary>
    /// <param name="user">The user for whom to retrieve TODOitems.</param>
    /// <param name="isDone">A flag indicating whether to retrieve done or not done items.</param>
    /// <returns>A list of TODOitems based on the specified status.</returns>
    [HttpGet("status")]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTodosByStatusAsync([FromQuery][Required] string user, [FromQuery][Required] bool isDone)
    {
        var todoElements = await _repository.GetTodosByStatusAsync(user, isDone);
        return Ok(todoElements);
    }

    /// <summary>
    /// Updates the description of a TODOitem.
    /// </summary>
    /// <param name="id">The ID of the TODOitem.</param>
    /// <param name="user">The user associated with the TODOitem.</param>
    /// <param name="newDescription">The new description for the TODOitem.</param>
    /// <returns>The updated TODOitem, or NotFound if the item was not found.</returns>
    [HttpPut("{id}/description")]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateTodoElementDescriptionAsync(string id, [FromQuery][Required] string user, [FromBody][Required] string newDescription)
    {
        var response = await _repository.UpdateTodoDescriptionAsync(id, user, newDescription);

        if (response == null)
        {
            return NotFound("The TODO was not found in the database");
        }

        return Ok(response);
    }
}