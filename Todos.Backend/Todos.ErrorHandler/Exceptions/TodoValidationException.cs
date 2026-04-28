namespace Todos.Middlewares.Exceptions;

public class TodoValidationException : Exception
{
    public Dictionary<string, string[]> Errors { get; }

    public TodoValidationException(string message, Dictionary<string, string[]> errors)
        : base(message)
    {
        Errors = errors;
    }
}
