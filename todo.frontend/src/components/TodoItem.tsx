import React from "react";
import type {TodoItemProps} from "../types/todo.ts";

export const TodoItem: React.FC<TodoItemProps> = ({todo, onToggle}) =>{
    const handleToggle = () => {
        onToggle(todo.id);
    };

    return (
        <div className={`todo-item ${todo.isDone ? 'completed' : ''}`}>
            <input 
            type="checkbox"
            checked={todo.isDone}
            onChange={handleToggle}
            id={`todo-${todo.id}`}
            />
            <label
            htmlFor={`todo-${todo.id}`}
            onClick={handleToggle}
            style={{ cursor : 'pointer', flex: 1}}
            >
                <span className={todo.isDone ? 'completed' : ''}>
                    {todo.title}
                </span>
                {todo.description && (
                    <p className="todo-description">{todo.description}</p>
                )}
            </label>
        </div>
    );
};