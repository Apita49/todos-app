import { useError } from "../context/ErrorContext";
import "./ErrorModal.css";

export function ErrorModal() {
  const { modalError, setModalError } = useError();

  if (!modalError) return null;

  return (
    <div className="error-modal-overlay" onClick={() => setModalError(null)}>
      <div className="error-modal" onClick={(e) => e.stopPropagation()}>
        <div className="error-modal-body">
          <p>{modalError}</p>
          <button
            className="error-modal-button"
            onClick={() => setModalError(null)}
          >
            Ok
          </button>
        </div>
      </div>
    </div>
  );
}
