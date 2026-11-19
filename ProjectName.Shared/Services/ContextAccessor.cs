using ProjectName.Shared.Contracts;

namespace ProjectName.Shared.Services;

/// <summary>
/// Provides access to the current user context.
/// </summary>
public class UserContextAccessor : IUserContextAccessor
{
    /// <summary>
    /// Gets or sets the current admin user context.
    /// </summary>
    public UserContext? Current { get; set; }
}

/// <summary>
/// Provides access to the current consumer context.
/// </summary>
public class ConsumerContextAccessor : IConsumerContextAccessor
{
    /// <summary>
    /// Gets or sets the current consumer user context.
    /// </summary>
    public ConsumerContext? Current { get; set; }
}

