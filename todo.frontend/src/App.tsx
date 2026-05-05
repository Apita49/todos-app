import {TodoForm} from "./components/TodoForm";
import {TodoList} from "./components/TodoList";
import {useTodos} from "./hooks/useTodos"
import { ErrorProvider } from "./context/ErrorContext";
import { ErrorModal } from "./components/ErrorModal";
import './App.css';

function AppContent() {
  const {todos, loading, addTodo, toggleTodo} = useTodos();
  if(loading){
    return <div className="loading">Loading...</div>;
  }
  return (
    <div className="app">
      <header>
        <h1>My Todos</h1>
      </header>
      <main>
        <TodoForm onSubmit={addTodo}/>
        <TodoList todos={todos} onToggle={toggleTodo}/>
      </main>
    </div>
  );
}

function App() {
  return (
    <ErrorProvider>
      <AppContent />
      <ErrorModal />
    </ErrorProvider>
  );
}

export default App;
