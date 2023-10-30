
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Moq;
using TodoManager.DataAccess;
using TodoManager.Models;

namespace TodoManagerTests
{
    public class TodoRepositoryTest
    {
        [Fact]
        public async Task CreateTodoAsync_ShouldReturnCreatedTodo()
        {
            // Arrange
            var mockCosmosClient = new Mock<CosmosClient>();
            var mockDatabase = new Mock<Database>();
            var mockContainer = new Mock<Container>();
            var mockResponse = new Mock<ItemResponse<Todo>>();
            mockCosmosClient.Setup(c => c.GetDatabase("databaseName")).Returns(mockDatabase.Object);
            mockDatabase.Setup(d => d.GetContainer("containerName")).Returns(mockContainer.Object);

            var todoDto = new TodoDto
            {
                User = "Test User",
                Description = "Test Todo",
                IsDone = true
            };

            var todo = new Todo
            {
                Id = Guid.NewGuid().ToString(),
                User = "Test User",
                Description = "Test Todo",
                IsDone = true
            };
            mockResponse.Setup(r => r.Resource).Returns(todo);

            mockContainer.Setup(c => c.CreateItemAsync(It.Is<Todo>(t => t.User == "Test User" && t.Description == "Test Todo" && t.IsDone), null, null, default))
                .ReturnsAsync(mockResponse.Object);

            var todoRepository = new TodoRepository(mockCosmosClient.Object, "databaseName", "containerName");

            // Act
            var createdTodo = await todoRepository.CreateTodoAsync(todoDto);

            // Assert
            createdTodo.Should().NotBeNull();
            Guid.TryParse(createdTodo.Id, out _).Should().BeTrue();
            createdTodo.User.Should().Be("Test User");
            createdTodo.Description.Should().Be("Test Todo");
            createdTodo.IsDone.Should().BeTrue();
        }
    }
}
