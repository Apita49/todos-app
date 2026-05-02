using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Todos.Domain;
using Todos.Service;
using Todos.Test.Fixtures;

namespace Todos.Test.Endpoints
{
    [TestFixture]
    public class TodosEndpointsTests
    {
        private TodosWebApplicationFactory _factory;
        private HttpClient _client;
        private const string _apiEndpoint = "/api/todo";

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _factory = new TodosWebApplicationFactory();
            await _factory.InitializeAsync();
            _client = _factory.CreateClient();
        }

        [SetUp]
        public async Task SetUp()
        {
            await _factory.ResetDatabaseAsync();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }

        #region GET /api/todo Tests

        [Test]
        public async Task GetTodos_ReturnsAllTodos()
        {
            var _todoTitleOne = "Todo 1";
            var _todoTitleTwo = "Todo 2";
            var dbContext = _factory.GetDbContext();
            dbContext.Todos.Add(new Todo { Title = _todoTitleOne, Description = "Desc 1", IsDone = false });
            dbContext.Todos.Add(new Todo { Title = _todoTitleTwo, Description = "Desc 2", IsDone = true });
            await dbContext.SaveChangesAsync();

            var response = await _client.GetAsync(_apiEndpoint);

            var todos = await response.Content.ReadFromJsonAsync<List<Todo>>();
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(todos, Has.Count.EqualTo(2));
                Assert.That(todos?[0].Title, Is.EqualTo(_todoTitleOne));
                Assert.That(todos?[1].Title, Is.EqualTo(_todoTitleTwo));
            });
        }

        [Test]
        public async Task GetTodos_WithEmptyDatabase_ReturnsEmptyArray()
        {
            var response = await _client.GetAsync(_apiEndpoint);

            var todos = await response.Content.ReadFromJsonAsync<List<Todo>>();
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(todos, Has.Count.EqualTo(0));
            });
        }

        #endregion

        #region POST /api/todo Tests

        [Test]
        public async Task CreateTodo_WithValidData_Returns201Created()
        {
            var createDto = new CreateTodoDto("Test Title", "Test Description");

            var response = await _client.PostAsJsonAsync(_apiEndpoint, createDto);

            var locationPath = response.Headers.Location?.ToString();
            var createdTodo = await response.Content.ReadFromJsonAsync<Todo>();
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
                Assert.That(locationPath, Does.Contain($"{_apiEndpoint}/"));
                Assert.That(createdTodo?.Title, Is.EqualTo(createDto.Title));
                Assert.That(createdTodo?.Description, Is.EqualTo(createDto.Description));
                Assert.That(createdTodo?.IsDone, Is.False);
                Assert.That(createdTodo?.Id, Is.GreaterThan(0));
            });
        }

        [Test]
        public async Task CreateTodo_WithoutDescription_ReturnsCreated()
        {
            var createDto = new CreateTodoDto("Test Title");

            var response = await _client.PostAsJsonAsync(_apiEndpoint, createDto);

            var createdTodo = await response.Content.ReadFromJsonAsync<Todo>();
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
                Assert.That(createdTodo?.Title, Is.EqualTo(createDto.Title));
                Assert.That(createdTodo?.Description, Is.EqualTo(string.Empty));
                Assert.That(createdTodo?.IsDone, Is.False);
                Assert.That(createdTodo?.Id, Is.GreaterThan(0));
            });
        }

        [Test]
        public async Task CreateTodo_WithNullTitle_Returns400BadRequest()
        {
            var invalidRequest = new { title = (string)null, description = "Test" };

            var response = await _client.PostAsJsonAsync(_apiEndpoint, invalidRequest);

            var errorResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
            var titleErrors = errorResponse.GetProperty("errors").GetProperty("Title");
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(titleErrors[0].GetString(), Does.Contain("Title is required").IgnoreCase);
            });
        }

        [Test]
        public async Task CreateTodo_WithEmptyTitle_Returns400BadRequest()
        {
            var createDto = new CreateTodoDto("", "Test Description");

            var response = await _client.PostAsJsonAsync(_apiEndpoint, createDto);

            var errorResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
            var titleErrors = errorResponse.GetProperty("errors").GetProperty("Title");
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(titleErrors[0].GetString(), Does.Contain("Title is required").IgnoreCase);
            });
        }

        [Test]
        public async Task CreateTodo_WithTitleTooLong_Returns400BadRequest()
        {
            var longTitle = new string('a', 101);
            var createDto = new CreateTodoDto(longTitle, "Test Description");

            var response = await _client.PostAsJsonAsync(_apiEndpoint, createDto);

            var errorResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
            var titleErrors = errorResponse.GetProperty("errors").GetProperty("Title");
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(titleErrors[0].GetString(), Does.Contain("100 characters").IgnoreCase);
            });
        }

        [Test]
        public async Task CreateTodo_WithDescriptionTooLong_Returns400BadRequest()
        {
            var longDescription = new string('a', 501);
            var createDto = new CreateTodoDto("Test Title", longDescription);

            var response = await _client.PostAsJsonAsync(_apiEndpoint, createDto);

            var errorResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
            var titleErrors = errorResponse.GetProperty("errors").GetProperty("Description");
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(titleErrors[0].GetString(), Does.Contain("500 characters").IgnoreCase);
            });
        }

        [Test]
        public async Task CreateTodo_WithMultipleValidationErrors_ReturnsAllErrors()
        {
            var longDescription = new string('a', 501);
            var invalidRequest = new CreateTodoDto("", longDescription);

            var response = await _client.PostAsJsonAsync(_apiEndpoint, invalidRequest);

            var errorResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
            var errors = errorResponse.GetProperty("errors");
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(errors.GetProperty("Title").GetArrayLength(), Is.GreaterThan(0));
                Assert.That(errors.GetProperty("Description").GetArrayLength(), Is.GreaterThan(0));
            });
        }

        #endregion

        #region PATCH /api/todo/{id} Tests

        [Test]
        public async Task ToggleTodo_WithExistingTodo_Returns200AndToggledTodo()
        {
            var dbContext = _factory.GetDbContext();
            var todo = new Todo { Title = "Test Title", Description = "Test Description", IsDone = false };
            dbContext.Todos.Add(todo);
            await dbContext.SaveChangesAsync();
            int todoId = todo.Id;

            var response = await _client.PatchAsync($"{_apiEndpoint}/{todoId}", null);

            var toggledTodo = await response.Content.ReadFromJsonAsync<Todo>();
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(toggledTodo?.Id, Is.EqualTo(todoId));
                Assert.That(toggledTodo?.IsDone, Is.True);
            });
        }

        [Test]
        public async Task ToggleTodo_TwiceShouldReverse_ReturnsFalse()
        {
            var dbContext = _factory.GetDbContext();
            var todo = new Todo { Title = "Test Title", Description = "Test Description", IsDone = false };
            dbContext.Todos.Add(todo);
            await dbContext.SaveChangesAsync();
            int todoId = todo.Id;

            var response = await _client.PatchAsync($"{_apiEndpoint}/{todoId}", null);
            var toggledTodo = await response.Content.ReadFromJsonAsync<Todo>();

            response = await _client.PatchAsync($"{_apiEndpoint}/{todoId}", null);
            var notToggledTodo = await response.Content.ReadFromJsonAsync<Todo>();

            Assert.Multiple(() =>
            {
                Assert.That(toggledTodo?.IsDone, Is.True);
                Assert.That(notToggledTodo?.IsDone, Is.False);
            });
        }

        [Test]
        public async Task ToggleTodo_WithNonExistentId_Returns404NotFound()
        {
            var response = await _client.PatchAsync($"{_apiEndpoint}/999", null);

            var errorResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
            var message = errorResponse.GetProperty("message").GetString();
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(message, Does.Contain("not found").IgnoreCase);
            });
        }

        #endregion
    }
}
