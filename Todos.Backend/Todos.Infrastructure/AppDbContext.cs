using Microsoft.EntityFrameworkCore;
using Todos.Domain;

namespace Todos.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Todo> Todos => Set<Todo>();
    }
}
