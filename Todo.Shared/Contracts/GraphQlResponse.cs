namespace Todo.Shared.Contracts;

public sealed class GraphQlResponse<TData>
{
    public TData? Data { get; init; }
    public GraphQlError[]? Errors { get; init; }
}

public sealed class GraphQlError
{
    public string? Message { get; init; }
}
