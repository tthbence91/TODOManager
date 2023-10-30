﻿using System.ComponentModel.DataAnnotations;
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

        [HttpGet]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> GetTodosOfUserAsync([FromQuery][Required] string user)
        {
            var todoElements = await _repository.GetTodosByUserAsync(user);
            return Ok(todoElements);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> SetTodoDoneAsync(string id, [FromQuery][Required] string user)
        {

            var response = await _repository.SetTodoDoneAsync(id, user);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }
    }
}
