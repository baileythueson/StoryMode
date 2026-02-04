using Avalonia.Controls;

namespace StoryMode.ViewModels;

/// <summary>
/// Represents the primary view model for the main window of the application,
/// providing properties and commands for handling user interaction and navigation.
/// </summary>
public partial class MainWindowViewModel : ViewModelBase
{
    public bool IsCodexOpen { get; set; } = false;
    public string Greeting { get; } = "Welcome to Avalonia!";

    public UserControl CurrentView { get; set; }
    
    public void NavigateToEditorCommand()
    {
        IsCodexOpen = !IsCodexOpen;
    }
    
    public void NavigateToSettingsCommand()
    {
        
    }
}