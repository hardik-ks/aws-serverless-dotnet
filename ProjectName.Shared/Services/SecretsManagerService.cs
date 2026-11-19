using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ProjectName.Shared.Services;

/// <summary>
/// Service for retrieving secrets from AWS Secrets Manager with caching support
/// Optimized for Lambda functions with minimal performance impact
/// </summary>
public class SecretsManagerService : ISecretsManagerService, IDisposable
{
    private readonly IAmazonSecretsManager _secretsManager;
    private readonly IMemoryCache _cache;
    private readonly ILogger<SecretsManagerService> _logger;
    private readonly TimeSpan _cacheExpiration;
    private bool _disposed;

    /// <summary>
    /// Creates a new instance of SecretsManagerService
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="region">AWS region (defaults to ap-south-1)</param>
    /// <param name="cacheExpirationMinutes">Cache expiration in minutes (default: 5 minutes for Lambda containers)</param>
    public SecretsManagerService(ILogger<SecretsManagerService> logger, RegionEndpoint? region = null, int cacheExpirationMinutes = 5)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _secretsManager = new AmazonSecretsManagerClient(region ?? RegionEndpoint.AFSouth1);
        _cache = new MemoryCache(new MemoryCacheOptions());
        _cacheExpiration = TimeSpan.FromMinutes(cacheExpirationMinutes);
    }

    /// <summary>
    /// Creates a new instance with a custom Secrets Manager client
    /// </summary>
    /// <param name="secretsManager">Custom AWS Secrets Manager client</param>
    /// <param name="logger">Logger instance</param>
    /// <param name="cacheExpirationMinutes">Cache expiration in minutes</param>
    public SecretsManagerService(
        IAmazonSecretsManager secretsManager,
        ILogger<SecretsManagerService> logger,
        int cacheExpirationMinutes = 5)
    {
        _secretsManager = secretsManager ?? throw new ArgumentNullException(nameof(secretsManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = new MemoryCache(new MemoryCacheOptions());
        _cacheExpiration = TimeSpan.FromMinutes(cacheExpirationMinutes);
    }

    /// <summary>
    /// Retrieves a secret value from AWS Secrets Manager or the cache.
    /// </summary>
    /// <remarks>A successfully retrieved secret is cached for subsequent requests.</remarks>
    /// <param name="secretName">The name or Amazon Resource Name of the secret.</param>
    /// <param name="versionId">The version identifier of the secret to retrieve. When omitted, the default version is used.</param>
    /// <param name="cancellationToken">A token that cancels the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the secret value.</returns>
    /// <exception cref="ArgumentException">The secret name is empty or invalid, or AWS Secrets Manager rejects the request or parameter.</exception>
    /// <exception cref="InvalidOperationException">The secret does not contain a string value, cannot be decrypted, or AWS Secrets Manager encounters a service
    /// error.</exception>
    /// <exception cref="KeyNotFoundException">The specified secret cannot be found.</exception>
    public async Task<string> GetSecretAsync(string secretName, string? versionId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(secretName))
            throw new ArgumentException("Secret name cannot be null or empty", nameof(secretName));

        var cacheKey = GetCacheKey(secretName, versionId);

        // Try to get from cache first
        if (_cache.TryGetValue<string>(cacheKey, out var cachedValue))
        {
            _logger.LogDebug("Retrieved secret '{SecretName}' from cache", secretName);
            return cachedValue!;
        }

        _logger.LogInformation("Retrieving secret '{SecretName}' from AWS Secrets Manager", secretName);

        try
        {
            var request = new GetSecretValueRequest
            {
                SecretId = secretName,
                VersionId = versionId
            };

            var response = await _secretsManager.GetSecretValueAsync(request, cancellationToken);

            var secretValue = response.SecretString ?? throw new InvalidOperationException(
                $"Secret '{secretName}' does not contain a string value");

            // Cache the secret
            _cache.Set(cacheKey, secretValue, _cacheExpiration);

            _logger.LogInformation("Successfully retrieved and cached secret '{SecretName}'", secretName);

            return secretValue;
        }
        catch (ResourceNotFoundException ex)
        {
            _logger.LogError(ex, "Secret '{SecretName}' not found in AWS Secrets Manager", secretName);
            throw new KeyNotFoundException($"Secret '{secretName}' not found", ex);
        }
        catch (InvalidRequestException ex)
        {
            _logger.LogError(ex, "Invalid request for secret '{SecretName}'", secretName);
            throw new ArgumentException($"Invalid request for secret '{secretName}'", ex);
        }
        catch (InvalidParameterException ex)
        {
            _logger.LogError(ex, "Invalid parameter for secret '{SecretName}'", secretName);
            throw new ArgumentException($"Invalid parameter for secret '{secretName}'", ex);
        }
        catch (DecryptionFailureException ex)
        {
            _logger.LogError(ex, "Failed to decrypt secret '{SecretName}'", secretName);
            throw new InvalidOperationException($"Failed to decrypt secret '{secretName}'", ex);
        }
        catch (InternalServiceErrorException ex)
        {
            _logger.LogError(ex, "AWS Secrets Manager internal error for secret '{SecretName}'", secretName);
            throw new InvalidOperationException($"AWS Secrets Manager service error for secret '{secretName}'", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving secret '{SecretName}'", secretName);
            throw;
        }
    }

    /// <summary>
    /// Gets and deserializes a secret from AWS Secrets Manager.
    /// </summary>
    /// <typeparam name="T">The reference type to deserialize the secret value as.</typeparam>
    /// <param name="secretName">The name or ARN of the secret.</param>
    /// <param name="versionId">The identifier of the secret version to retrieve.</param>
    /// <param name="cancellationToken">A token that cancels the secret retrieval.</param>
    /// <returns>A deserialized secret of type <typeparamref name="T" />, or <see langword="null" /> if the secret value
    /// deserializes to null.</returns>
    /// <exception cref="InvalidOperationException">The secret value cannot be deserialized to type <typeparamref name="T" />.</exception>
    public async Task<T?> GetSecretAsync<T>(string secretName, string? versionId = null, CancellationToken cancellationToken = default) where T : class
    {
        var secretValue = await GetSecretAsync(secretName, versionId, cancellationToken);

        try
        {
            var deserializedSecret = JsonSerializer.Deserialize<T>(secretValue, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (deserializedSecret == null)
            {
                _logger.LogWarning("Secret '{SecretName}' deserialized to null", secretName);
            }

            return deserializedSecret;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize secret '{SecretName}' to type {Type}", secretName, typeof(T).Name);
            throw new InvalidOperationException($"Failed to deserialize secret '{secretName}' to {typeof(T).Name}", ex);
        }
    }

    /// <summary>
    /// Retrieves the values of the specified secrets from AWS Secrets Manager.
    /// </summary>
    /// <remarks>Retrieval is performed concurrently for all specified secrets.</remarks>
    /// <param name="secretNames">The names of the secrets to retrieve.</param>
    /// <param name="cancellationToken">A token that can cancel the asynchronous operation.</param>
    /// <returns>A dictionary that maps each secret name to its retrieved value.</returns>
    /// <exception cref="ArgumentException">Thrown when secretNames is null or empty.</exception>
    public async Task<Dictionary<string, string>> GetSecretsAsync(IEnumerable<string> secretNames, CancellationToken cancellationToken = default)
    {
        if (secretNames == null || !secretNames.Any())
            throw new ArgumentException("Secret names collection cannot be null or empty", nameof(secretNames));

        _logger.LogInformation("Retrieving {Count} secrets from AWS Secrets Manager", secretNames.Count());

        var tasks = secretNames.Select(async secretName =>
        {
            try
            {
                var value = await GetSecretAsync(secretName, null, cancellationToken);
                return new KeyValuePair<string, string>(secretName, value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve secret '{SecretName}'", secretName);
                throw;
            }
        });

        var results = await Task.WhenAll(tasks);

        return results.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    /// <summary>
    /// Clears the cached secret entries.
    /// </summary>
    /// <remarks>Calling this method without a secret name disposes the cache.</remarks>
    /// <param name="secretName">The name of the secret whose cached value is removed; when omitted or blank, all cached entries are cleared.</param>
    public void ClearCache(string? secretName = null)
    {
        if (string.IsNullOrWhiteSpace(secretName))
        {
            _logger.LogInformation("Clearing entire secrets cache");
            _cache.Dispose();
            // Note: In production, you might want to recreate the cache instead of disposing
        }
        else
        {
            var cacheKey = GetCacheKey(secretName, null);
            _cache.Remove(cacheKey);
            _logger.LogInformation("Cleared cache for secret '{SecretName}'", secretName);
        }
    }

    /// <summary>
    /// Gets the cache key for a secret.
    /// </summary>
    /// <param name="secretName">The name of the secret.</param>
    /// <param name="versionId">The optional secret version identifier.</param>
    /// <returns>The cache key for the secret name and, when provided, the secret version.</returns>
    private static string GetCacheKey(string secretName, string? versionId)
    {
        return string.IsNullOrWhiteSpace(versionId)
            ? $"secret:{secretName}"
            : $"secret:{secretName}:version:{versionId}";
    }

    /// <summary>
    /// Releases the cache and secrets manager resources.
    /// </summary>
    /// <remarks>This method may be called multiple times.</remarks>
    public void Dispose()
    {
        if (!_disposed)
        {
            _cache?.Dispose();
            _secretsManager?.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
