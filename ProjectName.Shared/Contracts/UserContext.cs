namespace ProjectName.Shared.Contracts;

/// <summary>
/// Represents the context for the current user.
/// </summary>
public class UserContext
{
    /// <summary>
    /// Gets or sets the identifier of the user.
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the phone number of the user.
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the type of the user.
    /// </summary>
    public string UserType { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the dictionary of all claims for the user.
    /// </summary>
    public Dictionary<string, string> AllClaims { get; set; } = new();
}

/// <summary>
/// Provides contextual identity and claim information for a consumer.
/// </summary>
public class ConsumerContext
{
    /// <summary>
    /// Gets or sets the identifier of the consumer.
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the identifier of the consumer.
    /// </summary>
    public string ConsumerId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the name of the consumer.
    /// </summary>
    public string ConsumerName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the phone number of the consumer.
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the type of the consumer.
    /// </summary>
    public string UserType { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the dictionary of all claims for the consumer.
    /// </summary>
    public Dictionary<string, string> AllClaims { get; set; } = new();
}
