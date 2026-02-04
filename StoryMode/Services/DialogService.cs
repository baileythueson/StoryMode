using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using MessageBox = StoryMode.Views.MessageBox;

namespace StoryMode.Services;

/// <summary>
/// Provides methods for displaying dialog windows for user interaction, such as confirmations and alerts.
/// </summary>
public static class DialogService
{
    /// <summary>
    /// Displays a confirmation dialog with a specified title and message to the user, allowing for a "Yes" or "No" response.
    /// </summary>
    /// <param name="title">The title of the confirmation dialog.</param>
    /// <param name="message">The message content displayed in the confirmation dialog.</param>
    /// <param name="window">The parent window associated with the confirmation dialog.</param>
    /// <returns>A task that resolves to a boolean indicating the user's response. Returns <c>true</c> if the user selects "Yes"; otherwise, <c>false</c>.</returns>
    /// <exception cref="ApplicationException">Thrown if required controls are not found in the dialog's layout.</exception>
    public static async Task<bool> ConfirmAsync(string title, string message, Window window)
    {
        // Create the custom window we designed earlier
        var dialog = new MessageBox();
        InitDialog(dialog, title, message);
        
        // Setup buttons
        var result = false;
        var yesBtn = dialog.FindControl<Button>("YesButton");
        var noBtn = dialog.FindControl<Button>("NoButton");

        yesBtn?.Click += (_, _) =>
        {
            result = true;
            dialog.Close();
        };
        noBtn?.Click += (_, _) =>
        {
            result = false;
            dialog.Close();
        };

        await dialog.ShowDialog(window);
        return result;
    }

    /// <summary>
    /// Displays an alert dialog with a specified title and message to the user.
    /// </summary>
    /// <param name="title">The title of the alert dialog.</param>
    /// <param name="message">The message content displayed in the alert dialog.</param>
    /// <param name="window">The parent window associated with the alert dialog.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ApplicationException">Thrown if required controls are not found in the dialog's layout.</exception>
    public static async Task AlertAsync(string title, string message, Window window)
    {
        // Create the custom window we designed earlier
        var dialog = new MessageBox();
        InitDialog(dialog, title, message);

        // Setup buttons
        var okBtn = dialog.FindControl<Button>("OkButton");

        okBtn?.Click += (_, _) => { dialog.Close(); };


        await dialog.ShowDialog(window);
    }

    /// <summary>
    /// Initializes the given dialog window with the specified title and message.
    /// </summary>
    /// <param name="dialog">The dialog window to be initialized.</param>
    /// <param name="title">The title to be displayed in the dialog.</param>
    /// <param name="message">The message content to be displayed in the dialog.</param>
    /// <exception cref="ApplicationException">Thrown if the required title or message controls cannot be found in the dialog's layout.</exception>
    private static void InitDialog(Window dialog, string title, string message)
    {
        // Setup the text (assuming you named your controls in XAML)
        var titleBlock = dialog.FindControl<TextBlock>("TitleBlock");
        var messageBlock = dialog.FindControl<TextBlock>("MessageBlock");
        if (titleBlock is null || messageBlock is null) throw new ApplicationException(); // You forgot to rename them
        titleBlock.Text = title;
        messageBlock.Text = message;
    }

}