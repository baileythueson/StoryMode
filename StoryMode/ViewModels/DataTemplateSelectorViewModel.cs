using System.Collections.Generic;

namespace StoryMode.ViewModels;

public class DataTemplateSelectorViewModel : ViewModelBase
{
    public string SelectedTemplate { get; set; }
    public List<string> DynamicFields { get; set; }
}