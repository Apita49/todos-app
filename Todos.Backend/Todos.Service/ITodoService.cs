using Todos.Domain;
using Todos.Infrastructure;

namespace Todos.Service
{
    public interface ITodoService
    {
        Task<IEnumerable<Todo>> GetTodosAsync();
        Task<Todo> CreateTodoAsync(CreateTodoDto todo);
        Task<Todo> ToggleTodoAsync(int id);
    }
}
