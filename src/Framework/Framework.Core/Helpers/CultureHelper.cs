using System.Globalization;

namespace Framework.Core.Helpers;

/// <summary>
/// Provides helper methods for culture and localization operations.
/// Used throughout the application for determining the current language/culture context.
/// </summary>
/// <remarks>
/// This helper supports the Saudi Vision 2030 requirement for bilingual (Arabic/English) support.
///
/// Usage Example:
/// <code>
/// string displayName = CultureHelper.IsArabic ? entity.NameAr : entity.NameEn;
/// </code>
/// </remarks>
public static class CultureHelper
{
    /// <summary>
    /// Gets whether the current thread culture is Arabic.
    /// Checks if the current culture's TwoLetterISOLanguageName is "ar".
    /// </summary>
    /// <value>
    /// true if the current culture is Arabic; otherwise, false.
    /// </value>
    /// <remarks>
    /// This property uses Thread.CurrentThread.CurrentCulture to determine the language.
    /// The culture is typically set by:
    /// - Request localization middleware in ASP.NET Core
    /// - Accept-Language HTTP header from client requests
    /// - Application default culture configuration
    ///
    /// Supported Arabic cultures include:
    /// - ar (Arabic - generic)
    /// - ar-SA (Arabic - Saudi Arabia)
    /// - ar-EG (Arabic - Egypt)
    /// - ar-AE (Arabic - United Arab Emirates)
    /// And all other Arabic regional variants.
    /// </remarks>
    public static bool IsArabic =>
        CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals("ar", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Gets whether the current UI culture is Arabic.
    /// Checks if the current UI culture's TwoLetterISOLanguageName is "ar".
    /// </summary>
    /// <value>
    /// true if the current UI culture is Arabic; otherwise, false.
    /// </value>
    /// <remarks>
    /// CurrentUICulture is used for resource string lookups.
    /// In most scenarios, CurrentCulture and CurrentUICulture are set to the same value.
    /// </remarks>
    public static bool IsArabicUI =>
        CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.Equals("ar", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Gets the current culture's two-letter ISO language code.
    /// </summary>
    /// <returns>Two-letter ISO language code (e.g., "ar", "en")</returns>
    public static string GetCurrentLanguageCode() =>
        CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

    /// <summary>
    /// Gets the current culture name (e.g., "ar-SA", "en-US").
    /// </summary>
    /// <returns>Full culture name</returns>
    public static string GetCurrentCultureName() =>
        CultureInfo.CurrentCulture.Name;
}
