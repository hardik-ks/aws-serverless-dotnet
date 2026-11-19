using Amazon.SimpleEmail;
using ProjectName.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectName.Shared.Configuration;
using ProjectName.Shared.Infrastructure;
using ProjectName.Shared.Repository;

namespace ProjectName.Shared;

public static class SharedServiceCollectionExtensions
{
    public static IServiceCollection AddSharedInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configuration - Map from IConfiguration to strongly-typed settings
        var settings = new ProjectSettings
        {
            Database = new DatabaseSettings
            {
                Host = configuration["db-proxy-endpoint"] ?? string.Empty,
                Port = configuration["db-port"] ?? string.Empty,
                Database = configuration["db-name"] ?? string.Empty,
                Username = configuration["db-username"] ?? string.Empty,
                Password = configuration["db-password"] ?? string.Empty
            },
            JwtSettings = new JwtSettingsCollection
            {
                Consumer = new JwtSettings
                {
                    SecretKeyArn = configuration["jwt:consumer:secret-arn"] ?? string.Empty,
                    Issuer = configuration["jwt:consumer:issuer"] ?? string.Empty,
                    Audience = configuration["jwt:consumer:audience"] ?? string.Empty,
                    KeyId = configuration["jwt:consumer:key-id"] ?? "consumer-key",
                    Algorithm = configuration["jwt:consumer:algorithm"] ?? "RS256",
                    PublicKey = configuration["jwt:consumer:public-key"] ?? string.Empty,
                    ExpirationMinutes = int.TryParse(configuration["jwt:consumer:expiration-minutes"], out var consumerExp) ? consumerExp : 60
                },
                Admin = new JwtSettings
                {
                    SecretKeyArn = configuration["jwt:admin:secret-arn"] ?? string.Empty,
                    Issuer = configuration["jwt:admin:issuer"] ?? string.Empty,
                    Audience = configuration["jwt:admin:audience"] ?? string.Empty,
                    KeyId = configuration["jwt:admin:key-id"] ?? "admin-key",
                    Algorithm = configuration["jwt:admin:algorithm"] ?? "RS256",
                    PublicKey = configuration["jwt:admin:public-key"] ?? string.Empty,
                    ExpirationMinutes = int.TryParse(configuration["jwt:admin:expiration-minutes"], out var adminExp) ? adminExp : 30
                }
            },
            Email = new EmailSettings
            {
                SmtpHost = configuration["email:smtp-host"] ?? string.Empty,
                SmtpPort = int.TryParse(configuration["email:smtp-port"], out var port) ? port : 587,
                EnableSsl = bool.TryParse(configuration["email:enable-ssl"], out var ssl) ? ssl : true,
                Username = configuration["email:username"] ?? string.Empty,
                Password = configuration["email:password"] ?? string.Empty,
                FromAddress = configuration["email:from-address"] ?? string.Empty,
                FromName = configuration["email:from-name"] ?? "No-Reply",
                ContactEmail = configuration["email:contact-email"] ?? string.Empty,
                DefaultSubject = configuration["email:default-subject"]
            },
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "dev",
        };
        services.AddSingleton(settings);

        // Register individual settings for backward compatibility and easy injection
        services.AddSingleton(settings.JwtSettings);
        services.AddSingleton(settings.Database);
        services.AddSingleton(settings.Email);
        
        //logging
        var loggerFactory = LoggingConfigurator.Configure();
        services.AddSingleton(loggerFactory);
        services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

        // AWS Secrets Manager Service
        services.AddSingleton<ISecretsManagerService, SecretsManagerService>();

        // JWT Services - Register factory and user-type-specific services
        services.AddSingleton<IJwtServiceFactory, JwtServiceFactory>();
        services.AddSingleton<IConsumerJwtService, ConsumerJwtService>();
        services.AddSingleton<IAdminJwtService, AdminJwtService>();

        // Register generic IJwtService for backward compatibility (defaults to Consumer)
        services.AddSingleton<IJwtService>(sp => sp.GetRequiredService<IConsumerJwtService>());

        // HttpClient for SMS service - using direct instantiation to avoid IHttpClientFactory compatibility issues
        services.AddSingleton(sp => new HttpClient());

        // Communication Services
        // Only register AWS SES in production (skip in test environment to avoid AWS credentials requirement)
        if (Environment.GetEnvironmentVariable("SKIP_AWS_CONFIG") != "true")
        {
            services.AddAWSService<IAmazonSimpleEmailService>();
            services.AddSingleton<IEmailService, SesEmailService>();
        }
        // Note: TestEmailService will be registered by ServiceConfigurator when USE_TEST_SECRETS_MANAGER=true


        // Localization Service
        services.AddSingleton<ILocalizationService, LocalizationService>();

        // Communication Templates Service
        services.AddMemoryCache();

        // Database
        services.AddSingleton<IConnectionFactory>(
            new ConnectionFactory(settings.Database));

        // Optional: retry helper
        services.AddScoped<IUserContextAccessor, UserContextAccessor>();
        services.AddScoped<IConsumerContextAccessor, ConsumerContextAccessor>();

        return services;
    }
}

public static class ConfigurationProvider
{
    public static IConfiguration BuildConfiguration()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "dev";
        var skipAwsConfig = Environment.GetEnvironmentVariable("SKIP_AWS_CONFIG");

        var builder = new ConfigurationBuilder()
            .AddEnvironmentVariables();

        // Only add Systems Manager if not in test environment
        if (string.IsNullOrEmpty(skipAwsConfig) || skipAwsConfig != "true")
        {
            builder.AddSystemsManager(options =>
            {
                options.Path = $"/projectName/{environment}/";
                options.Optional = false;
                options.AwsOptions = new Amazon.Extensions.NETCore.Setup.AWSOptions
                {
                    Region = Amazon.RegionEndpoint.AFSouth1
                };
            });
        }

        return builder.Build();
    }
}