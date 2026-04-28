using System.ComponentModel.DataAnnotations;

namespace Todos.Domain
{
    public class Todo
    {
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public int Id { get; set; }
        public bool IsDone { get; set; }
    }
}
