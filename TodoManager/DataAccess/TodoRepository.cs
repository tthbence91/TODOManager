using System.Collections;
using Microsoft.Azure.Cosmos;
using TodoManager.Models;

namespace TodoManager.DataAccess;

public class TodoRepository : ITodoRepository
{
    private readonly Container _container;
    private readonly ILogger<TodoRepository> _logger;

    public TodoRepository(CosmosClient cosmosClient, string databaseName, string containerName, ILogger<TodoRepository> logger)
    {
        var database = cosmosClient.GetDatabase(databaseName);
        _container = database.GetContainer(containerName);
        _logger = logger;
    }

    public async Task<Todo?> CreateTodoAsync(TodoDto todoDto)
    {
        try
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
        catch (Exception e)
        {
            _logger.LogError(e,  $"An error occurred for user [{todoDto.User}] while trying to create a TODO on the database.");
            return null;
        }
    }

    public async Task<IEnumerable<Todo>> GetTodosByUserAsync(string user)
    {
        try
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
        catch (Exception e)
        {
            _logger.LogError(e, $"An error occurred for user [{user}] while trying to list their TODOs from the database.");
            return Enumerable.Empty<Todo>();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <returns>Can return null, when cannot find </returns>
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
            _logger.LogError(e, $"An error occurred for user [{user}] while trying to set a TODO done in the database.");
            return null;
        }
    }

    public async Task<IEnumerable<Todo>> GetTodosByStatusAsync(string user, bool isDone)
    {
        try
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
        catch (Exception e)
        {
            _logger.LogError(e,$"An error occurred for user [{user}] while trying to query TODOs by status from the database.");
            return Enumerable.Empty<Todo>();
        }
    }

    public async Task<Todo?> UpdateTodoDescriptionAsync(string id, string user, string newDescription)
    {
        try
        {
            var todoElement = await _container.ReadItemAsync<Todo>(id, new PartitionKey(user));

            if (todoElement == null)
            {
                return null;
            }

            todoElement.Resource.Description = newDescription;

            var response = await _container.ReplaceItemAsync<Todo>(
                todoElement.Resource,
                id,
                new PartitionKey(user)
            );

            return response.Resource;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"An error occurred for user [{user}] while trying to change a TODO's description in the database.");
            return null;
        }
    }
}