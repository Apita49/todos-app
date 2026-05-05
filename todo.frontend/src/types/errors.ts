export interface ValidationError {
  type: "validation";
  message: string;
  errors: Record<string, string[]>;
}

export interface ApiError {
  type: "api";
  message: string;
  status: number;
}

export type AppError = ValidationError | ApiError;

export function isValidationError(error: unknown): error is ValidationError {
  return (
    typeof error === "object" &&
    error !== null &&
    "type" in error &&
    error.type === "validation"
  );
}

export function isApiError(error: unknown): error is ApiError {
  return (
    typeof error === "object" &&
    error !== null &&
    "type" in error &&
    error.type === "api"
  );
}
