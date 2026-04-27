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

This project follows **Clean Architecture** with four separate projects:

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

## Project Next Steps

1. **Input Validation**
   - Add more comprehensive validation on `CreateTodoDto` (e.g., max length for Title)
   - Return 400 Bad Request with detailed error messages for validation failures

2. **Error Handling**
   - Implement global exception handling middleware
   - Return consistent error response format (currently returns default ASP.NET Core responses)
   - Add try-catch blocks in service methods to handle unexpected database errors

3. **Authentication/Authorization**
   - Currently anyone can access the API
   - Consider adding JWT authentication for production
   - Implement per-user todo filtering

4. **Logging**
   - Current logging is minimal
   - Add structured logging (e.g., Serilog) to track API requests and errors
   - Useful for debugging and monitoring

5. **Testing**
   - Add unit tests for `TodoService`
   - Add integration tests using an in-memory database
   - Test edge cases like creating todos with empty titles

6. **Database**
   - Consider PostgreSQL or SQL Server for production
   - Add connection pooling for multiple concurrent requests
   - Implement database backups

7. **Documentation**
   - Add XML doc comments to service methods
   - Document API response codes (200, 201, 400, 404, 500)
