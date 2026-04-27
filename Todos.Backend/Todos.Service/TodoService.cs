using Microsoft.EntityFrameworkCore;
using Todos.Domain;
using Todos.Infrastructure;

namespace Todos.Service
{
    public class TodoService : ITodoService
    {
        private readonly AppDbContext _context;

        public TodoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Todo> CreateTodoAsync(CreateTodoDto todo)
        {
            var newTodo = new Todo { Description = todo.Description, Title = todo.Title };
            _context.Todos.Add(newTodo);
            await _context.SaveChangesAsync();
            return newTodo;
        }

        public async Task<IEnumerable<Todo>> GetTodosAsync()
        {
            return await _context.Todos.ToListAsync();
        }

        public async Task<Todo> ToggleTodoAsync(int id)
        {
            var patchedTodo = await _context.Todos.FindAsync(id);
            if (patchedTodo == null) return null;

            patchedTodo.IsDone = !patchedTodo.IsDone;
            await _context.SaveChangesAsync();
            return patchedTodo;
        }
    }
}
