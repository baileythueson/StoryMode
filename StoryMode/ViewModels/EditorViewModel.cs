using StoryMode.Services;

namespace StoryMode.ViewModels;

/// <summary>
/// Represents the view model for an editor, providing functionality to manage and track
/// changes to content. This class inherits from <see cref="ViewModelBase"/> and integrates
/// with the application framework for maintaining state and notifying changes.
/// </summary>
public class EditorViewModel : ViewModelBase
{
    private string _content = string.Empty;
    public string Content
    {
        get => _content;
        set
        {
            if (SetProperty(ref _content, value))
            {
                ProjectManager.Instance.MarkDirty();
            }
        }
    }
}