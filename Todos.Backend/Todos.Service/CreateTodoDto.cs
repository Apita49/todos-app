using System.ComponentModel.DataAnnotations;

namespace Todos.Service
{
    public record CreateTodoDto(
        [StringLength(100)] string Title,
        [StringLength(500)] string Description = ""
    );
}
