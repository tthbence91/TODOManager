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
            _logger.LogDebug($"Creating a TODO element consumed {response.RequestCharge} RUs");
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

            double requestCharge = 0;
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                requestCharge += response.RequestCharge;
                results.AddRange(response.ToList());
            }
            _logger.LogDebug($"Querying TODO elements consumed {requestCharge} RUs");

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

            if (todoElement.Resource == null)
            {
                _logger.LogInformation($"TODO element with ID [{id}] not found for user [{user}]");
                return null;
            }

            todoElement.Resource.IsDone = true;

            var response = await _container.ReplaceItemAsync(
                todoElement.Resource,
                id,
                new PartitionKey(user)
            );

            _logger.LogDebug($"Setting a TODO element to Done consumed {todoElement.RequestCharge + response.RequestCharge} RUs");

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
            
            double requestCharge = 0;
            while (resultSet.HasMoreResults)
            {
                var response = await resultSet.ReadNextAsync();
                requestCharge += response.RequestCharge;
                todoElements.AddRange(response);
            }
            _logger.LogDebug($"Querying TODO elements by status consumed {requestCharge} RUs");

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

            if (todoElement.Resource == null)
            {
                _logger.LogInformation($"TODO element with ID [{id}] not found for user [{user}]");
                return null;
            }

            todoElement.Resource.Description = newDescription;

            var response = await _container.ReplaceItemAsync(
                todoElement.Resource,
                id,
                new PartitionKey(user)
            );
            
            _logger.LogDebug($"Changing a TODO element's description consumed {todoElement.RequestCharge + response.RequestCharge} RUs");

            return response.Resource;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"An error occurred for user [{user}] while trying to change a TODO's description in the database.");
            return null;
        }
    }
}