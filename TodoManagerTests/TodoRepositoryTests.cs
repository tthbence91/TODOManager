
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

        [Fact]
        public async Task GetTodosByUserAsync_ReturnsListOfTodoElements()
        {
            // Arrange
            var mockCosmosClient = new Mock<CosmosClient>();
            var mockDatabase = new Mock<Database>();
            var mockContainer = new Mock<Container>();
            mockCosmosClient.Setup(c => c.GetDatabase("databaseName")).Returns(mockDatabase.Object);
            mockDatabase.Setup(d => d.GetContainer("containerName")).Returns(mockContainer.Object);

            var todoRepository = new TodoRepository(mockCosmosClient.Object, "databaseName", "containerName");

            string user = "testUser";

            var expectedTodos = new List<Todo>
            {
                new Todo { Id = "1", User = user, Description = "Task 1", IsDone = false },
                new Todo { Id = "2", User = user, Description = "Task 2", IsDone = true }
            };
            var feedResponseMock = new Mock<FeedResponse<Todo>>();
            feedResponseMock.Setup(x => x.GetEnumerator()).Returns(expectedTodos.GetEnumerator());
            var feedIterator = new Mock<FeedIterator<Todo>>();
            feedIterator.Setup(f => f.HasMoreResults).Returns(true);
            feedIterator.Setup(f => f.ReadNextAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(feedResponseMock.Object)
                        .Callback(() => feedIterator
                            .Setup(f => f.HasMoreResults)
                            .Returns(false));

            mockContainer.Setup(c => c.GetItemQueryIterator<Todo>(It.IsAny<QueryDefinition>(), null, It.IsAny<QueryRequestOptions>()))
                .Returns(feedIterator.Object);

            // Act
            var todoElements = await todoRepository.GetTodosByUserAsync(user);

            // Assert
            todoElements.Should().HaveCount(expectedTodos.Count);
            todoElements.Should().Contain(t => t.Id == "1");
            todoElements.Should().Contain(t => t.Id == "2");

        }
    }
}
