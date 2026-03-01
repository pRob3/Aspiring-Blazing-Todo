using System.Collections.Concurrent;
using Todo.Shared.Models;
using KeyNotFoundException = System.Collections.Generic.KeyNotFoundException;

namespace Todo.ApiService.Data;

public sealed class InMemoryTodoRepository : ITodoRepository
{
    private readonly ConcurrentDictionary<Guid, TodoItem> _items = new();

    public InMemoryTodoRepository()
    {
        Seed();
    }

    public IReadOnlyList<TodoItem> GetAll()
        => _items.Values
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

    public TodoItem Add(string title)
    {
        var item = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            IsDone = false,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _items[item.Id] = item;
        return item;
    }

    public TodoItem Toggle(Guid id)
    {
        if (!_items.TryGetValue(id, out var existing))
            throw new KeyNotFoundException($"TodoItem '{id}' not found.");

        var updated = new TodoItem
        {
            Id = existing.Id,
            Title = existing.Title,
            IsDone = !existing.IsDone,
            CreatedAt = existing.CreatedAt
        };

        _items[id] = updated;
        return updated;
    }

    public bool Delete(Guid id)
        => _items.TryRemove(id, out _);


    // Demo seed data
    private void Seed()
    {
        AddSeed("Ringa 'en kille som kan svetsa'", isDone: true);
        AddSeed("Vaxa masten.", isDone: true);
        AddSeed("Köpa tacokrydda.", isDone: false);
        AddSeed("Det är inte snett — det är marint.", isDone: true);
        AddSeed("Polera relingen så måsarna ser sin spegelbild.", false);
        AddSeed("Leka med Aspire, Blazor + GraphQL.", isDone: false);
        AddSeed("Testa tutor, mest för stämningens skull.", isDone: false);
    }

    private void AddSeed(string title, bool isDone)
    {
        var item = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = title,
            IsDone = isDone,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _items[item.Id] = item;
    }
}