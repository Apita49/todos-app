using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using Todos.Middlewares;
using Todos.Middlewares.Exceptions;

namespace Todos.Test.Middleware
{
    [TestFixture]
    public class ErrorHandlerTests
    {
        private Mock<ILogger<ErrorHandler>> _mockLogger;
        private const string _validationFailed = "Validation failed";
        private const string _requiredTitle = "Title is required";
        private const string _title = "Title";
        private const string _requiredDescription = "Description too long";
        private const string _description = "Description";
        private const string _message = "message";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mockLogger = new Mock<ILogger<ErrorHandler>>();
        }

        #region TodoNotFoundException Tests

        [Test]
        public async Task InvokeAsync_TodoNotFoundException_Returns404AndCorrectId()
        {
            var context = CreateHttpContext();
            var next = new RequestDelegate(_ => throw new TodoNotFoundException(42));
            var errorHandler = new ErrorHandler(next, _mockLogger.Object);

            await errorHandler.InvokeAsync(context);

            var responseBody = await ReadResponseBody(context);
            var json = JsonDocument.Parse(responseBody);
            var message = json.RootElement.GetProperty(_message).GetString();
            Assert.Multiple(() =>
            {
                Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
                Assert.That(message, Does.Contain("not found").IgnoreCase);
                Assert.That(message, Does.Contain("42"));
            });
        }

        #endregion

        #region TodoValidationException Tests

        [Test]
        public async Task InvokeAsync_TodoValidationException_IncludesErrorsInResponse()
        {
            var errors = new Dictionary<string, string[]> { { _title, new[] { _requiredTitle } } };
            var context = CreateHttpContext();
            var next = new RequestDelegate(_ => throw new TodoValidationException(_validationFailed, errors));
            var errorHandler = new ErrorHandler(next, _mockLogger.Object);

            await errorHandler.InvokeAsync(context);

            var responseBody = await ReadResponseBody(context);
            var jsonTitle = JsonDocument.Parse(responseBody).RootElement.GetProperty("errors").GetProperty(_title);
            Assert.Multiple(() =>
            {
                Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(jsonTitle[0].GetString(), Is.EqualTo(_requiredTitle));
            });
        }

        [Test]
        public async Task InvokeAsync_TodoValidationException_WithMultipleErrors_ReturnsAllErrors()
        {
            var errors = new Dictionary<string, string[]>
            {
                { _title, new[] { _requiredTitle } },
                { _description, new[] { _requiredDescription } }
            };
            var context = CreateHttpContext();
            var next = new RequestDelegate(_ => throw new TodoValidationException(_validationFailed, errors));
            var errorHandler = new ErrorHandler(next, _mockLogger.Object);

            await errorHandler.InvokeAsync(context);

            var responseBody = await ReadResponseBody(context);
            var json = JsonDocument.Parse(responseBody);
            var errorsElement = json.RootElement.GetProperty("errors");
            var title = errorsElement.GetProperty(_title);
            var description = errorsElement.GetProperty(_description);
            Assert.Multiple(() =>
            {
                Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(title[0].GetString(), Is.EqualTo(_requiredTitle));
                Assert.That(description[0].GetString(), Is.EqualTo(_requiredDescription));
            });
        }

        #endregion

        #region SqliteException Tests

        [Test]
        public async Task InvokeAsync_SqliteException_Returns500()
        {
            var context = CreateHttpContext();
            var next = new RequestDelegate(_ => throw new SqliteException("Database error", 1));
            var errorHandler = new ErrorHandler(next, _mockLogger.Object);

            await errorHandler.InvokeAsync(context);

            var responseBody = await ReadResponseBody(context);
            var json = JsonDocument.Parse(responseBody);
            Assert.Multiple(() =>
            {
                Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
                Assert.That(json.RootElement.GetProperty(_message).GetString(), Is.EqualTo("A database error occurred"));
            });
        }

        #endregion

        #region Generic Exception Tests

        [Test]
        public async Task InvokeAsync_GenericException_Returns500()
        {
            var context = CreateHttpContext();
            var next = new RequestDelegate(_ => throw new Exception("Something broke"));
            var errorHandler = new ErrorHandler(next, _mockLogger.Object);

            await errorHandler.InvokeAsync(context);

            var responseBody = await ReadResponseBody(context);
            var json = JsonDocument.Parse(responseBody);
            Assert.Multiple(() =>
            {
                Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
                Assert.That(json.RootElement.GetProperty(_message).GetString(), Is.EqualTo("An unexpected error occurred"));
            });
        }

        #endregion

        #region Helper Methods

        private static HttpContext CreateHttpContext()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();
            return httpContext;
        }

        private static async Task<string> ReadResponseBody(HttpContext context)
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(context.Response.Body);
            return await reader.ReadToEndAsync();
        }

        #endregion
    }
}
