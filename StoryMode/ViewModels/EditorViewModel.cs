using StoryMode.Services;

namespace StoryMode.ViewModels;

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