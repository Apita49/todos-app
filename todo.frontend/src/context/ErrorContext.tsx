import { createContext, useContext, useState } from "react";
import type { ReactNode } from "react"

export interface FieldErrors {
  [fieldName: string]: string[];
}

interface ErrorContextType {
  modalError: string | null;
  fieldErrors: FieldErrors;
  setModalError: (error: string | null) => void;
  setFieldErrors: (errors: FieldErrors) => void;
  clearFieldError: (fieldName: string) => void;
}

const ErrorContext = createContext<ErrorContextType | undefined>(undefined);

export function ErrorProvider({ children }: { children: ReactNode }) {
  const [modalError, setModalError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});

  const clearFieldError = (fieldName: string) => {
    setFieldErrors((prev) => {
      const updated = { ...prev };
      delete updated[fieldName];
      return updated;
    });
  };

  return (
    <ErrorContext.Provider
      value={{
        modalError,
        fieldErrors,
        setModalError,
        setFieldErrors,
        clearFieldError,
      }}
    >
      {children}
    </ErrorContext.Provider>
  );
}

export function useError() {
  const context = useContext(ErrorContext);
  if (!context) {
    throw new Error("useError must be used within ErrorProvider");
  }
  return context;
}
