using Microsoft.Extensions.Logging;

namespace ProjectName.Shared.Infrastructure;

/// <summary>
/// Configures logging for AWS Lambda applications.
/// </summary>
/// <remarks>Clears existing logger providers and enables Lambda logging with log levels, categories, and trailing
/// newlines.</remarks>
public static class LoggingConfigurator
{
    /// <summary>
    /// Creates an <see cref="ILoggerFactory"/> configured for AWS Lambda logging.
    /// </summary>
    /// <remarks>Existing logging providers are removed and the Lambda logger provider is configured to
    /// include log levels, categories, and newlines.</remarks>
    /// <returns>The configured logger factory.</returns>
    public static ILoggerFactory Configure()
    {
        return LoggerFactory.Create(builder =>
        {
            builder.ClearProviders();

            builder.AddLambdaLogger(new LambdaLoggerOptions
            {
                IncludeLogLevel = true,
                IncludeCategory = true,
                IncludeNewline = true
            });
        });
    }
}

/// <summary>
/// Provides the correlation identifier for the current asynchronous execution context.
/// </summary>
/// <remarks>The identifier is propagated across asynchronous operations and is stored using an AsyncLocal
/// value.</remarks>
public static class CorrelationContext
{
    /// <summary>
    /// Provides correlation identifier values isolated to the current asynchronous execution context.
    /// </summary>
    private static readonly AsyncLocal<string> _correlationId = new();

    /// <summary>
    /// Gets or sets the correlation ID.
    /// </summary>
    /// <remarks>A new GUID is generated when no correlation ID is set.</remarks>
    public static string CorrelationId
    {
        get => _correlationId.Value ?? Guid.NewGuid().ToString();
        set => _correlationId.Value = value;
    }
}

