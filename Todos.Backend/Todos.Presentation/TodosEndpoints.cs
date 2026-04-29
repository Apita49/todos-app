using Microsoft.Extensions.Logging;
using Todos.Domain;
using Todos.Service;
using Todos.Middlewares.Exceptions;

namespace Todos.API
{
    public static class TodosEndpoints
    {
        public static void MapTodosEndpoint(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/todo");

            group.MapGet("/", async (ITodoService service, ILogger<Program> logger) =>
            {
                var todos = await service.GetTodosAsync();
                logger.LogInformation("GET /api/todo - Returning {Count} todos", ((List<Todo>)todos).Count);
                return Results.Ok(todos);
            })
                .Produces<IEnumerable<Todo>>();

            group.MapPost("/", async (CreateTodoDto todo, ITodoService service, ILogger<Program> logger) =>
            {
                var errors = new Dictionary<string, string[]>();

                if (string.IsNullOrWhiteSpace(todo.Title))
                    errors["Title"] = ["Title is required"];
                else if (todo.Title.Length > 100)
                    errors["Title"] = ["Title must not exceed 100 characters"];

                if (todo.Description?.Length > 500)
                    errors["Description"] = ["Description must not exceed 500 characters" ];

                if (errors.Count > 0)
                {
                    logger.LogWarning("POST /api/todo - Validation failed: {@ValidationErrors}", errors);
                    throw new TodoValidationException("Validation failed", errors);
                }

                var createdTodo = await service.CreateTodoAsync(todo);
                logger.LogInformation("POST /api/todo - Todo created, Response: 201");
                return Results.Created($"/api/todo/{createdTodo.Id}", createdTodo);
            })
                .Produces<Todo>(200)
                .Produces(400);

            group.MapPatch("/{id}", async (int id, ITodoService service, ILogger<Program> logger) =>
            {
                var todo = await service.ToggleTodoAsync(id);
                logger.LogInformation("PATCH /api/todo/{TodoId} - Todo toggled successfully", id);
                return Results.Ok(todo);
            })
                .Produces<Todo>(200)
                .Produces(404);
        }
    }
}
