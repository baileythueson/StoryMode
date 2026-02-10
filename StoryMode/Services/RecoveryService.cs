using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using StoryMode.Utils;

namespace StoryMode.Services;

/// <summary>
/// Responsible for diagnosing and repairing invalid or abandoned sessions.
/// </summary>
public class RecoveryService
{
    private readonly ILogger<RecoveryService> _logger;
    private readonly DialogService _dialogService;
    private readonly LanguageService _language;
    private readonly WorkspaceService _workspaceService; // Needs muscle to fix things

    public RecoveryService(
        ILogger<RecoveryService> logger, 
        DialogService dialogService, 
        LanguageService language,
        WorkspaceService workspaceService)
    {
        _logger = logger;
        _dialogService = dialogService;
        _language = language;
        _workspaceService = workspaceService;
    }

    /// <summary>
    /// Attempts to recover an existing, abandoned workspace.
    /// Returns TRUE if recovered, FALSE if the user chose to discard (or if it was corrupt).
    /// </summary>
    public async Task<bool> TryRecoverAsync(string projectFilePath, string workspacePath)
    {
        _logger.LogInformation("Attempting recovery for {Path}", workspacePath);

        // 1. Diagnose Database
        var dbPath = Path.Combine(workspacePath, "codex.db");
        if (!IsDatabaseIntact(dbPath))
        {
            _logger.LogWarning("Database corrupted. Cannot recover.");
            await _dialogService.AlertAsync(
                _language["Error.LoadTitle"], 
                "The previous session was corrupted and cannot be restored.", 
                AvaloniaUtils.GetActiveWindow());
            
            // We failed to recover, so we clean up and return false
            return false;
        }

        // 2. Ask User
        var fileInfo = new FileInfo(projectFilePath);
        var dirInfo = new DirectoryInfo(workspacePath);

        var restore = await _dialogService.ChoiceAsync(
            _language["Session.FoundTitle"],
            string.Format(_language["Session.FoundMessage"], fileInfo.LastWriteTime, dirInfo.LastWriteTime),
            _language["Session.Restore"], 
            _language["Session.Discard"], 
            AvaloniaUtils.GetActiveWindow()
        );

        if (restore)
        {
            IOUtils.Touch(Path.Combine(workspacePath, ".lock")); // Claim lock
            return true;
        }

        return false;
    }

    private bool IsDatabaseIntact(string databasePath)
    {
        if (!File.Exists(databasePath)) return false;
        try
        {
            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = databasePath,
                Mode = SqliteOpenMode.ReadOnly
            };
            using var connection = new SqliteConnection(builder.ToString());
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "PRAGMA integrity_check;";
            return command.ExecuteScalar()?.ToString() == "ok";
        }
        catch { return false; }
    }
}