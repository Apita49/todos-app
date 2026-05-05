import axios, { AxiosError } from "axios";
import type {AxiosResponse} from "axios";
import type { Todo, CreateTodoDto } from "../types/todo.ts";
import type { ValidationError, ApiError } from "../types/errors.ts";

const API_URL = import.meta.env.VITE_API_URL;
const API_ENDPOINT = '/todo';

const apiClient = axios.create({
    baseURL:API_URL,
    headers: {'Content-Type': "application/json"}
});

apiClient.interceptors.response.use(
    (response) => response,
    (error: AxiosError) => {
        if (error.response?.status === 400) {
            const data = error.response.data as any;
            if (data && typeof data === 'object' && 'errors' in data) {
                const validationError: ValidationError = {
                    type: 'validation',
                    message: data.message || 'Validation failed',
                    errors: data.errors || {},
                };
                return Promise.reject(validationError);
            }
        }

        const apiError: ApiError = {
            type: 'api',
            message: 'An error occurred',
            status: error.response?.status || 0,
        };

        console.error('API Error:', {
            status: error.response?.status,
            message: error.message,
            data: error.response?.data,
        });

        return Promise.reject(apiError);
    }
);

const todoService = {
    getTodos : async (): Promise<Todo[]> =>{
        const response : AxiosResponse<Todo[]> = await apiClient.get(API_ENDPOINT);
        return response.data;
    },

    createTodo: async(data:CreateTodoDto): Promise<Todo> => {
        const response : AxiosResponse<Todo> = await apiClient.post(API_ENDPOINT, data);
        return response.data;
    },

    toggleTodo: async(id:number): Promise<Todo> => {
        const response : AxiosResponse<Todo> = await apiClient.patch(API_ENDPOINT+`/${id}`);
        return response.data;
    },
};

export default todoService;
