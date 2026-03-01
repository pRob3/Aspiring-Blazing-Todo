using Todo.ApiService.Data;
using Todo.Shared.Models;
using KeyNotFoundException = System.Collections.Generic.KeyNotFoundException;

namespace Todo.ApiService.GraphQL;

public sealed class Mutation
{
    public TodoItem AddTodo(string title, [Service] ITodoRepository repo)
        => repo.Add(title);


    public TodoItem ToggleTodo(Guid id, [Service] ITodoRepository repo)
    {
        try
        {
            return repo.Toggle(id);
        }
        catch (KeyNotFoundException)
        {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetMessage($"Todo '{id}' was not found.")
                    .SetCode("TODO_NOT_FOUND")
                    .Build());
        }
    }

    public bool DeleteTodo(Guid id, [Service] ITodoRepository repo)
    {
        var deleted = repo.Delete(id);

        if (!deleted)
        {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetMessage($"Todo '{id}' was not found.")
                    .SetCode("TODO_NOT_FOUND")
                    .Build());
        }

        return true;
    }
}