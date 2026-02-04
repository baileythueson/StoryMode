using CommunityToolkit.Mvvm.ComponentModel;

namespace StoryMode.ViewModels;

/// <summary>
/// Serves as the base class for all view models in the application, providing
/// foundational functionality for property change notification through the
/// MVVM pattern. This ensures consistent state management and updates between
/// the UI and underlying data logic.
/// </summary>
/// <remarks>
/// This class extends <see cref="CommunityToolkit.Mvvm.ComponentModel.ObservableObject"/>,
/// leveraging its built-in support for implementing the INotifyPropertyChanged interface.
/// Derived classes can take advantage of these features to create responsive and data-driven
/// user interfaces.
/// </remarks>
public abstract class ViewModelBase : ObservableObject
{
}