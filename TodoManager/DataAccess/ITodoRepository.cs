using TodoManager.Models;

namespace TodoManager.DataAccess
{
    public interface ITodoRepository
    {
        Task CreateTodoAsync(Todo todo);
    }
}
