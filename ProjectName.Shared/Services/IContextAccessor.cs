using ProjectName.Shared.Contracts;

namespace ProjectName.Shared.Services;

/// <summary>
/// Provides access to the current consumer context.
/// </summary>
public interface IConsumerContextAccessor
{
    /// <summary>
    /// Gets or sets the current consumer context.
    /// </summary>
    ConsumerContext? Current { get; set; }
}

/// <summary>
/// Provides access to the current user context.
/// </summary>
public interface IUserContextAccessor
{
    /// <summary>
    /// Gets or sets the current user context.
    /// </summary>
    UserContext? Current { get; set; }
}