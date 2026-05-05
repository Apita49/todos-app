import {useState} from "react";
import type {FormEvent} from "react";
import type {TodoFormProps, CreateTodoDto} from "../types/todo.ts";
import { useError } from "../context/ErrorContext";
import "./TodoForm.css";

export const TodoForm : React.FC<TodoFormProps> = ({ onSubmit }) => {
    const [title, setTitle] = useState<string>('');
    const [description, setDescription] = useState<string>('');
    const [isSubmitting, setIsSubmitting] = useState<boolean>(false);
    const { fieldErrors, clearFieldError } = useError();

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

    const handleTitleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setTitle(e.target.value);
        if (fieldErrors['Title']) {
            clearFieldError('Title');
        }
    };

    const handleDescriptionChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
        setDescription(e.target.value);
        if (fieldErrors['Description']) {
            clearFieldError('Description');
        }
    };

    return (
        <form onSubmit={handleSubmit} className="todo-form">
            <div className="todo-form-row">
                <div className="todo-form-field">
                    <input
                    type="text"
                    value={title}
                    onChange={handleTitleChange}
                    placeholder="Todo Title"
                    required
                    disabled={isSubmitting}
                    className={fieldErrors['Title'] ? 'input-error' : ''}
                    />
                    {fieldErrors['Title'] && (
                        <div className="field-error">
                            {fieldErrors['Title'][0]}
                        </div>
                    )}
                </div>
                <button type="submit" disabled={isSubmitting}>
                    {isSubmitting? 'Creating...' : 'Add Task'}
                </button>
            </div>
            <div className="todo-form-row">
                <div className="todo-form-field">
                    <textarea
                    value={description}
                    onChange={handleDescriptionChange}
                    placeholder="Description (Optional)"
                    disabled={isSubmitting}
                    className={`todo-form-description ${fieldErrors['Description'] ? 'input-error' : ''}`}
                    />
                    {fieldErrors['Description'] && (
                        <div className="field-error">
                            {fieldErrors['Description'][0]}
                        </div>
                    )}
                </div>
            </div>
        </form>
    );
};