using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoManager.Controllers;
using TodoManager.DataAccess;
using TodoManager.Models;

namespace TodoManagerTests;

public class TodoControllerTest
{
    #region CreateTodoAsync

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
        var result = await todoController.CreateTodoAsync(todoDto);

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
    public async Task WhenPostBodyIsCorrect_CreateTodoAsync_IfRepositoryReturnsNull_ReturnsUnprocessableEntityResult()
    {
        // Arrange
        var mockRepository = new Mock<ITodoRepository>();
        var controller = new TodoController(mockRepository.Object);
        var todoDto = new TodoDto
        {
            User = "testUser",
            Description = "Test Description",
            IsDone = false
        };
        
        mockRepository.Setup(repo => repo.CreateTodoAsync(todoDto))
            .ReturnsAsync((Todo?)null);

        // Act
        var result = await controller.CreateTodoAsync(todoDto);

        // Assert
        result.Should().BeOfType<UnprocessableEntityObjectResult>();
    }

    #endregion

    #region GetTodosOfUserAsync

    [Fact]
    public async Task WhenUserProvided_GetTodosAsync_ReturnsOkResult()
    {
        // Arrange
        string user = "testUser";
        var mockTodoRepository = new Mock<ITodoRepository>();
        var todoList = new List<Todo>
        {
            new() { Id = "1", User = user, Description = "Task 1", IsDone = true },
            new() { Id = "2", User = user, Description = "Task 2", IsDone = false }
        };
        mockTodoRepository.Setup(r => r.GetTodosByUserAsync(user)).ReturnsAsync(todoList);
        var todoController = new TodoController(mockTodoRepository.Object);


        // Act
        var result = await todoController.GetTodosOfUserAsync(user);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().BeAssignableTo<List<Todo>>();
        var resultList = okResult.Value as List<Todo>;
        resultList.Should().NotBeNullOrEmpty();
        resultList.Should().HaveCount(2);
        resultList.All(i => i.User == "testUser").Should().BeTrue();
        resultList.Should().Contain(i => i.Id == "1");
        resultList.Should().Contain(i => i.Id == "2");
    }

    #endregion

    #region SetTodoDoneAsync

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
    public async Task WhenInputIsValid_SetTodoDoneAsync_IfTodoNotFound_ReturnsNotFound()
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
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region GetTodosByStatusAsync

    [Fact]
    public async Task WhenInputIsValid_GetTodosByStatusAsync_ReturnsOkResultAndListOfFoundTodos()
    {
        // Arrange
        var mockRepository = new Mock<ITodoRepository>();
        var controller = new TodoController(mockRepository.Object);
        var user = "testUser";
        var isDone = true;

        var todoList = new List<Todo>
        {
            new() { Id = "1", User = user, Description = "Task 1", IsDone = true },
            new() { Id = "2", User = user, Description = "Task 2", IsDone = true }
        };

        mockRepository.Setup(repo => repo.GetTodosByStatusAsync(user, isDone))
            .ReturnsAsync(todoList);

        // Act
        var result = await controller.GetTodosByStatusAsync(user, isDone);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().BeOfType<List<Todo>>();
        var resultList = okResult.Value as List<Todo>;
        resultList.Should().NotBeNullOrEmpty();
        resultList.Should().HaveCount(2);
        resultList.All(i => i.IsDone).Should().BeTrue();
        resultList.Should().Contain(i=>i.Id == "1");
        resultList.Should().Contain(i=>i.Id == "2");
    }

    #endregion

    #region UpdateTodoElementDescriptionAsync

    [Fact]
    public async Task WhenInputIsValid_UpdateTodoElementDescriptionAsync_IfTodoIsFoundAndUpdated_ReturnsOkResult()
    {
        // Arrange
        var mockRepository = new Mock<ITodoRepository>();
        var controller = new TodoController(mockRepository.Object);
        var id = "testId";
        var user = "testUser";
        var newDescription = "New description";

        var mockUpdatedTodo = new Todo
        {
            Id = id,
            User = user,
            Description = newDescription,
            IsDone = false
        };

        mockRepository.Setup(repo => repo.UpdateTodoDescriptionAsync(id, user, newDescription))
            .ReturnsAsync(mockUpdatedTodo);

        // Act
        var result = await controller.UpdateTodoElementDescriptionAsync(id, user, newDescription);

        // Assert
        result.Should().BeOfType<OkObjectResult>();

        var okResult = (OkObjectResult)result;
        okResult.Value.Should().BeOfType<Todo>();
        var updatedTodo = okResult.Value as Todo;
        updatedTodo.Id.Should().Be(id);
        updatedTodo.User.Should().Be(user);
        updatedTodo.Description.Should().Be(newDescription);
        updatedTodo.IsDone.Should().Be(false);
    }

    [Fact]
    public async Task WhenInputIsValid_UpdateTodoElementDescriptionAsync_IfTodoNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var mockRepository = new Mock<ITodoRepository>();
        var controller = new TodoController(mockRepository.Object);
        var id = "testId";
        var user = "testUser";
        var newDescription = "New description";
        
        mockRepository.Setup(repo => repo.UpdateTodoDescriptionAsync(id, user, newDescription))
            .ReturnsAsync((Todo?)null);

        // Act
        var result = await controller.UpdateTodoElementDescriptionAsync(id, user, newDescription);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion
}