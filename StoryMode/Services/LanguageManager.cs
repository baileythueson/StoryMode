using System;
using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using StoryMode.Models;
using Tomlyn;
using Tomlyn.Model;

namespace StoryMode.Services;

public partial class LanguageManager : ObservableObject
{
    public static LanguageManager Instance { get; } = new();

    private TomlTable? _currentTable;

    // The indexer allows XAML to use: {Binding [Key.SubKey]}
    public string this[string key] => GetValue(key);

    [ObservableProperty] private ObservableCollection<LanguageOption> _availableLanguages = new();
    
    [ObservableProperty] private LanguageOption? _selectedLanguage;

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