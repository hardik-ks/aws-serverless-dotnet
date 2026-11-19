using ProjectName.Shared.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;


namespace ProjectName.Shared.Services;

/// <summary>
/// Provides JWT generation and validation services using RSA signing keys loaded from AWS Secrets Manager.
/// </summary>
public class JwtService : IJwtService
{
    private readonly ISecretsManagerService _secretsManager;
    private readonly JwtSettings _jwtSettings;

    /// <summary>
    /// Initializes a new instance of the JwtService class.
    /// </summary>
    /// <param name="secretsManager">The secrets manager service used to access JWT secrets.</param>
    /// <param name="jwtSettings">The settings used to configure JWT generation and validation.</param>
    /// <exception cref="ArgumentNullException">Thrown when secretsManager or jwtSettings is null.</exception>
    public JwtService(ISecretsManagerService secretsManager, JwtSettings jwtSettings)
    {
        _secretsManager = secretsManager ?? throw new ArgumentNullException(nameof(secretsManager));
        _jwtSettings = jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings));

    }

    /// <summary>
    /// Generates a signed JSON Web Token for the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="additionalClaims">Additional claims to include in the token, or null.</param>
    /// <returns>A signed JSON Web Token.</returns>
    public async Task<string> GenerateTokenAsync(string userId, Dictionary<string, object>? additionalClaims = null)
    {
        await GetJwtConfigurationAsync();

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        // Add additional claims
        if (additionalClaims != null)
        {
            foreach (var claim in additionalClaims)
            {
                claims.Add(new Claim(claim.Key, claim.Value?.ToString() ?? string.Empty));
            }
        }

        var signingKey = GetRsaSecurityKey(_jwtSettings.Secret.PrivateKey);
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.RsaSha256)
        {
            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            Claims = claims.ToDictionary(c => c.Type, c => (object)c.Value),
            SigningCredentials = credentials
        };

        var tokenHandler = new JsonWebTokenHandler();
        
        return tokenHandler.CreateToken(tokenDescriptor);
    }

   /// <summary>
   /// Validates a JSON Web Token and determines whether it is valid.
   /// </summary>
   /// <remarks>Validates the configured issuer, audience, signing key, and lifetime, with a five-minute clock
   /// skew.</remarks>
   /// <param name="token">The JSON Web Token to validate.</param>
   /// <returns><see langword="true"/> when the token is valid; otherwise, <see langword="false"/>.</returns>
    public async Task<bool> ValidateTokenAsync(string token)
    {
        await GetJwtConfigurationAsync();
        var validationKey = GetRsaSecurityKey(_jwtSettings.PublicKey);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = validationKey,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        };

        var tokenHandler = new JsonWebTokenHandler();
        var principal = await tokenHandler.ValidateTokenAsync(token, validationParameters);

        return principal.IsValid;
    }

    /// <summary>
    /// Gets JWT configuration from Secrets Manager with caching
    /// </summary>
    private async Task GetJwtConfigurationAsync()
    {
        _jwtSettings.Secret = await _secretsManager.GetSecretAsync<JwtSigningKey>(_jwtSettings.SecretKeyArn) ?? throw new InvalidOperationException("JWT secret not found");
    }

    /// <summary>
    /// Creates an RSA security key from a PEM-encoded private or public key.
    /// </summary>
    /// <remarks>The key must use PKCS #8 private-key or SubjectPublicKeyInfo public-key encoding.</remarks>
    /// <param name="pemKey">The PEM-encoded RSA private or public key.</param>
    /// <returns>A security key containing the RSA key and configured key identifier.</returns>
    private RsaSecurityKey GetRsaSecurityKey(string pemKey)
    {
        var rsa = RSA.Create();

        // Remove PEM headers/footers and decode
        var keyData = pemKey
            .Replace("-----BEGIN PRIVATE KEY-----", "")
            .Replace("-----END PRIVATE KEY-----", "")
            .Replace("-----BEGIN PUBLIC KEY-----", "")
            .Replace("-----END PUBLIC KEY-----", "")
            .Replace("\n", "")
            .Replace("\r", "");

        var keyBytes = Convert.FromBase64String(keyData);

        if (pemKey.Contains("PRIVATE"))
        {
            rsa.ImportPkcs8PrivateKey(keyBytes, out _);
        }
        else
        {
            rsa.ImportSubjectPublicKeyInfo(keyBytes, out _);
        }

        return new RsaSecurityKey(rsa)
        {
            KeyId = _jwtSettings.KeyId
        }; 
       
    }
}
