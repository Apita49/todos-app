# Todos Backend

A modern **ASP.NET Core 8.0** todo application API built with **Clean Architecture** principles. This backend provides RESTful endpoints for managing todos and is designed to work seamlessly with a React frontend.

## Tech Stack

- **Framework**: ASP.NET Core 8.0 (latest LTS)
- **Language**: C# 12 with nullable reference types
- **Database**: Entity Framework Core 8.0 with SQLite
- **API Documentation**: Swagger/OpenAPI with Swashbuckle
- **Architecture**: Clean Architecture (layered approach)
- **Runtime**: .NET 8.0

## Prerequisites

Before running this project, ensure you have:

- **.NET 8.0 SDK** or later
- **Git** (for cloning the repository)
- Optional: **Visual Studio 2022** or **Visual Studio Code** with C# extension

### Verify Installation

```bash
dotnet --version  # Should show 8.0.x or higher
```

## Project Structure

This project follows **Clean Architecture** with five separate projects:

```
Todos.Backend/
├── Todos.Presentation/          # API minimal
│   ├── Program.cs               # Application entry point
│   ├── TodosEndpoint.cs         # API endpoint definitions
│   └── Todos.API.csproj
│
├── Todos.Service/               # Business logic & services
│   ├── ITodoService.cs          # Service interface
│   ├── TodoService.cs           # Core operations
│   ├── CreateTodoDto.cs         # Data transfer object
│   └── Todos.Service.csproj
│
├── Todos.Infrastructure/        # Data access layer
│   ├── AppDbContext.cs          # Entity Framework context
│   ├── Migrations/              # Database migrations
│   └── Todos.Infrastructure.csproj
│
├── Todos.Domain/                # Core entity
│   ├── Todo.cs                  # Todo entity model
│   └── Todos.Domain.csproj
│
├── Todos.ErrorHandler/          # Global exception handling
│   ├── ErrorHandler.cs          # Exception handling middleware
│   ├── Exceptions/              # Custom exception types
│   │   ├── TodoNotFoundException.cs
│   │   └── TodoValidationException.cs
│   └── Todos.ErrorHandler.csproj
│
├── Todos.Test/                  # Unit, integration, and error handler tests
│   ├── Service/                 # Service layer tests with mocked database
│   ├── Endpoints/               # API endpoint integration tests
│   ├── Middleware/              # Error handler middleware tests
│   └── Todos.Test.csproj
│
├── Todos.Backend.sln            # Solution file
├── appsettings.json             # Base configuration
├── appsettings.Development.json # Development-specific config
└── README.md
```

### Layer Responsibilities

- **Presentation**: Handles HTTP requests/responses and routing
- **Service**: Implements business logic and validation
- **Infrastructure**: Manages database access via Entity Framework Core
- **Domain**: Contains pure models (no dependencies)
- **MiddleWare - ErrorHandler**: Provides global exception handling middleware that catches all exceptions, logs them appropriately, and returns standardized JSON error responses with proper HTTP status codes. Handles both custom exceptions (TodoNotFoundException, TodoValidationException) and unexpected errors.

## Installation & Setup

### 1. Clone the Repository

```bash
git clone https://github.com/Apita49/todos-app.git
cd todos-app/Todos.Backend
```

### 2. Restore NuGet Packages

```bash
dotnet restore
```

### 3. Initialize the Database

Entity Framework Core will automatically create the SQLite database (`todos.db`) on first run with the existing migrations. No manual setup required.

**Database Location**: The `todos.db` file will be created in the `Todos.Presentation` project directory during first run.

**Connection String**: Configured in `appsettings.Development.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=todos.db"
}
```

### 4. Build the Project

```bash
dotnet build
```

### 5. Run the Application

```bash
dotnet run --project Todos.Presentation/Todos.API.csproj
```

Or using the simpler syntax from the Presentation directory:

```bash
cd Todos.Presentation
dotnet run
```

The application will start and display:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

## Swagger Implementation

Once the application is running, access the interactive **Swagger UI** at:

```
http://localhost:5000/swagger/index.html
```

This provides:
- Interactive testing of all endpoints
- Request/response schema information
- Try-it-out functionality

### API Base Route

All endpoints are prefixed with `/api/todo`

### Core Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/todo` | Retrieve all todos |
| `POST` | `/api/todo` | Create a new todo |
| `PATCH` | `/api/todo/{id}` | Toggle todo completion status |

**Visit Swagger for complete request/response examples and schemas.**

### Migrations

Database schema changes are managed via Entity Framework Core migrations:

```bash
# View migration history
dotnet ef migrations list --project Todos.Infrastructure

# The initial migration "InitialCreate" creates the Todos table
```

## Testing

The backend includes comprehensive test coverage using **NUnit** and **Moq**:

### Test Framework

- **NUnit 3.14** - Unit testing framework with fluent assertion syntax
- **Moq 4.20** - Mocking library for isolating dependencies
- **Microsoft.AspNetCore.Mvc.Testing** - Integration testing utilities for WebApplicationFactory
- **Entity Framework Core InMemory** - In-memory database provider for unit tests
- **NUnit3TestAdapter** - Test adapter for Visual Studio and command-line test runners

### Test Categories

1. **Service Tests** (`Todos.Test/Service/TodoServiceTests.cs`)
   - Unit tests for TodoService with mocked database
   - Tests CRUD operations, business logic, and error scenarios
   - Uses in-memory database for isolated service layer testing

2. **Integration Tests** (`Todos.Test/Endpoints/TodosEndpointsTests.cs`)
   - Tests all REST API endpoints (GET, POST, PATCH)
   - Uses WebApplicationFactory for full application context testing
   - Validates input validation, response formats, and error handling

3. **Error Handler Tests** (`Todos.Test/Middleware/ErrorHandlerTests.cs`)
   - Tests global exception handling middleware
   - Validates correct HTTP status codes and error response formats
   - Tests custom exceptions (TodoNotFoundException, TodoValidationException)

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter Todos.Test.Service.TodoServiceTests

# Run with test output
dotnet test --logger "console;verbosity=detailed"
```

## Project Next Steps

1. **Authentication/Authorization**
   - Currently anyone can access the API
   - Consider adding JWT authentication for production
   - Implement per-user todo filtering

2. **Logging Enhancement**
   - Currently using Microsoft's built-in logging facade
   - Consider migrating to **Serilog** for more advanced features

3. **Database**
   - Consider PostgreSQL or SQL Server for production
   - Add connection pooling for multiple concurrent requests
   - Implement database backups

4. **Documentation**
   - Add XML doc comments to service methods
   - Document API response codes (200, 201, 400, 404, 500)
 