using Avalonia.Controls;

namespace StoryMode.ViewModels;

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