using System.ComponentModel;

namespace ProjectName.Shared.Contracts;

/// <summary>
/// Specifies the category of an API error.
/// </summary>
public enum ErrorType
{
    [Description("VALIDATION_ERROR")]
    Validation,
    [Description("RESOURCE_NOT_FOUND")]
    NotFound,
    Unauthorized,
    Forbidden,
    Conflict,
    [Description("INTERNAL_ERROR")]
    Internal
}

/// <summary>
/// Represents an application error returned by an API.
/// </summary>
/// <remarks>Contains a domain-specific error code, a human-readable message, an error category, and optional
/// field-level validation details.</remarks>
public class ApiError
{
    /// <summary>
    /// Business / domain error code (e.g., USER_404, ACC_001)
    /// </summary>
    public string Code { get; set; } = default!;

    /// <summary>
    /// Human-readable message
    /// </summary>
    public string Message { get; set; } = default!;

    /// <summary>
    /// High-level error category
    /// </summary>
    public ErrorType Type { get; set; }

    /// <summary>
    /// Optional field-level validation errors
    /// </summary>
    public List<ErrorDetails>? Details { get; set; }
}

/// <summary>
/// Provides details about an API error, including the affected field, a descriptive message, and an error code.
/// </summary>
public class ErrorDetails
{
    /// <summary>
    /// Gets or sets the field.
    /// </summary>
    public string Field { get; set; } = default!;

    /// <summary>
    /// Gets or sets the message describing the API error.
    /// </summary>
    public string Message { get; set; } = default!;
    public string Code { get; set; } = default!;   // Optional but useful
}