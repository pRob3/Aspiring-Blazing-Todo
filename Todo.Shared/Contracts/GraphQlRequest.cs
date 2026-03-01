
namespace Todo.Shared.Contracts;

public sealed class GraphQlRequest
{
    public required string Query { get; init; }

    public object? Variables { get; init; }
}
