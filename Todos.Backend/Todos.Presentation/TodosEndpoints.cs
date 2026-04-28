using Todos.Domain;
using Todos.Service;

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
                if (string.IsNullOrWhiteSpace(todo.Title))
                    return Results.BadRequest("Title is required");

                if (todo.Title.Length > 100)
                    return Results.BadRequest("Title must not exceed 100 characters");

                if (todo.Description.Length > 500)
                    return Results.BadRequest("Description must not exceed 500 characters");

                var createdTodo = await service.CreateTodoAsync(todo);
                return Results.Created($"/api/todo/{createdTodo.Id}", createdTodo);
            })
                .Produces<Todo>(200)
                .Produces(400);

            group.MapPatch("/{id}", async (int id, ITodoService service) =>
            {
                var todo = await service.ToggleTodoAsync(id);
                if (todo == null)
                    return Results.NotFound();
                return Results.Ok(todo);
            })
                .Produces<Todo>(200)
                .Produces(404);
        }
    }
}
