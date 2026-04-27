namespace Todos.Domain
{
    public class Todo
    {
        public string Title { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Id { get; set; }
        public bool IsDone { get; set; }
    }
}
