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

        public async Task CreateTodoAsync(Todo todo)
        {
            await _container.CreateItemAsync(todo);
        }
    }
}
