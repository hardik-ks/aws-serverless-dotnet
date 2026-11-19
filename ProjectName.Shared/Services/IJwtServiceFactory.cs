namespace ProjectName.Shared.Services;

/// <summary>
/// JWT service factory to get the appropriate JWT service based on user type
/// </summary>
public interface IJwtServiceFactory
{
    /// <summary>
    /// Gets the JWT service for the specified user type
    /// </summary>
    /// <param name="userType">The user type (Consumer, Vendor, Admin)</param>
    /// <returns>JWT service instance for the specified user type</returns>
    IJwtService GetJwtService(string userType);
}

/// <summary>
/// Consumer-specific JWT service
/// </summary>
public interface IConsumerJwtService : IJwtService { }

/// <summary>
/// Admin-specific JWT service
/// </summary>
public interface IAdminJwtService : IJwtService { }
