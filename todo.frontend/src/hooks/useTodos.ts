import {useState, useEffect} from 'react';
import todoService from '../services/todoService.ts';
import type {Todo, CreateTodoDto} from "../types/todo.ts";

interface UseTodosReturn{
    todos : Todo[];
    loading : boolean;
    error : string | null;
    addTodo : (data : CreateTodoDto) => Promise<Todo>;
    toggleTodo : (id : number) => Promise<void>;
    refreshTodos : () => Promise<void>
}

export const useTodos = () : UseTodosReturn =>{
    const [todos, setTodos] = useState<Todo[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string|null>(null);

    useEffect(()=>{
        loadTodos();
    },[])

    const loadTodos = async() : Promise<void> =>{
        try {
            setLoading(true);
            const data = await todoService.getTodos();
            setTodos(data);
            setError(null)
        } catch (err){
            const message = err instanceof Error ? err.message : "Unkown error";
            setError(message);
            console.error("Error loading", err)
        } finally{
            setLoading(false)
        }
    }

    const addTodo = async (data : CreateTodoDto) : Promise<Todo> => {
        try {
            const newTodo = await todoService.createTodo(data);
            setTodos(prev => [...prev, newTodo]);
            return newTodo;
        } catch(err) {
            const message = err instanceof Error ? err.message : "Unkown error";
            setError(message);
            throw err;
        }
    }

    const toggleTodo = async (id: number) : Promise<void> =>{
        try {
            const updatedTodo = await todoService.toggleTodo(id);
            setTodos(prev => prev.map(todo => todo.id === id ? updatedTodo : todo));
        } catch (err) {
            const message = err instanceof Error ? err.message : "Unkown error";
            setError(message);
            throw err;            
        }
    }

    return {
        todos,
        loading,
        error,
        addTodo,
        toggleTodo,
        refreshTodos:loadTodos,
    };
};