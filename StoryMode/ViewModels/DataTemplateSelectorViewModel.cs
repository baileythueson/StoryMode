using System.Collections.Generic;

namespace StoryMode.ViewModels;

/// <summary>
/// Represents a view model for selecting data templates dynamically based on
/// the selected template and a collection of dynamic fields.
/// </summary>
/// <remarks>
/// This view model is used in conjunction with a data template selector to
/// provide dynamic UI elements based on the data provided. It inherits
/// functionality from <see cref="ViewModelBase"/>, which includes support
/// for property change notifications.
/// </remarks>
public class DataTemplateSelectorViewModel : ViewModelBase
{
    public string SelectedTemplate { get; set; }
    public List<string> DynamicFields { get; set; }
}