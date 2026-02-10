using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using StoryMode.Services;
using StoryMode.ViewModels;
using StoryMode.Views;

namespace StoryMode;

public partial class App : Application
{
    public IServiceProvider Services { get; private set; }
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        // Begin creating services.
        // Logging First.
        string logFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "StoryMode", "Logs");
        
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // silence core/ EF / system logs
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(
                path: logFolder + "/log-.txt",
                rollingInterval: RollingInterval.Day, // New file every day
                fileSizeLimitBytes: 5 * 1024 * 1024,  // Max 5MB per file
                rollOnFileSizeLimit: true,            // If today's log > 5MB, split it
                retainedFileCountLimit: 31,
                outputTemplate:"{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")           // Keep 1 month of history
            .CreateLogger();

        var collection = new ServiceCollection();
        collection.AddLogging(builder => builder.AddSerilog(Log.Logger, dispose:true));
        
        collection.AddSingleton<ProjectService>();
        collection.AddSingleton<SettingsService>();
        collection.AddSingleton<LanguageService>();
        collection.AddSingleton<DialogService>();
        
        Services = collection.BuildServiceProvider();
        
        LanguageService.Instance.Initialize(); // Scan TOML files in Assets/Lang
        SettingsService.Instance.Load();       // Load settings from file
        
        var savedLanguage = SettingsService.Instance.CurrentSettings.Language;
        LanguageService.Instance.LoadLanguage(savedLanguage);

        // Build service provider

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
                    ProjectService.Instance.LoadFromRecoveredFolder(recent.Path);
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