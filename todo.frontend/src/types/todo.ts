export interface Todo {
    id: number;
    title:string;
    description?:string;
    isDone:boolean;
}

export interface CreateTodoDto{
    title:string;
    description?:string;
}

export interface TodoListProps{
    todos:Todo[];
    onToggle: (todoId:number) => void;
}

export interface TodoItemProps{
    todo:Todo;
    onToggle: (todoId:number) => void;
}

export interface TodoFormProps{
    onSubmit: (todoData:CreateTodoDto) => Promise<Todo>;
}