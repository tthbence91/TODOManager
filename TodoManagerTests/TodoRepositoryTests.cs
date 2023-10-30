using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
            var mockLogger = new Mock<ILogger<TodoRepository>>();
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

            var todoRepository = new TodoRepository(mockCosmosClient.Object, "databaseName", "containerName", mockLogger.Object);

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

            var todoRepository = new TodoRepository(mockCosmosClient.Object, "databaseName", "containerName", NullLogger<TodoRepository>.Instance);

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

        [Fact]
        public async Task WhenInputIsValid_SetTodoDoneAsync_ReturnsUpdatedElement()
        {
            // Arrange
            var mockCosmosClient = new Mock<CosmosClient>();
            var mockDatabase = new Mock<Database>();
            var mockContainer = new Mock<Container>();

            string id = "1";
            string user = "testUser";

            var originalTodoElement = new Todo
            {
                Id = id,
                User = user,
                Description = "Task 1",
                IsDone = false
            };
            var updatedTodoElement = new Todo
            {
                Id = id,
                User = user,
                Description = "Task 1",
                IsDone = true
            };
            var itemResponseMock = new Mock<ItemResponse<Todo>>();
            itemResponseMock.SetupSequence(x => x.Resource).Returns(originalTodoElement).Returns(updatedTodoElement);

            mockContainer.Setup(c => c.ReadItemAsync<Todo>(id, new PartitionKey(user),null,default))
                .ReturnsAsync(itemResponseMock.Object);


            var updatedItemResponseMock = new Mock<ItemResponse<Todo>>();
            updatedItemResponseMock.Setup(x => x.Resource).Returns(updatedTodoElement);

            mockContainer.Setup(c => c.ReplaceItemAsync(updatedTodoElement, id, new PartitionKey(user),null,default))
                .ReturnsAsync(updatedItemResponseMock.Object);

            mockCosmosClient.Setup(c => c.GetDatabase("databaseName")).Returns(mockDatabase.Object);
            mockDatabase.Setup(d => d.GetContainer("containerName")).Returns(mockContainer.Object);
            var todoRepository = new TodoRepository(mockCosmosClient.Object, "databaseName", "containerName", NullLogger<TodoRepository>.Instance);

            // Act
            var result = await todoRepository.SetTodoDoneAsync(id, user);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(updatedTodoElement);
        }

        [Fact]
        public async Task SetTodoDoneAsync_IfElementNotFound_ReturnsNull()
        {
            // Arrange
            var mockCosmosClient = new Mock<CosmosClient>();
            var mockDatabase = new Mock<Database>();
            var mockContainer = new Mock<Container>();
            var mockLogger = new Mock<ILogger<TodoRepository>>();
            
            var id = "1";
            var user = "testUser";
            var errorMessage = "An error occured while trying to set a TODO done in the database.";



            mockContainer.Setup(c => c.ReadItemAsync<Todo>(id, new PartitionKey(user), null, default))
                .ThrowsAsync(new CosmosException("Not Found", System.Net.HttpStatusCode.NotFound, 0, null, 1));

            mockCosmosClient.Setup(c => c.GetDatabase("databaseName")).Returns(mockDatabase.Object);
            mockDatabase.Setup(d => d.GetContainer("containerName")).Returns(mockContainer.Object);
            var todoRepository = new TodoRepository(mockCosmosClient.Object, "databaseName", "containerName", mockLogger.Object);

            // Act
            var result = await todoRepository.SetTodoDoneAsync(id, user);

            // Assert
            result.Should().BeNull();
            mockLogger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == errorMessage && @type.Name == "FormattedLogValues"),
                    It.IsAny<CosmosException>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
