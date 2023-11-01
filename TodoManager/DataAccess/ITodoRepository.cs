using TodoManager.Models;

namespace TodoManager.DataAccess;

/// <summary>
/// Interface defining operations for managing TODOs in a data store.
/// </summary>
public interface ITodoRepository
{
    /// <summary>
    /// Creates a new TODOitem.
    /// </summary>
    /// <param name="todoDto">The DTO containing information for the new TODOitem.</param>
    /// <returns>The created TODOitem, or <c>null</c> if creation fails.</returns>
    Task<Todo?> CreateTodoAsync(TodoDto todoDto);

    /// <summary>
    /// Retrieves all TODOitems associated with a specific user.
    /// </summary>
    /// <param name="user">The user for whom to retrieve TODOitems.</param>
    /// <returns>A collection of TODOitems associated with the specified user.</returns>
    Task<IEnumerable<Todo>> GetTodosByUserAsync(string user);

    /// <summary>
    /// Marks a TODOitem as done.
    /// </summary>
    /// <param name="id">The ID of the TODOitem to mark as done.</param>
    /// <param name="user">The user associated with the TODOitem.</param>
    /// <returns>The updated TODOitem, or <c>null</c> if the item does not exist.</returns>
    Task<Todo?> SetTodoDoneAsync(string id, string user);

    /// <summary>
    /// Retrieves TODOitems by their status for a specific user.
    /// </summary>
    /// <param name="user">The user for whom to retrieve TODOitems.</param>
    /// <param name="isDone">The status of the TODOitems to retrieve.</param>
    /// <returns>A collection of TODOitems matching the specified criteria.</returns>
    Task<IEnumerable<Todo>> GetTodosByStatusAsync(string user, bool isDone);

    /// <summary>
    /// Updates the description of a TODOitem.
    /// </summary>
    /// <param name="id">The ID of the TODOitem to update.</param>
    /// <param name="user">The user associated with the TODOitem.</param>
    /// <param name="newDescription">The new description for the TODOitem.</param>
    /// <returns>The updated TODOitem, or <c>null</c> if the item does not exist.</returns>
    Task<Todo?> UpdateTodoDescriptionAsync(string id, string user, string newDescription);
}