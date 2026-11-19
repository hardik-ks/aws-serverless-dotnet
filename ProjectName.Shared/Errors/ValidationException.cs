using ProjectName.Shared.Contracts;

namespace ProjectName.Shared.Errors;

/// <summary>
/// Represents an error that occurs when request validation fails.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Gets the error details associated with the validation exception.
    /// </summary>
    public List<ErrorDetails> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the ValidationException class with a specified error message and validation
    /// errors.
    /// </summary>
    /// <param name="message">The message that describes the validation failure.</param>
    /// <param name="errors">The validation errors associated with the exception.</param>
    public ValidationException(string message, List<ErrorDetails> errors)
        : base(message)
    {
        Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="errors">The validation errors associated with the request.</param>
    public ValidationException(List<ErrorDetails> errors)
        : base("Request validation failed")
    {
        Errors = errors;
    }
}
