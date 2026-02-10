using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Tomlyn;

namespace StoryMode.Services;

/// <summary>
/// Provides functionalities for managing language localization in the application.
/// This service loads translation dictionaries from JSON files and ensures
/// proper fallback behavior for missing translation keys.
/// </summary>
public partial class LanguageService : ObservableObject
{
    private readonly ILogger<LanguageService> _logger;
    private readonly string _langDirectory;

    // The UI binds to this. When updated, 'PropertyChanged' fires automatically.
    [ObservableProperty]
    private Dictionary<string, string> _strings = new();

    // Fallback for missing keys so the UI doesn't crash or show blank space
    public string this[string key] => Strings.TryGetValue(key, out var value) ? value : $"#{key}#";

    public LanguageService(ILogger<LanguageService> logger)
    {
        _logger = logger;
        
        // Define where language files live (e.g., AppData/StoryMode/Assets/Lang)
        // Or strictly inside the app bundle if they are read-only resources.
        _langDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Lang");
        
        // Load default immediately (safe to do in constructor for read-only)
        LoadLanguage("en-US");
    }

    /// <summary>
    /// Loads language translations from a TOML file based on the specified ISO language code.
    /// Updates the internal dictionary with the loaded translations or falls back to
    /// an empty dictionary if the file is not found or fails to parse.
    /// </summary>
    /// <param name="isoCode">The ISO code of the language to load (e.g., "en-US").</param>
    public void LoadLanguage(string isoCode)
    {
        _logger.LogInformation("Switching language to {IsoCode}", isoCode);

        // Update extension to .toml
        var filePath = Path.Combine(_langDirectory, $"{isoCode}.toml");

        if (!File.Exists(filePath))
        {
            _logger.LogWarning("Language file not found: {Path}", filePath);
            Strings = new Dictionary<string, string>();
            return;
        }

        try
        {
            var text = File.ReadAllText(filePath);
            
            var toml = Toml.ToModel(text);
            
            var newStrings = new Dictionary<string, string>();

            foreach (var kvp in toml)
            {
                // Ensure we only grab string values, ignoring nested tables if any
                if (kvp.Value is string value)
                {
                    newStrings[kvp.Key] = value;
                }
            }
            
            Strings = newStrings;
            _logger.LogDebug("Loaded {Count} strings from TOML.", Strings.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse TOML language file: {Path}", filePath);
        }
    }

    /// <summary>
    /// Retrieves a list of available language ISO codes by scanning the language files
    /// in the configured language directory.
    /// </summary>
    /// <returns>A list of strings representing the ISO codes of available languages.
    /// If the language directory does not exist, a default list containing "en-US" is returned.</returns>
    // Optional: Get a list of available languages for the Settings dropdown
    public List<string> GetAvailableLanguages()
    {
        if (!Directory.Exists(_langDirectory)) return new List<string> { "en-US" };

        return Directory.GetFiles(_langDirectory, "*.json")
                        .Select(Path.GetFileNameWithoutExtension)
                        .Where(x => x != null)
                        .Cast<string>()
                        .ToList();
    }
}