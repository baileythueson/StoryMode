using System.Globalization;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using HarfBuzzSharp;
using StoryMode.Services;
using StoryMode.ViewModels;
using StoryMode.Views;
using MessageBox = StoryMode.Views.MessageBox;

namespace StoryMode;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        LanguageManager.Instance.Initialize(); // Scan TOML files in Assets/Lang
        SettingsService.Instance.Load();       // Load settings from file
        
        var savedLanguage = SettingsService.Instance.CurrentSettings.Language;
        LanguageManager.Instance.LoadLanguage(savedLanguage);
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            var window = new MainWindow()
            {
                DataContext = new MainWindowViewModel(),
            };
            desktop.MainWindow = window;

            var recoveryService = new RecoveryService();
            var ghosts = recoveryService.ScanForCrashes();
            
            if (ghosts.Count > 0)
            {
                var recent = ghosts.OrderByDescending(x => x.LastWriteTime).First();
                
                if (await DialogService.ConfirmAsync(
                        $"Found a crash from {recent.LastWriteTime:yyyy-MM-dd HH:mm:ss}. Do you want to recover?",
                        "Crash Recovery",
                        window))
                {
                    ProjectManager.Instance.LoadFromRecoveredFolder(recent.Path);
                }
                else
                {
                    recoveryService.CleanUp(recent.Path, true);
                }
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}