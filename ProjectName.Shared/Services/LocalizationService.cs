namespace ProjectName.Shared.Services;

/// <summary>
/// Provides localized application messages and manages the current culture.
/// </summary>
public class LocalizationService : ILocalizationService
{
    /// <summary>
    /// Gets the message associated with the specified key.
    /// </summary>
    /// <param name="key">The key identifying the message.</param>
    /// <returns>The message associated with the key, or the key if no message is found.</returns>
    public string GetMessage(string key)
    {
        //read from Resources/Messages.resx file using the key and return the localized message
        var message = ""; //Messages.ResourceManager.GetString(key, Messages.Culture);
        return message ?? key;
    }

    /// <summary>
    /// Retrieves a localized message and formats it with the supplied arguments.
    /// </summary>
    /// <param name="key">The key identifying the localized message.</param>
    /// <param name="args">The arguments used to format the localized message.</param>
    /// <returns>The formatted localized message.</returns>
    public string GetMessage(string key, params object[] args)
    {
        var message = GetMessage(key);
        return string.Format(message, args);
    }

    /// <summary>
    /// Sets the culture used for localization.
    /// </summary>
    /// <param name="culture">The culture name.</param>
    public void SetCulture(string culture)
    {
        //Messages.Culture = new CultureInfo(culture);
    }
}
