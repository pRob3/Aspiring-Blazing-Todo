using Todo.ApiService.Data;
using Todo.Shared.Models;

namespace Todo.ApiService.GraphQL;

public sealed class Query
{
    public IReadOnlyList<TodoItem> Todos([Service] ITodoRepository repo)
        => repo.GetAll();
}
