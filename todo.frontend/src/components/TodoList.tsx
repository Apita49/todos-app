import React  from "react";
import {TodoItem} from "./TodoItem"
import type {TodoListProps} from "../types/todo.ts";

export const TodoList: React.FC<TodoListProps> = ({todos, onToggle}) => {
    if (todos.length === 0){
        return <p className="empty-state">No todos yet. Create one!</p>
    }

    return (
        <div className="todo-list">
            {todos.map(todo =>(
                <TodoItem
                onToggle={onToggle}
                todo={todo}
                key={todo.id}
                />
            ))}
        </div>
    );
};

