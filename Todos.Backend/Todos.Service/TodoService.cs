using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Todos.Domain;
using Todos.Infrastructure;
using Todos.Middlewares.Exceptions;

namespace Todos.Service
{
    public class TodoService : ITodoService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TodoService> _logger;

        public TodoService(AppDbContext context, ILogger<TodoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Todo> CreateTodoAsync(CreateTodoDto todo)
        {
            var newTodo = new Todo { Description = todo.Description, Title = todo.Title };
            _context.Todos.Add(newTodo);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Todo created successfully with ID: {TodoId}, Title: {Title}", newTodo.Id, newTodo.Title);
            return newTodo;
        }

        public async Task<IEnumerable<Todo>> GetTodosAsync()
        {
            var todos = await _context.Todos.ToListAsync();
            _logger.LogInformation("Retrieving all todos, found {Count} items", todos.Count);
            return todos;
        }

        public async Task<Todo> ToggleTodoAsync(int id)
        {
            var patchedTodo = await _context.Todos.FindAsync(id);
            if (patchedTodo == null)
            {
                _logger.LogWarning("Todo with ID {TodoId} not found for toggle operation", id);
                throw new TodoNotFoundException(id);
            }

            var oldStatus = patchedTodo.IsDone;
            patchedTodo.IsDone = !patchedTodo.IsDone;
            _logger.LogDebug("Toggling todo {TodoId} from {OldStatus} to {NewStatus}", id, oldStatus, patchedTodo.IsDone);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Todo updated successfully with ID: {TodoId}, Title: {Title}", patchedTodo.Id, patchedTodo.Title);
            return patchedTodo;
        }
    }
}
