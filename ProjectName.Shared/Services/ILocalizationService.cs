namespace ProjectName.Shared.Services;

/// <summary>
/// Provides access to localized messages and culture selection.
/// </summary>
public interface ILocalizationService
{
    /// <summary>
    /// Gets a localized message for the specified key.
    /// </summary>
    /// <param name="key">The key of the message to retrieve.</param>
    /// <returns>The localized message.</returns>
    string GetMessage(string key);

    /// <summary>
    /// Gets a localized message for the specified key, with placeholders replaced by the provided arguments.
    /// </summary>
    /// <param name="key">The key of the message to retrieve.</param>
    /// <param name="args">The arguments to replace placeholders with.</param>
    /// <returns>The localized message.</returns>
    string GetMessage(string key, params object[] args);

    /// <summary>
    /// Sets the culture used for localization.
    /// </summary>
    /// <param name="culture">The culture name to set.</param>
    void SetCulture(string culture);
}
