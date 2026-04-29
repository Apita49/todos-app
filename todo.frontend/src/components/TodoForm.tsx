import {useState} from "react";
import type {FormEvent} from "react";
import type {TodoFormProps, CreateTodoDto} from "../types/todo.ts";

export const TodoForm : React.FC<TodoFormProps> = ({ onSubmit }) => {
    const [title, setTitle] = useState<string>('');
    const [description, setDescription] = useState<string>('');
    const [isSubmitting, setIsSubmitting] = useState<boolean>(false);

    const handleSubmit = async (e : FormEvent<HTMLFormElement>):Promise<void> => {
        e.preventDefault();
        if (!title.trim()) return;

        setIsSubmitting(true);
        try{
            const data: CreateTodoDto = {
                title : title.trim(),
                description : description.trim() || undefined,
            };
            await onSubmit(data);
            setTitle('');
            setDescription('');
        } catch(err){
            console.error(err)
        } finally {
            setIsSubmitting(false);
        }
    }
    return (
        <form onSubmit={handleSubmit} className="todo-form">
            <div className="todo-form-row">
                <input
                type="text"
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                placeholder="Todo Title"
                required
                disabled={isSubmitting}
                />
                <button type="submit" disabled={isSubmitting}>
                    {isSubmitting? 'Creating...' : 'Add Task'}
                </button>
            </div>
            <div className="todo-form-row">
                <textarea
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                placeholder="Description (Optional)"
                disabled={isSubmitting}
                className="todo-form-description"
                />
            </div>
        </form>
    );
};