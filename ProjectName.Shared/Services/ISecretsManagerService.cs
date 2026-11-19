namespace ProjectName.Shared.Services;

/// <summary>
/// Interface for AWS Secrets Manager operations
/// </summary>
public interface ISecretsManagerService
{
    /// <summary>
    /// Retrieves a secret value from AWS Secrets Manager
    /// </summary>
    /// <param name="secretName">Name or ARN of the secret</param>
    /// <param name="versionId">Optional version ID (defaults to latest)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The secret value as a string</returns>
    Task<string> GetSecretAsync(string secretName, string? versionId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a secret value and deserializes it to the specified type
    /// </summary>
    /// <typeparam name="T">Type to deserialize the secret into</typeparam>
    /// <param name="secretName">Name or ARN of the secret</param>
    /// <param name="versionId">Optional version ID (defaults to latest)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deserialized secret object</returns>
    Task<T?> GetSecretAsync<T>(string secretName, string? versionId = null, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Retrieves multiple secrets in parallel
    /// </summary>
    /// <param name="secretNames">Collection of secret names</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary of secret names to secret values</returns>
    Task<Dictionary<string, string>> GetSecretsAsync(IEnumerable<string> secretNames, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears the cache for a specific secret or all secrets
    /// </summary>
    /// <param name="secretName">Optional secret name (if null, clears entire cache)</param>
    void ClearCache(string? secretName = null);
}
