# React Todo Application

A TypeScript-based React frontend for managing todos via a .NET backend API. Built with Vite, featuring type-safe API integration and a modular component architecture.

**Tech Stack:** React 19 • TypeScript • Vite • Axios

---

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
- [Why TypeScript](#why-typescript)
- [Future Enhancements](#future-enhancements)

---

## Overview

This application provides a simple yet structured interface for managing todos. Users can create new todos with titles and optional descriptions, toggle completion status, and view their todo list in real-time. The frontend consumes a .NET backend API.

**Current Features:**
- Create todos with title and optional description
- Toggle todos between incomplete and complete states
- View all todos with their current status
- Global error handling with validation and API error distinction

---

## Architecture

### Directory Structure

```
src/
├── App.tsx                    # Root component, main app logic
├── App.css                    # App styles
├── main.tsx                   # Entry point
├── index.css                  # Global styles
├── components/
│   ├── ErrorModal.tsx         # Modal component for displaying generic errors
│   ├── TodoForm.tsx           # Form for creating new todos
│   ├── TodoItem.tsx           # Individual todo display with toggle
│   └── TodoList.tsx           # Container for todo items
├── context/
│   └── ErrorContext.tsx       # Global error state management
├── hooks/
│   └── useTodos.ts            # Custom hook for todo state management
├── services/
│   └── todoService.ts         # Axios client for API communication
└── types/
    ├── errors.ts              # Error type definitions
    └── todo.ts                # TypeScript interfaces and types
```

### Data Flow

```
Components (TodoForm, TodoList) 
    ↓
useTodos Hook (state management + error handling)
    ↓
todoService (API client with Axios + error interceptor)
    ↓
.NET Backend API
```

### Key Patterns

**Service Layer:** All API calls are centralized in `todoService.ts`, providing a single source of truth for API communication. The service uses Axios with typed responses for compile-time safety.

**Custom Hook:** The `useTodos` hook encapsulates all todo-related state and logic, including loading states and error handling. Components consume this hook to access todos and trigger actions.

**Type Safety:** Full TypeScript coverage ensures that API responses match expected shapes. Key interfaces:
- `Todo`: Represents a complete todo item with id, title, optional description, and completion status
- `CreateTodoDto`: Data transfer object for creating new todos
- Component prop interfaces ensure type-safe component communication

### Error Handling Architecture

The application implements a two-tier error handling system that distinguishes between validation errors and API errors:

**Error Context** (`ErrorContext.tsx`)
- Global state management for all errors across the application
- Tracks two error types:
  - `fieldErrors`: Validation errors mapped to specific form fields (field name → error messages)
  - `modalError`: Generic API error messages displayed in a modal overlay

**Validation Errors (400 status codes)**
- Captured from API responses with field-level error mappings
- Displayed inline under problematic form fields with red styling
- Automatically cleared when the user interacts with the field
- Example: "Title must be less than 100 characters" appears below the title input

**API Errors (other failures)**
- Displayed in a modal overlay with a generic error message
- Provides a clear user-facing fallback for unexpected server failures
- Logged for debugging purposes
- Dismissed by clicking the modal overlay or the "Ok" button

**Error Flow**
1. API service detects errors and categorizes them using TypeScript discriminated unions
2. Error handler in `useTodos` hook routes errors to appropriate context setters
3. Components subscribe to `ErrorContext` and render errors accordingly
4. Form fields display validation errors inline; the modal displays API errors globally

---

## Getting Started

### Prerequisites

- Node.js 18+ and npm

### Installation

```bash
# Install dependencies
npm install
```

### Environment Configuration

Create a `.env` file in the project root:

```env
VITE_API_URL=https://localhost:5000/api
```

The `VITE_API_URL` is used by the API client to construct request URLs. Adjust as needed for your backend instance.

### Running the Development Server

```bash
npm run dev
```

The application will be available at `http://localhost:5173`

### Building for Production

```bash
npm run build
```

This runs TypeScript type checking and creates an optimized production build in the `dist` directory.

## Why TypeScript

TypeScript was chosen for this project to provide formal contracts between the frontend and backend, ensuring type safety across API boundaries.

### Trade-off

JavaScript would have allowed faster development with less boilerplate. However, the verbosity of TypeScript is a worthwhile investment because:

**Type Safety at Compile Time:** Any mismatch between expected API response shapes and actual consumption is caught immediately, preventing runtime errors.

**Clear API Contracts:** The interfaces in `src/types/todo.ts` explicitly document what data structures the backend provides. This serves as living documentation and prevents silent failures.

**Better Developer Experience:** IDE autocomplete and type hints reduce cognitive load when working with the API client and component props.

**Preventing Silent Bugs:** Without TypeScript, incorrect API response handling or prop passing might only surface in production. Type checking prevents these categories of bugs entirely.

## Future Enhancements

Potential improvements and feature additions for future iterations:

### Core Functionality
- **Delete todos** - Add backend endpoint and UI button to remove todos
- **Edit existing todos** - Implement edit mode with description and title updates
- **Local filtering/searching** - Add client-side search to filter todos by title or content
- **Todo categories or tags** - Extend schema and UI to organize todos into groups

### User Experience
- **Sorting options** - Sort by creation date, alphabetical order, or completion status
- **Persistence with localStorage** - Cache todos locally as a fallback when offline
- **Skeleton loading screens** - Improve perceived performance during data fetching
- **Toast notifications** - Add visual feedback for success and error messages
- **Improved responsive design** - Ensure mobile-friendly layout and touch interactions
- **Accessibility improvements** - Add ARIA labels, keyboard navigation, and screen reader support

### Advanced Features
- **Real-time sync with WebSockets** - Push updates from backend without polling
- **Authentication & user-specific todos** - Multi-user support with personalized todo lists
- **Drag-and-drop reordering** - Allow users to reorder todos by dragging
- **State management upgrade** - Migrate from custom hooks to Context API or Redux if complexity grows

### Testing & Quality
- **Unit tests with Vitest** - Test components and hooks in isolation
- **E2E tests with Playwright/Cypress** - Test complete user workflows
- **API error handling improvements** - Add retry logic and rate limiting awareness
