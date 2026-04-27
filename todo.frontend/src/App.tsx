import {TodoForm} from "./components/TodoForm";
import {TodoList} from "./components/TodoList";
import {useTodos} from "./hooks/useTodos"
import './App.css';

function App() {
  const {todos, loading, error, addTodo, toggleTodo} = useTodos();
  if(loading){
    return <div className="loading">Loading...</div>;
  }
  if (error){
    return <div className="error">Error: {error}</div>;
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
};

export default App;
