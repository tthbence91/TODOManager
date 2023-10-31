using Microsoft.Azure.Cosmos;
using TodoManager.Models;

namespace TodoManager.DataAccess
{
    public class TodoRepository : ITodoRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Database _database;
        private readonly Container _container;
        private readonly ILogger<TodoRepository> _logger;

        public TodoRepository(CosmosClient cosmosClient, string databaseName, string containerName, ILogger<TodoRepository> logger)
        {
            _cosmosClient = cosmosClient;
            _database = _cosmosClient.GetDatabase(databaseName);
            _container = _database.GetContainer(containerName);
            _logger = logger;
        }

        public async Task<Todo> CreateTodoAsync(TodoDto todoDto)
        {
            var todo = new Todo
            {
                Id = Guid.NewGuid().ToString(), // Generate new GUID
                User = todoDto.User,
                Description = todoDto.Description,
                IsDone = todoDto.IsDone
            };

            var response = await _container.CreateItemAsync(todo);
            return response.Resource;
        }

        public async Task<IEnumerable<Todo>> GetTodosByUserAsync(string user)
        {
            var query = new QueryDefinition("SELECT * FROM c WHERE c.User = @user")
                .WithParameter("@user", user);

            var iterator = _container.GetItemQueryIterator<Todo>(query);
            var results = new List<Todo>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<Todo?> SetTodoDoneAsync(string id, string user)
        {
            try
            {
                var todoElement = await _container.ReadItemAsync<Todo>(id, new PartitionKey(user));

                todoElement.Resource.IsDone = true;

                var response = await _container.ReplaceItemAsync(
                    todoElement.Resource,
                    id,
                    new PartitionKey(user)
                );

                return response.Resource;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured while trying to set a TODO done in the database.");
            }

            return null;
        }

        public async Task<IEnumerable<Todo>> GetTodosByStatusAsync(string user, bool isDone)
        {
            var query = new QueryDefinition("SELECT * FROM c WHERE c.User = @user AND c.IsDone = @isDone")
                .WithParameter("@user", user)
                .WithParameter("@isDone", isDone);

            var todoElements = new List<Todo>();

            var resultSet = _container.GetItemQueryIterator<Todo>(query);

            while (resultSet.HasMoreResults)
            {
                var response = await resultSet.ReadNextAsync();
                todoElements.AddRange(response);
            }

            return todoElements;
        }
    }
}
