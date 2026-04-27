namespace Todos.Service
{
    public record CreateTodoDto(
        string Title,
        string Description = ""
        );
}
