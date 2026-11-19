namespace ProjectName.Shared.Services
{
    /// <summary>
    /// Defines operations for generating and validating JSON Web Tokens.
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Generates a JWT token for the specified user.
        /// </summary>
        /// <param name="userId">The identifier of the user for whom the token is generated.</param>
        /// <param name="additionalClaims">Optional claims to include in the token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the generated token.</returns>
        Task<string> GenerateTokenAsync(string userId, Dictionary<string, object>? additionalClaims = null);
        
        /// <summary>
        /// Validates the specified JWT token.
        /// </summary>
        /// <param name="token">The token to validate.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <c>true</c> if the token is valid; otherwise, <c>false</c>.</returns>
        Task<bool> ValidateTokenAsync(string token);
    }
}