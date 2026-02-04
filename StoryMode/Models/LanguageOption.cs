namespace StoryMode.Models;

/// <summary>
/// Represents a language option available in the application.
/// Each language is defined by a display name and its ISO 639-1 code.
/// </summary>
/// <param name="DisplayName">
/// The user-facing name of the language, typically displayed in the application.
/// </param>
/// <param name="IsoCode">
/// The ISO 639-1 code of the language, used to uniquely identify it.
/// </param>
public record LanguageOption(string DisplayName, string IsoCode);