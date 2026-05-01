using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Todos.Domain;
using Todos.Infrastructure;
using Todos.Middlewares.Exceptions;
using Todos.Service;

namespace Todos.Test.Service
{
    [TestFixture]
    public class TodoServiceTests
    {
        private AppDbContext _context;
        private Mock<ILogger<TodoService>> _mockLogger;
        private TodoService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _mockLogger = new Mock<ILogger<TodoService>>();
            _service = new TodoService(_context, _mockLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        #region GetTodosAsync Tests

        [Test]
        public async Task GetTodosAsync_WithMultipleTodos_ReturnsAllTodos()
        {            
            _context.Todos.Add(new Todo { Title = "Todo 1", Description = "Desc 1", IsDone = false });
            _context.Todos.Add(new Todo { Title = "Todo 2", Description = "Desc 2", IsDone = true });
            _context.Todos.Add(new Todo { Title = "Todo 3", Description = "Desc 3", IsDone = false });
            await _context.SaveChangesAsync();
                        
            var result = await _service.GetTodosAsync();

            Assert.Multiple(() =>
            {                
                Assert.That(result.Count(), Is.EqualTo(3));
                Assert.That(result.First().Title, Is.EqualTo("Todo 1"));
            });
        }

        [Test]
        public async Task GetTodosAsync_WithEmptyDatabase_ReturnsEmptyList()
        {            
            var result = await _service.GetTodosAsync();
            
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        #endregion

        #region CreateTodoAsync Tests

        [Test]
        public async Task CreateTodoAsync_WithValidData_CreatesTodoSuccessfully()
        {            
            var createDto = new CreateTodoDto("Test Title", "Test Description");
                        
            var result = await _service.CreateTodoAsync(createDto);

            Assert.Multiple(() =>
            {                
                Assert.That(result.Title, Is.EqualTo(createDto.Title));
                Assert.That(result.Description, Is.EqualTo(createDto.Description));
                Assert.That(result.IsDone, Is.False);
                Assert.That(result.Id, Is.GreaterThan(0));
            });
        }

        [Test]
        public async Task CreateTodoAsync_WithoutDescription_CreatesWithEmptyDescription()
        {            
            var createDto = new CreateTodoDto("Test Title");
            
            var result = await _service.CreateTodoAsync(createDto);

            Assert.Multiple(() =>
            {                
                Assert.That(result.Title, Is.EqualTo(createDto.Title));
                Assert.That(result.Description, Is.EqualTo(string.Empty));
                Assert.That(result.IsDone, Is.False);
                Assert.That(result.Id, Is.GreaterThan(0));
            });
        }

        #endregion

        #region ToggleTodoAsync Tests

        [Test]
        public async Task ToggleTodoAsync_WithExistingTodo_TogglesDoneStatus()
        {            
            var todo = new Todo { Title = "Test", Description = "Desc", IsDone = false };
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();
            int todoId = todo.Id;
            
            var result = await _service.ToggleTodoAsync(todoId);
            
            Assert.That(result.IsDone, Is.True);
        }

        [Test]
        public async Task ToggleTodoAsync_ToggleFromTrueToFalse_ReversesDoneStatus()
        {            
            var todo = new Todo { Title = "Test", Description = "Desc", IsDone = true };
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();
            int todoId = todo.Id;
            
            var result = await _service.ToggleTodoAsync(todoId);
            
            Assert.That(result.IsDone, Is.False);
        }

        [Test]
        public void ToggleTodoAsync_WithNonExistentTodo_ThrowsTodoNotFoundException()
        {
            Assert.ThrowsAsync<TodoNotFoundException>(() => _service.ToggleTodoAsync(999));
        }

        #endregion
    }
}
