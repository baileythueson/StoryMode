using System;
using System.IO;
using System.Text.Json;
using StoryMode.Models;

namespace StoryMode.Services;

/// <summary>
/// Provides functionality to manage and persist user settings for the application.
/// </summary>
/// <remarks>
/// This service handles the loading and saving of user preferences, such as language and theme,
/// to a local JSON configuration file. It ensures that user settings are persisted across application sessions
/// and provides a singleton instance for global access.
/// </remarks>
public class SettingsService
{
    public static SettingsService Instance { get; } = new();

    private readonly string _configPath;
    
    public UserSettings CurrentSettings { get; set; } = new();

    private SettingsService()
    {
        var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "StoryMode");
        
        Directory.CreateDirectory(folder);
        _configPath = Path.Combine(folder, "config.json");

        CurrentSettings = new UserSettings();
    }

    /// <summary>
    /// Loads user settings from the configuration file into the application.
    /// </summary>
    /// <remarks>
    /// This method attempts to deserialize user settings stored in a JSON file specified by the configuration path.
    /// If the file exists and contains valid configuration data, the settings are loaded into the CurrentSettings property.
    /// If the file does not exist or an error occurs during deserialization, the method logs the exception or defaults to pre-set properties.
    /// </remarks>
    public void Load()
    {
        if (File.Exists(_configPath))
        {
            try
            {
                var settings = JsonSerializer.Deserialize<UserSettings>(File.ReadAllText(_configPath));
                if (settings is null) return;
                
                CurrentSettings = settings;
            }
            catch (Exception e)
            {
                // TODO: replace with proper logging.
                Console.WriteLine(e);
            }
        }
    }

    /// <summary>
    /// Persists the current user settings to the configuration file.
    /// </summary>
    /// <remarks>
    /// This method serializes the CurrentSettings object into a JSON format and writes it to the configuration file
    /// specified by the service. It ensures that any changes made to user settings during runtime are saved and
    /// available for the next application session. If the file write operation fails, an exception may be thrown.
    /// </remarks>
    public void Save()
    {
        // atomic save
        File.Copy(_configPath, $"{_configPath}.bak", true);
        File.WriteAllText(_configPath, JsonSerializer.Serialize(CurrentSettings));
        File.Delete($"{_configPath}.bak");
    }
}