var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.Todo_ApiService>("todo-api")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.Todo_Web>("todo-webfront")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
