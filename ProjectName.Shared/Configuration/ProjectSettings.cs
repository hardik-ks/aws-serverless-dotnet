namespace ProjectName.Shared.Configuration;

/// <summary>
/// Strongly-typed configuration settings for {projectName} application.
/// Maps to AWS Parameter Store values at /{projectName}/{environment}/
/// </summary>
public class ProjectSettings
{
    /// <summary>
    /// Database configuration settings
    /// Maps to /{projectName}/{environment}/db/*
    /// </summary>
    public DatabaseSettings Database { get; set; } = new();

    /// <summary>
    /// JWT configuration settings for different user types
    /// Maps to /{projectName}/{environment}/jwt/{userType}/*
    /// </summary>
    public JwtSettingsCollection JwtSettings { get; set; } = new();

    /// <summary>
    /// Email configuration settings
    /// Maps to /{projectName}/{environment}/email/*
    /// </summary>
    public EmailSettings Email { get; set; } = new();

    /// <summary>
    /// Gets or sets the application environment name.
    /// </summary>
    public string Environment { get; set; } = string.Empty;
}

/// <summary>
/// Collection of JWT settings for different user types 
/// </summary>
public class JwtSettingsCollection
{
    /// <summary>
    /// JWT settings for Consumer users
    /// Maps to /{projectName}/{environment}/jwt/consumer/*
    /// </summary>
    public JwtSettings Consumer { get; set; } = new();

    /// <summary>
    /// JWT settings for Admin users
    /// Maps to /{projectName}/{environment}/jwt/admin/*
    /// </summary>
    public JwtSettings Admin { get; set; } = new();

    /// <summary>
    /// Gets JWT settings by user type
    /// </summary>
    /// <param name="userType">The user type (Consumer, Admin)</param>
    /// <returns>JWT settings for the specified user type</returns>
    public JwtSettings GetByUserType(string userType)
    {
        return userType?.ToLowerInvariant() switch
        {
            "consumer" => Consumer,
            "admin" => Admin,
            _ => throw new ArgumentException($"Invalid user type: {userType}", nameof(userType))
        };
    }
}

/// <summary>
/// Provides configuration values for connecting to a MySQL database.
/// </summary>
/// <remarks>The settings map to Parameter Store values under /{projectName}/{environment}/db/.</remarks>
public class DatabaseSettings
{
    /// <summary>
    /// Database host/server address
    /// Parameter: /{projectName}/{environment}/db/host
    /// </summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// Database port number
    /// Parameter: /{projectName}/{environment}/db/port
    /// </summary>
    public string Port { get; set; } = "3306";

    /// <summary>
    /// Database name
    /// Parameter: /{projectName}/{environment}/db/database
    /// </summary>
    public string Database { get; set; } = string.Empty;

    /// <summary>
    /// Database username
    /// Parameter: /{projectName}/{environment}/db/username
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Database password (stored as SecureString in Parameter Store)
    /// Parameter: /{projectName}/{environment}/db/password
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets the MySQL connection string built from individual settings
    /// </summary>
    public string GetConnectionString()
    {
        if (string.IsNullOrEmpty(Port))
            return $"Server={Host};Database={Database};Uid={Username};Pwd={Password};";
        else
            return $"Server={Host};Port={Port};Database={Database};Uid={Username};Pwd={Password};";

    }
}

/// <summary>
/// Represents the configuration settings used to sign and validate JWT tokens.
/// </summary>
/// <remarks>The settings include signing credentials, token validity, issuer and audience values, key
/// identification, signing algorithm, and public-key validation data.</remarks>
public class JwtSettings
{
    /// <summary>
    /// JWT Signing Key for token signing
    /// Secrete Manger key
    /// </summary>
    public JwtSigningKey Secret { get; set; } = new JwtSigningKey();

    /// <summary>
    /// JWT secret key for token signing
    /// Parameter: /{projectName}/{environment}/jwt/secret-arn
    /// </summary>
    public string SecretKeyArn { get; set; } = string.Empty;

    /// <summary>
    /// JWT token expiration in minutes
    /// Parameter: /{projectName}/{environment}/jwt/expiration-minutes
    /// </summary>
    public int ExpirationMinutes { get; set; } = 60;

    /// <summary>
    /// JWT issuer
    /// Parameter: /{projectName}/{environment}/jwt/issuer
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// JWT audience
    /// Parameter: /{projectName}/{environment}/jwt/audience
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// JWT key ID
    /// Parameter: /{projectName}/{environment}/jwt/key-id
    /// </summary>
    public string KeyId { get; set; } = string.Empty;

    /// <summary>
    /// JWT token algorithm (e.g. RS256)
    /// Parameter: /{projectName}/{environment}/jwt/algorithm
    /// </summary>
    public string Algorithm { get; set; } = string.Empty;

    /// <summary>
    /// JWT token Public Key for token validation (PEM format)
    /// Parameter: /{projectName}/{environment}/jwt/public-key
    /// </summary>
    public string PublicKey { get; set; } = string.Empty;
}

/// <summary>
/// Represents the JWT signing key configuration and associated issuer metadata.
/// </summary>
public class JwtSigningKey
{
    /// <summary>
    /// Gets or sets the private key.
    /// </summary>
    /// <remarks>Store and transmit the private key securely.</remarks>
    public string PrivateKey { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the public key.
    /// </summary>
    public string PublicKey { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the issuer.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the audience.
    /// </summary>
    public string Audience { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the algorithm.
    /// </summary>
    public string Algorithm { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the key ID.
    /// </summary>
    public string KeyId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the generated at timestamp.
    /// </summary>
    public string GeneratedAt { get; set; } = string.Empty;
}

/// <summary>
/// Defines SMTP and sender settings used for email delivery.
/// </summary>
public class EmailSettings
{
    /// <summary>
    /// SMTP server host address
    /// Parameter: /{projectName}/{environment}/email/smtp-host
    /// </summary>
    public string SmtpHost { get; set; } = string.Empty;

    /// <summary>
    /// SMTP server port (typically 587 for TLS, 465 for SSL, 25 for non-SSL)
    /// Parameter: /{projectName}/{environment}/email/smtp-port
    /// </summary>
    public int SmtpPort { get; set; } = 587;

    /// <summary>
    /// Enable SSL/TLS for SMTP connection
    /// Parameter: /{projectName}/{environment}/email/enable-ssl
    /// </summary>
    public bool EnableSsl { get; set; } = true;

    /// <summary>
    /// SMTP username for authentication
    /// Parameter: /{projectName}/{environment}/email/username
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// SMTP password for authentication
    /// Parameter: /{projectName}/{environment}/email/password
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// From email address
    /// Parameter: /{projectName}/{environment}/email/from-address
    /// </summary>
    public string FromAddress { get; set; } = string.Empty;

    /// <summary>
    /// Contact email address
    /// Parameter: /{projectName}/{environment}/email/contact-email
    /// </summary>
    public string ContactEmail { get; set; } = string.Empty;

    /// <summary>
    /// From display name
    /// Parameter: /{projectName}/{environment}/email/from-name
    /// </summary>
    public string FromName { get; set; } = "{projectName}s";

    /// <summary>
    /// Default email subject if not specified
    /// Parameter: /{projectName}/{environment}/email/default-subject
    /// </summary>
    public string? DefaultSubject { get; set; }
}

