using Todo.Shared.Models;

namespace Todo.ApiService.Data;

public interface ITodoRepository
{
    IReadOnlyList<TodoItem> GetAll();
    TodoItem Add(string title);
    TodoItem Toggle(Guid id);
    bool Delete(Guid id);
}