namespace Todo.Shared.Models;

public sealed class TodoItem
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public bool IsDone { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}
