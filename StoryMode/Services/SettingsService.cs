using System;
using System.IO;
using System.Text.Json;
using StoryMode.Models;

namespace StoryMode.Services;

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
    
    public void Save()
    {
        File.WriteAllText(_configPath, JsonSerializer.Serialize(CurrentSettings));
    }
}