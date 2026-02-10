using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Microsoft.Extensions.Logging;
using MessageBox = StoryMode.Views.MessageBox;

namespace StoryMode.Services;

/// <summary>
/// Provides methods for displaying dialog windows for user interaction, such as confirmations and alerts.
/// </summary>
public class DialogService
{
    private readonly ILogger<DialogService> _logger;
    public DialogService(ILogger<DialogService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Displays a choice dialog with a specified title and message to the user, allowing selection between two options.
    /// </summary>
    /// <param name="title">The title of the choice dialog.</param>
    /// <param name="message">The message content displayed in the choice dialog.</param>
    /// <param name="option1">The text for the first option button.</param>
    /// <param name="option2">The text for the second option button.</param>
    /// <param name="window">The parent window associated with the choice dialog.</param>
    /// <returns>A task that resolves to a boolean indicating the selected option. Returns <c>true</c> if the user selects the first option; otherwise, <c>false</c>.</returns>
    /// <exception cref="ApplicationException">Thrown if required controls are not found in the dialog's layout.</exception>
    public async Task<bool> ChoiceAsync(string title, string message, string option1, string option2,
        Window window)
    {
        _logger.LogInformation("Showing choice dialog with title '{Title}' and message '{Message}'", title, message);

        _logger.LogInformation("Showing confirmation dialog with title '{Title}' and message '{Message}'", title, message);
        
        var dialog = new MessageBox();
        InitDialog(dialog, title, message);
        
        var result = false;
        var yesBtn = dialog.FindControl<Button>("YesButton");
        var noBtn = dialog.FindControl<Button>("NoButton");
        
        yesBtn?.Content = option1;
        noBtn?.Content = option2;

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
    /// Displays a confirmation dialog with a specified title and message to the user, allowing for a "Yes" or "No" response.
    /// </summary>
    /// <param name="title">The title of the confirmation dialog.</param>
    /// <param name="message">The message content displayed in the confirmation dialog.</param>
    /// <param name="window">The parent window associated with the confirmation dialog.</param>
    /// <returns>A task that resolves to a boolean indicating the user's response. Returns <c>true</c> if the user selects "Yes"; otherwise, <c>false</c>.</returns>
    /// <exception cref="ApplicationException">Thrown if required controls are not found in the dialog's layout.</exception>
    public async Task<bool> ConfirmAsync(string title, string message, Window window)
    {
        _logger.LogInformation("Showing confirmation dialog with title '{Title}' and message '{Message}'", title, message);
        
        var dialog = new MessageBox();
        InitDialog(dialog, title, message);
        
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
    public async Task AlertAsync(string title, string message, Window window)
    {
        _logger.LogInformation("Showing alert dialog with title '{Title}' and message '{Message}'", title, message);
        
        var dialog = new MessageBox();
        InitDialog(dialog, title, message);

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
    private void InitDialog(Window dialog, string title, string message)
    {
        var titleBlock = dialog.FindControl<TextBlock>("TitleBlock");
        var messageBlock = dialog.FindControl<TextBlock>("MessageBlock");
        if (titleBlock is null || messageBlock is null)
        {
            _logger.LogError("Required controls not found in dialog layout");
            throw new ApplicationException("Missing required controls in dialog layout"); // You forgot to rename them
        }
        titleBlock.Text = title;
        messageBlock.Text = message;
    }

}