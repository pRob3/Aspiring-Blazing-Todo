using System.Text.Json;
using Todo.Shared.Contracts;
using Todo.Shared.Models;

namespace Todo.Web.Services;

public sealed class TodoGraphQlClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _http;

    public TodoGraphQlClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<TodoItem>> GetTodosAsync(CancellationToken ct = default)
    {
        const string query = """
            query {
              todos {
                id
                title
                isDone
                createdAt
              }
            }
            """;

        var response = await PostAsync<TodosData>(query, variables: null, ct);
        return response.Data?.Todos ?? Array.Empty<TodoItem>();
    }

    public async Task<TodoItem> AddTodoAsync(string title, CancellationToken ct = default)
    {
        const string mutation = """
            mutation($title: String!) {
              addTodo(title: $title) {
                id
                title
                isDone
                createdAt
              }
            }
            """;

        var variables = new { title };
        var response = await PostAsync<AddTodoData>(mutation, variables, ct);

        var item = response.Data?.AddTodo;
        if (item is null) throw new InvalidOperationException("No todo returned from addTodo.");
        return item;
    }

    public async Task<TodoItem> ToggleTodoAsync(Guid id, CancellationToken ct = default)
    {
        const string mutation = """
            mutation($id: UUID!) {
              toggleTodo(id: $id) {
                id
                title
                isDone
                createdAt
              }
            }
            """;

        var variables = new { id };
        var response = await PostAsync<ToggleTodoData>(mutation, variables, ct);

        var item = response.Data?.ToggleTodo;
        if (item is null) throw new InvalidOperationException("No todo returned from toggleTodo.");
        return item;
    }

    public async Task<bool> DeleteTodoAsync(Guid id, CancellationToken ct = default)
    {
        const string mutation = """
            mutation($id: UUID!) {
              deleteTodo(id: $id)
            }
            """;

        var variables = new { id };
        var response = await PostAsync<DeleteTodoData>(mutation, variables, ct);

        return response.Data?.DeleteTodo ?? false;
    }

    private async Task<GraphQlResponse<TData>> PostAsync<TData>(string query, object? variables, CancellationToken ct)
    {
        var request = new GraphQlRequest
        {
            Query = query,
            Variables = variables
        };

        using var httpResponse = await _http.PostAsJsonAsync("/graphql", request, JsonOptions, ct);

        httpResponse.EnsureSuccessStatusCode();

        var gqlResponse = await httpResponse.Content.ReadFromJsonAsync<GraphQlResponse<TData>>(JsonOptions, ct);
        if (gqlResponse is null) throw new InvalidOperationException("GraphQL response was empty.");

        if (gqlResponse.Errors is { Length: > 0 })
        {
            var msg = string.Join("; ", gqlResponse.Errors.Select(e => e.Message).Where(m => !string.IsNullOrWhiteSpace(m)));
            throw new InvalidOperationException($"GraphQL error(s): {msg}");
        }

        return gqlResponse;
    }

    // Response shapes
    private sealed class TodosData
    {
        public TodoItem[] Todos { get; init; } = [];
    }

    private sealed class AddTodoData
    {
        public TodoItem? AddTodo { get; init; }
    }

    private sealed class ToggleTodoData
    {
        public TodoItem? ToggleTodo { get; init; }
    }

    private sealed class DeleteTodoData
    {
        public bool DeleteTodo { get; init; }
    }
}
