namespace ProjectName.Shared.Contracts;

/// <summary>
/// Provides pagination and metadata for an API response.
/// </summary>
public class ApiMetadata
{
    /// <summary>
    /// Gets or sets the page number.
    /// </summary>
    public int? Page { get; set; }
    
    /// <summary>
    /// Gets or sets the number of items to include in a page.
    /// </summary>
    public int? PageSize { get; set; }
    
    /// <summary>
    /// Gets or sets the total number of records.
    /// </summary>
    public int? TotalRecords { get; set; }

    /// <summary>
    /// Gets the total number of pages required for the available records.
    /// </summary>
    public int? TotalPages => (int)Math.Ceiling((double)(TotalRecords ?? 0) / ((PageSize ?? 1) == 0 ? 1 : (PageSize ?? 1)));
}

/// <summary>
/// Represents the outcome of a validation operation.
/// </summary>
/// <remarks>A result is valid when no validation errors are present.</remarks>
public class ValidationResult
{
    /// <summary>
    /// Gets a value indicating whether the object has no errors.
    /// </summary>
    public bool IsValid => Errors.Count == 0;

    /// <summary>
    /// Gets or sets the collection of errors.
    /// </summary>
    public List<ErrorDetails> Errors { get; set; } = new();
}