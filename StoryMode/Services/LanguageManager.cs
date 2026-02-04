using System;
using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using StoryMode.Models;
using Tomlyn;
using Tomlyn.Model;

namespace StoryMode.Services;

/// <summary>
/// Provides functionality for managing application languages, including loading available languages
/// and retrieving localized string values from language files.
/// </summary>
public partial class LanguageManager : ObservableObject
{
    public static LanguageManager Instance { get; } = new();

    private TomlTable? _currentTable;

    // The indexer allows XAML to use: {Binding [Key.SubKey]}
    public string this[string key] => GetValue(key);

    [ObservableProperty] private ObservableCollection<LanguageOption> _availableLanguages = new();
    
    [ObservableProperty] private LanguageOption? _selectedLanguage;

    /// <summary>
    /// Initializes the language manager by scanning the designated directory for language files,
    /// parsing their content, and populating the list of available languages. This method scans
    /// for TOML files located in the "Assets/Lang" folder relative to the application's base directory.
    /// </summary>
    /// <remarks>
    /// Each valid language file is expected to contain a meta section with "Name" and "ISO" keys.
    /// If a file cannot be parsed or is missing the required metadata, it will be skipped and logged.
    /// </remarks>
    public void Initialize()
    {
        var langDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Lang");
        if (!Directory.Exists(langDir)) return;

        var files = Directory.GetFiles(langDir, "*.toml");
        AvailableLanguages.Clear();

        foreach (var file in files)
        {
            try 
            {
                var tomlText = File.ReadAllText(file);
                var model = Toml.ToModel(tomlText);
            
                // Navigate to [Meta] -> Name
                if (model.TryGetValue("Meta", out var metaObj) && metaObj is TomlTable metaTable)
                {
                    var displayName = metaTable["Name"]?.ToString() ?? Path.GetFileNameWithoutExtension(file);
                    var isoCode = metaTable["ISO"]?.ToString() ?? Path.GetFileNameWithoutExtension(file);
                
                    AvailableLanguages.Add(new LanguageOption(displayName, isoCode));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error parsing {file}: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Loads the specified language file based on the given ISO code and updates the current language table.
    /// This method reads a TOML file from the "Assets/Lang" folder, parses its content, and applies the
    /// corresponding language data. NotifyPropertyChanged is invoked to update data-bound UI components.
    /// </summary>
    /// <param name="isoCode">The ISO code of the language to be loaded. This code determines the specific file to read.</param>
    public void LoadLanguage(string isoCode)
    {
        // For development, you can use absolute paths or 
        // include the .toml as an 'AvaloniaResource'
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Lang", $"{isoCode}.toml");
        if (File.Exists(path))
        {
            var toml = File.ReadAllText(path);
            _currentTable = Toml.ToModel(toml);
            
            // Notify Avalonia that the indexer has updated
            OnPropertyChanged("Item[]"); 
        }
    }

    /// <summary>
    /// Retrieves the localized string value associated with the specified key from the currently loaded language table.
    /// </summary>
    /// <param name="key">The key for the localized string, with subkeys separated by periods (e.g., "Category.SubCategory.Key").</param>
    /// <returns>
    /// The localized string value corresponding to the specified key if found; otherwise, a string in the format "!key!" indicating the key is missing.
    /// </returns>
    private string GetValue(string key)
    {
        if (_currentTable == null) return $"!{key}!";

        string[] parts = key.Split('.');
        object? current = _currentTable;

        foreach (var part in parts)
        {
            if (current is TomlTable table && table.TryGetValue(part, out var next))
            {
                current = next;
            }
            else
            {
                return $"!{key}!"; // Key not found
            }
        }

        return current?.ToString() ?? $"!{key}!";
    }
}