namespace StoryMode.Models;

/// <summary>
/// Represents a user's configurable settings within the application.
/// </summary>
/// <remarks>
/// This class serves as a data model to store user preferences, such as language, theme, and font size.
/// Instances of this class are typically managed by a service, such as SettingsService.
/// </remarks>
public class UserSettings
{
    public string Language { get; set; } = "en-US";
    public string Theme { get; set; } = "Light";
    public string FontSize { get; set; } = "14";
}