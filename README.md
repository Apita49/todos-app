# Todos App

A full-stack todo management application with a modern React frontend and ASP.NET Core backend. Demonstrates clean architecture principles, type safety across API boundaries, and a scalable service-oriented design.

**Tech Stack:** React 19 • TypeScript • Vite • Axios • ASP.NET Core 8.0 • Entity Framework Core • SQLite

---

## Quick Start

### Frontend
Navigate to `todo.frontend/` and run:
```bash
npm install
npm run dev
```
Frontend runs on `http://localhost:5173`. See [Frontend README](todo.frontend/README.md) for details.

### Backend
Navigate to `Todos.Backend/` and run:
```bash
dotnet restore
dotnet run --project Todos.Presentation/Todos.API.csproj
```
Backend runs on `http://localhost:5000` with Swagger UI at `/swagger/index.html`. See [Backend README](Todos.Backend/README.md) for details.

### Environment
Create `.env` in `todo.frontend/`:
```env
VITE_API_URL=https://localhost:5000/api
```

---

## Architecture & Integration

### System Overview
```
┌─────────────────────┐
│   React Components  │ (TodoForm, TodoList, TodoItem)
├─────────────────────┤
│   useTodos Hook     │ (State management, loading, errors)
├─────────────────────┤
│   todoService       │ (Axios client with typed requests/responses)
├─────────────────────┤
│   REST API Layer    │ (ASP.NET Core Minimal APIs, /api/todo endpoints)
├─────────────────────┤
│   Service Layer     │ (Business logic, TodoService)
├─────────────────────┤
│   Infrastructure    │ (Entity Framework Core with SQLite)
├─────────────────────┤
│   Domain Models     │ (Todo entity)
└─────────────────────┘
```

### Key Design Patterns

**Type Safety Across Boundaries**
- Frontend defines TypeScript interfaces (`src/types/todo.ts`) matching backend DTOs
- Axios client in `todoService.ts` provides compile-time contract verification
- Backend uses C# 12 with nullable reference types and Clean Architecture layering

**Frontend Architecture**
- **Components**: Presentational logic only (TodoForm, TodoList, TodoItem)
- **Hooks**: State management via `useTodos` encapsulates API calls and loading states
- **Services**: Centralized Axios client for all API communication
- **Types**: Shared interfaces ensure consistency with backend responses

**Backend Architecture (Clean Architecture)**
- **Presentation**: HTTP handlers + routing (ASP.NET Core Minimal APIs)
- **Service**: Business logic and validation (ITodoService, TodoService)
- **Infrastructure**: Data persistence via Entity Framework Core
- **Domain**: Core models with no external dependencies (Todo entity)

---

## API Contract

All endpoints prefixed with `/api/todo`:

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/todo` | Retrieve all todos |
| `POST` | `/api/todo` | Create a new todo with title & optional description |
| `PATCH` | `/api/todo/{id}` | Toggle todo completion status |

Full schema and interactive testing available via [Swagger UI](http://localhost:5000/swagger) when backend is running.

---

## Future Enhancements

### Core Features
- **Delete & Edit**: Remove todos or update title/description
- **Local Search**: Client-side filtering by title or content
- **Categorization**: Organize todos with tags or categories
- **Sorting**: By creation date, alphabetical order, or completion status

### Backend Infrastructure
- **Input Validation**: Comprehensive validation on `CreateTodoDto` with detailed error responses (400 Bad Request)
- **Error Handling**: Global exception middleware for consistent error response format
- **Authentication/Authorization**: JWT authentication for multi-user support with per-user todo filtering
- **Logging**: Structured logging (e.g., Serilog) for API requests and errors
- **Testing**: Unit tests for `TodoService`, integration tests with in-memory database

### Frontend UX & Quality
- **Offline Support**: Persist todos locally via localStorage as fallback
- **Loading States**: Skeleton screens for improved perceived performance
- **Notifications**: Toast messages for success and error feedback
- **Accessibility**: ARIA labels, keyboard navigation, screen reader support
- **Responsive Design**: Mobile-friendly layout with touch interactions
- **Advanced Features**: Drag-and-drop reordering, WebSocket real-time sync, state management upgrade (Context/Redux)

### Database & Deployment
- **Database Scaling**: PostgreSQL or SQL Server for production with connection pooling and backups
- **API Documentation**: XML doc comments on service methods, standardized response codes (200, 201, 400, 404, 500)

---

## Directory Structure

```
todos-app/
├── todo.frontend/              # React TypeScript frontend
│   ├── src/
│   │   ├── components/         # TodoForm, TodoList, TodoItem
│   │   ├── hooks/              # useTodos hook
│   │   ├── services/           # todoService (Axios client)
│   │   └── types/              # TypeScript interfaces
│   └── README.md               # Frontend-specific documentation
│
├── Todos.Backend/              # ASP.NET Core 8.0 backend
│   ├── Todos.Presentation/     # API layer (Minimal APIs)
│   ├── Todos.Service/          # Business logic
│   ├── Todos.Infrastructure/   # Data access (EF Core)
│   ├── Todos.Domain/           # Core models
│   └── README.md               # Backend-specific documentation
│
└── README.md                   # This file
```
