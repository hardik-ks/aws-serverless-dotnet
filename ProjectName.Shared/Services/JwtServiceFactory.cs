using ProjectName.Shared.Configuration;

namespace ProjectName.Shared.Services;

/// <summary>
/// Factory for creating user-type-specific JWT services
/// </summary>
public class JwtServiceFactory : IJwtServiceFactory
{
    private readonly Dictionary<string, IJwtService> _jwtServices;

    /// <summary>
    /// Initializes a factory that creates JWT services for consumer and admin users.
    /// </summary>
    /// <param name="secretsManager">The service used to retrieve secrets required by JWT services.</param>
    /// <param name="jwtSettingsCollection">The JWT settings for consumer and admin users.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="secretsManager"/> or <paramref name="jwtSettingsCollection"/> is <see
    /// langword="null"/>.</exception>
    public JwtServiceFactory(ISecretsManagerService secretsManager, JwtSettingsCollection jwtSettingsCollection)
    {
        var _secretsManager = secretsManager ?? throw new ArgumentNullException(nameof(secretsManager));
        var _jwtSettingsCollection = jwtSettingsCollection ?? throw new ArgumentNullException(nameof(jwtSettingsCollection));

        // Initialize JWT services for each user type
        _jwtServices = new Dictionary<string, IJwtService>(StringComparer.OrdinalIgnoreCase)
        {
            ["consumer"] = new JwtService(_secretsManager, _jwtSettingsCollection.Consumer),
            ["admin"] = new JwtService(_secretsManager, _jwtSettingsCollection.Admin)
        };
    }

    /// <summary>
    /// Gets the JWT service for the specified user type
    /// </summary>
    public IJwtService GetJwtService(string userType)
    {
        if (string.IsNullOrWhiteSpace(userType))
        {
            throw new ArgumentException("User type cannot be null or empty", nameof(userType));
        }

        if (_jwtServices.TryGetValue(userType, out var service))
        {
            return service;
        }

        throw new ArgumentException($"Invalid user type: {userType}. Valid types are: Consumer, Vendor, Admin", nameof(userType));
    }
}

/// <summary>
/// Consumer-specific JWT service implementation
/// </summary>
public class ConsumerJwtService : JwtService, IConsumerJwtService
{
    /// <summary>
    /// Initializes a consumer JWT service with the configured secrets manager and JWT settings.
    /// </summary>
    /// <param name="secretsManager">The service used to retrieve secrets required for JWT operations.</param>
    /// <param name="jwtSettings">The collection of JWT settings for the consumer service.</param>
    public ConsumerJwtService(ISecretsManagerService secretsManager, JwtSettingsCollection jwtSettings)
        : base(secretsManager, jwtSettings.Consumer)
    {
    }
}

/// <summary>
/// Admin-specific JWT service implementation
/// </summary>
public class AdminJwtService : JwtService, IAdminJwtService
{
    /// <summary>
    /// Initializes a new instance of the admin JWT service.
    /// </summary>
    /// <param name="secretsManager">The service used to retrieve secrets.</param>
    /// <param name="jwtSettings">The collection containing the admin JWT settings.</param>
    public AdminJwtService(ISecretsManagerService secretsManager, JwtSettingsCollection jwtSettings)
        : base(secretsManager, jwtSettings.Admin)
    {
    }
}
