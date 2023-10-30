using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoManager.Controllers;
using TodoManager.DataAccess;
using TodoManager.Models;

namespace TodoManagerTests
{
    public class TodoControllerTest
    {
        [Fact]
        public async Task WhenPostBodyIsCorrect_ShouldReturnCreatedTodo()
        {
            // Arrange
            var mockTodoRepository = new Mock<ITodoRepository>();
            var todoController = new TodoController(mockTodoRepository.Object);

            var todoDto = new TodoDto
            {
                Description = "Test Todo",
                User = "Test User",
                IsDone = false
            };

            var createdTodo = new Todo
            {
                Id = "1",
                Description = "Test Todo",
                User = "Test User",
                IsDone = false
            };

            mockTodoRepository.Setup(r => r.CreateTodoAsync(It.IsAny<TodoDto>())).ReturnsAsync(createdTodo);

            // Act
            var result = await todoController.CreateTodoElementAsync(todoDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;

            okResult.Value.Should().BeOfType<Todo>();
            var model = (Todo)okResult.Value!;

            model.Id.Should().Be("1");
            model.Description.Should().Be("Test Todo");
            model.User.Should().Be("Test User");
            model.IsDone.Should().BeFalse();
        }
    }
}
