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

        [Fact]
        public async Task WhenUserProvided_GetTodosAsync_ReturnsOkResult()
        {
            // Arrange
            string user = "testUser";
            var mockTodoRepository = new Mock<ITodoRepository>();
            mockTodoRepository.Setup(r => r.GetTodosByUserAsync(user)).ReturnsAsync(new List<Todo>());
            var todoController = new TodoController(mockTodoRepository.Object);


            // Act
            var result = await todoController.GetTodosOfUserAsync(user);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;
            okResult.Value.Should().BeAssignableTo<List<Todo>>();
        }

        [Fact]
        public async Task WhenInputIsValid_SetTodoDoneAsync_ReturnsOkResult()
        {
            // Arrange
            var mockTodoRepository = new Mock<ITodoRepository>();
            var todoController = new TodoController(mockTodoRepository.Object);

            string id = "1";
            string user = "testUser";

            var expectedTodoElement = new Todo
            {
                Id = id,
                User = user,
                Description = "Task 1",
                IsDone = true
            };

            mockTodoRepository.Setup(repo => repo.SetTodoDoneAsync(id, user))
                .ReturnsAsync(expectedTodoElement);

            // Act
            var result = await todoController.SetTodoDoneAsync(id, user);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;
            okResult.Value.Should().BeEquivalentTo(expectedTodoElement);
        }

        [Fact]
        public async Task SetTodoDoneAsync_IfTodoNotFound_ReturnsNotFound()
        {
            // Arrange
            var mockTodoRepository = new Mock<ITodoRepository>();
            var todoController = new TodoController(mockTodoRepository.Object);

            string id = "1";
            string user = "testUser";

            mockTodoRepository.Setup(repo => repo.SetTodoDoneAsync(id, user))
                .ReturnsAsync((Todo)null);

            // Act
            var result = await todoController.SetTodoDoneAsync(id, user);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
