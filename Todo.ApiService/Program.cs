using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using Todo.ApiService.Data;
using Todo.ApiService.GraphQL;
using Todo.Shared.Models;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddSingleton<ITodoRepository, InMemoryTodoRepository>();

// GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();


builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapDefaultEndpoints();

// Hot Chocolate endpoint
app.MapGraphQL("/graphql");

// playing with Server Sent Events
app.MapGet("/live-orders", (CancellationToken cancellationToken) =>
{
    async IAsyncEnumerable<SseItem<OrderNotification>> GetOrders(
        [EnumeratorCancellation] CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(1500, ct);
            }
            catch (OperationCanceledException)
            {
                yield break;
            }

            yield return new SseItem<OrderNotification>(BoatPartOrderGen.CreateOrder(), "order")
            {
                ReconnectionInterval = TimeSpan.FromMinutes(1),
            };
        }
    }
    return TypedResults.ServerSentEvents(GetOrders(cancellationToken));
});

app.Run();


