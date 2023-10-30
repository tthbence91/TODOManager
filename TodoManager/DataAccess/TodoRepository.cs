using Microsoft.Azure.Cosmos;
using TodoManager.Models;

namespace TodoManager.DataAccess
{
    public class TodoRepository : ITodoRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Database _database;
        private readonly Container _container;

        public TodoRepository(CosmosClient cosmosClient, string databaseName, string containerName)
        {
            _cosmosClient = cosmosClient;
            _database = _cosmosClient.GetDatabase(databaseName);
            _container = _database.GetContainer(containerName);
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

        public async Task<List<Todo>> GetTodosByUserAsync(string user)
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
    }
}
