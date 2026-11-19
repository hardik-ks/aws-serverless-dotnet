using ProjectName.Shared.Services;

namespace ProjectName.Shared.Middleware;

/// <summary>
/// Provides language-selection and culture configuration helpers for localization.
/// </summary>
public static class LocalizationHelper
{
    /// <summary>
    /// Gets the preferred supported language from request headers.
    /// </summary>
    /// <remarks>Checks the <c>Accept-Language</c> header first, then the <c>X-Language</c> header.</remarks>
    /// <param name="headers">The request headers containing the language preference.</param>
    /// <returns>The supported language code, or <c>en</c> when no supported language is found.</returns>
    public static string GetLanguageFromHeaders(IDictionary<string, string>? headers)
    {
        if (headers == null)
            return "en";

        // Check for Accept-Language header (common standard)
        if (headers.TryGetValue("accept-language", out var acceptLanguage))
        {
            // Parse the first language preference
            var language = acceptLanguage.Split(',').FirstOrDefault()?.Split('-').FirstOrDefault()?.Trim();
            return IsSupported(language) ? language! : "en";
        }

        // Check for custom X-Language header
        if (headers.TryGetValue("x-language", out var customLanguage))
        {
            return IsSupported(customLanguage) ? customLanguage : "en";
        }

        return "en";
    }

    /// <summary>
    /// Sets the localization culture based on the specified language.
    /// </summary>
    /// <param name="localizationService">The localization service whose culture is set.</param>
    /// <param name="language">The language code used to select the culture. French selects fr-FR; other values select en-US.</param>
    public static void SetLanguage(ILocalizationService localizationService, string language)
    {
        var culture = language.ToLower() switch
        {
            "fr" => "fr-FR",
            "en" => "en-US",
            _ => "en-US"
        };

        localizationService.SetCulture(culture);
    }

    /// <summary>
    /// Determines whether a language is supported.
    /// </summary>
    /// <param name="language">The language identifier to check.</param>
    /// <returns>true if language is "en" or "fr", ignoring case; otherwise, false.</returns>
    private static bool IsSupported(string? language)
    {
        if (string.IsNullOrWhiteSpace(language))
            return false;

        var supportedLanguages = new[] { "en", "fr" };
        return supportedLanguages.Contains(language.ToLower());
    }
}
