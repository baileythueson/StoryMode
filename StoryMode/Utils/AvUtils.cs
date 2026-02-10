using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace StoryMode.Utils;

/// <summary>
/// Provides utility methods and functionalities for working with Avalonia framework components and elements.
/// This class is designed to streamline common operations and interactions within the Avalonia UI framework.
/// </summary>
public static class AvaloniaUtils
{
    /// <summary>
    /// Retrieves the currently active window in the application.
    /// If no window is active, it returns the main window of the application if available.
    /// </summary>
    /// <returns>
    /// The currently active <see cref="Window"/> instance, or the main window if no active window is found.
    /// Returns null if no windows are available or the application lifetime is not desktop-based.
    /// </returns>
    public static Window? GetActiveWindow()
    {
        // The standard Avalonia way to find the main window 
        // without passing references around manually.
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.Windows.FirstOrDefault(w => w.IsActive) ?? desktop.MainWindow;
        }
        return null;
    }
}