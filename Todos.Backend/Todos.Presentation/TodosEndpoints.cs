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

            group.MapGet("/", async (ITodoService service) =>
            {
                var todos = await service.GetTodosAsync();
                return Results.Ok(todos);
            })
                .Produces<IEnumerable<Todo>>();

            group.MapPost("/", async (CreateTodoDto todo, ITodoService service) =>
            {
                var errors = new Dictionary<string, string[]>();

                if (string.IsNullOrWhiteSpace(todo.Title))
                    errors["Title"] = ["Title is required"];
                else if (todo.Title.Length > 100)
                    errors["Title"] = ["Title must not exceed 100 characters"];

                if (todo.Description?.Length > 500)
                    errors["Description"] = ["Description must not exceed 500 characters" ];

                if (errors.Count > 0)
                    throw new TodoValidationException("Validation failed", errors);

                var createdTodo = await service.CreateTodoAsync(todo);
                return Results.Created($"/api/todo/{createdTodo.Id}", createdTodo);
            })
                .Produces<Todo>(200)
                .Produces(400);

            group.MapPatch("/{id}", async (int id, ITodoService service) =>
            {
                var todo = await service.ToggleTodoAsync(id);
                return Results.Ok(todo);
            })
                .Produces<Todo>(200)
                .Produces(404);
        }
    }
}
