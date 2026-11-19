namespace ProjectName.Shared.Contracts;

/// <summary>
/// Standard Api Response which will be used to share with all microservices
/// </summary>
/// <typeparam name="T">The type of the response data.</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// The HTTP status code returned for the API response.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Gets or sets the response data.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Gets or sets the API error.
    /// </summary>
    public ApiError? Error { get; set; }
    
    /// <summary>
    /// Gets or sets the trace ID.
    /// </summary>
    public string TraceId { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    public long Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the API metadata.
    /// </summary>
    public ApiMetadata? Metadata { get; set; }

    /// <summary>
    /// Creates a successful API response containing the specified data.
    /// </summary>
    /// <param name="data">The data to include in the response.</param>
    /// <param name="traceId">The trace identifier for the response.</param>
    /// <param name="metadata">Optional metadata to include in the response.</param>
    /// <returns>An API response with a 200 status code, the specified data, trace identifier, and metadata, and the current UTC
    /// timestamp.</returns>
    public static ApiResponse<T> Ok(T data, string traceId, ApiMetadata? metadata = null)
    {
        return new ApiResponse<T>
        {
            StatusCode = 200,
            Data = data,
            TraceId = traceId,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Metadata = metadata
        };
    }

    /// <summary>
    /// Creates a not-found response.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="code">The error code.</param>
    /// <param name="traceId">The trace identifier.</param>
    /// <returns>An API response with a 404 status code and the specified error details.</returns>
    public static ApiResponse<T> NotFound(string message, string code, string traceId)
    {
        return new ApiResponse<T>
        {
            StatusCode = 404,
            Error = new ApiError
            {
                Code = code,
                Message = message,
                Type = ErrorType.NotFound 
            },
            TraceId = traceId,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
    }

    /// <summary>
    /// Creates an internal server error response.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="traceId">The trace identifier.</param>
    /// <returns>An API response with a 500 status code and the specified error details.</returns>
    public static ApiResponse<T> InternalError(string message, string traceId)
    {
        return new ApiResponse<T>
        {
            StatusCode = 500,
            Error = new ApiError
            {
                Code = "SERVER_ERROR",
                Message = message,
                Type =  ErrorType.Internal
            },
            TraceId = traceId,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
    }

    /// <summary>
    /// Creates an API response for a bad request.
    /// </summary>
    /// <param name="error">The error describing the bad request.</param>
    /// <param name="traceId">The trace identifier associated with the request.</param>
    /// <returns>An API response with status code 400.</returns>
    public static ApiResponse<T> BadRequest(ApiError error, string traceId)
    {
        return new ApiResponse<T>
        {
            StatusCode = 400,
            Error = error,
            TraceId = traceId,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
    }

    /// <summary>
    /// Creates an API response representing a bad request.
    /// </summary>
    /// <param name="message">The validation error message.</param>
    /// <param name="traceId">The request trace identifier.</param>
    /// <returns>A bad request API response with validation error details.</returns>
    public static ApiResponse<T> BadRequest(string message, string traceId)
    {
        return new ApiResponse<T>
        {
            StatusCode = 400,
            Error = new ApiError
            {
                Code = "SERVER_ERROR",
                Message = message,
                Type = ErrorType.Validation
            },
            TraceId = traceId,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
    }

    /// <summary>
    /// Creates an unauthorized API response.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="traceId">The request trace identifier.</param>
    /// <returns>An API response with a 401 status code and unauthorized error details.</returns>
    public static ApiResponse<T> Unauthorized(string message, string traceId)
    {
        return new ApiResponse<T>
        {
            StatusCode = 401,
            Error = new ApiError
            {
                Code = "UNAUTHORIZED",
                Message = message,
                Type = ErrorType.Unauthorized
            },
            TraceId = traceId,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
    }
}
