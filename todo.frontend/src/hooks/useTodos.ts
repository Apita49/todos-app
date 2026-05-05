import {useState, useEffect} from 'react';
import todoService from '../services/todoService.ts';
import { useError } from '../context/ErrorContext';
import type {Todo, CreateTodoDto} from "../types/todo.ts";
import { isValidationError, isApiError } from '../types/errors';

interface UseTodosReturn{
    todos : Todo[];
    loading : boolean;
    addTodo : (data : CreateTodoDto) => Promise<Todo>;
    toggleTodo : (id : number) => Promise<void>;
    refreshTodos : () => Promise<void>
}

export const useTodos = () : UseTodosReturn =>{
    const [todos, setTodos] = useState<Todo[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const { setModalError, setFieldErrors } = useError();

    useEffect(()=>{
        loadTodos();
    },[])

    const handleError = (err: unknown) => {
        if (isValidationError(err)) {
            setFieldErrors(err.errors);
        } else if (isApiError(err)) {
            setModalError(err.message);
        } else {
            setModalError('An error occurred');
        }
    };

    const loadTodos = async() : Promise<void> =>{
        try {
            setLoading(true);
            const data = await todoService.getTodos();
            setTodos(data);
        } catch (err){
            handleError(err);
            console.error("Error loading todos", err)
        } finally{
            setLoading(false)
        }
    }

    const addTodo = async (data : CreateTodoDto) : Promise<Todo> => {
        try {
            const newTodo = await todoService.createTodo(data);
            setTodos(prev => [...prev, newTodo]);
            setFieldErrors({});
            return newTodo;
        } catch(err) {
            handleError(err);
            throw err;
        }
    }

    const toggleTodo = async (id: number) : Promise<void> =>{
        try {
            const updatedTodo = await todoService.toggleTodo(id);
            setTodos(prev => prev.map(todo => todo.id === id ? updatedTodo : todo));
        } catch (err) {
            handleError(err);
            throw err;
        }
    }

    return {
        todos,
        loading,
        addTodo,
        toggleTodo,
        refreshTodos:loadTodos,
    };
};