using TodoManager.Models;

namespace TodoManager.DataAccess
{
    public interface ITodoRepository
    {
        Task<Todo> CreateTodoAsync(TodoDto todoDto);
    }
}
