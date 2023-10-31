using TodoManager.Models;

namespace TodoManager.DataAccess
{
    public interface ITodoRepository
    {
        Task<Todo> CreateTodoAsync(TodoDto todoDto);

        Task<IEnumerable<Todo>> GetTodosByUserAsync(string user);

        Task<Todo?> SetTodoDoneAsync(string id, string user);
        Task<IEnumerable<Todo>> GetTodosByStatusAsync(string user, bool isDone);
    }
}
