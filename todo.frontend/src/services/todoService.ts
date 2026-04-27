import axios from "axios";
import type {AxiosResponse} from "axios";
import type { Todo, CreateTodoDto } from "../types/todo.ts";

const API_URL = import.meta.env.VITE_API_URL;
const API_ENDPOINT = '/todo';

const apiClient = axios.create({
    baseURL:API_URL,
    headers: {'Content-Type': "application/json"}
});

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
