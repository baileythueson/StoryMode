using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StoryMode.Models;
using StoryMode.Utils;

namespace StoryMode.Services;

/// <summary>
/// Provides functionality to manage the current project and active session in the application.
/// This class acts as the orchestrator, delegating file system operations to WorkspaceService
/// and crash recovery logic to RecoveryService.
/// </summary>
public class ProjectService : IDisposable
{
    // -- State Properties --
    public string? CurrentWorkspace { get; private set; }
    public string? ProjectFilePath { get; private set; }
    public bool IsDirty { get; private set; } = false;

    // Helper properties for UI binding or logic
    public string CurrentProjectPath => Path.Combine(CurrentWorkspace ?? "", ProjectFilePath ?? "");
    public string CurrentProjectFileName => Path.GetFileName(ProjectFilePath ?? "");

    // -- Dependencies --
    private SessionLock? _currentSessionLock;
    private readonly ILogger<ProjectService> _logger;
    private readonly DialogService _dialogService;
    private readonly LanguageService _language;
    private readonly RecoveryService _recoveryService;
    private readonly WorkspaceService _workspaceService;

    public ProjectService(
        ILogger<ProjectService> logger,
        DialogService dialogService,
        LanguageService language,
        RecoveryService recoveryService,
        WorkspaceService workspaceService)
    {
        _logger = logger;
        _dialogService = dialogService;
        _language = language;
        _recoveryService = recoveryService;
        _workspaceService = workspaceService;

        _logger.LogInformation("ProjectService initialized.");
    }

    public void MarkDirty() => IsDirty = true;

    /// <summary>
    /// Loads a project from the specified file path.
    /// Manages the flow between checking for existing sessions, attempting recovery,
    /// and initializing a fresh workspace.
    /// </summary>
    /// <param name="storyFilePath">The file path of the project to load.</param>
    public async Task LoadProject(string storyFilePath)
    {
        if (string.IsNullOrWhiteSpace(storyFilePath) || !File.Exists(storyFilePath))
        {
            _logger.LogWarning("LoadProject aborted: File not found {Path}", storyFilePath);
            await _dialogService.AlertAsync("Error", "The project file could not be found.", AvaloniaUtils.GetActiveWindow());
            return;
        }

        _logger.LogInformation("Initiating project load for: {Path}", storyFilePath);

        try
        {
            // 1. Determine where the workspace should be
            var workspacePath = _workspaceService.GetWorkspacePath(storyFilePath);
            bool isRecoveredSession = false;

            // 2. Check if a workspace already exists (potential crash or active lock)
            if (_workspaceService.WorkspaceExists(workspacePath))
            {
                if (_workspaceService.IsWorkspaceLocked(workspacePath))
                {
                    _logger.LogWarning("Workspace locked: {Path}", workspacePath);
                    await _dialogService.AlertAsync(
                        _language["Error.SessionInUseTitle"],
                        _language["Error.SessionInUseMessage"],
                        AvaloniaUtils.GetActiveWindow());
                    return; // Abort load
                }

                // Workspace exists but is unlocked. Attempt recovery.
                isRecoveredSession = await _recoveryService.TryRecoverAsync(storyFilePath, workspacePath);
            }

            // 3. If no session existed, or recovery was declined/failed, initialize fresh
            if (!isRecoveredSession)
            {
                await _workspaceService.InitializeFreshWorkspaceAsync(storyFilePath, workspacePath);
            }

            // 4. Finalize Session Setup
            _currentSessionLock = new SessionLock(workspacePath);
            CurrentWorkspace = workspacePath;
            ProjectFilePath = storyFilePath;

            // Configure Database Context
            var dbPath = Path.Combine(workspacePath, "codex.db");
            CodexContext.SetDatabasePath(dbPath);

            IsDirty = false;
            _logger.LogInformation("Project loaded successfully. Workspace: {Path}", workspacePath);
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "IO Error during project load.");
            await _dialogService.AlertAsync("Error", $"Disk error while loading project: {ex.Message}", AvaloniaUtils.GetActiveWindow());
            CloseProject();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error loading project.");
            await _dialogService.AlertAsync(
                _language["Error.LoadTitle"],
                $"An unexpected error occurred: {ex.Message}",
                AvaloniaUtils.GetActiveWindow());

            CloseProject();
        }
    }

    /// <summary>
    /// Saves the currently loaded project to the specified file path.
    /// Uses an atomic save strategy (Write Temp Zip -> Copy -> Delete Temp).
    /// </summary>
    /// <param name="targetFilePath">The file path where the project should be saved.</param>
    public void SaveProject(string targetFilePath)
    {
        if (string.IsNullOrWhiteSpace(CurrentWorkspace) || string.IsNullOrWhiteSpace(ProjectFilePath))
        {
            _logger.LogWarning("SaveProject aborted: No active project.");
            return;
        }

        _logger.LogInformation("Saving project to: {Path}", targetFilePath);
        string? tempZip = null;

        try
        {
            // 1. Create a temporary zip file
            tempZip = Path.GetTempFileName();
            IOUtils.RmFileExists(tempZip); // GetTempFileName creates a 0-byte file; delete it so ZipFile can create a new one.

            // 2. Zip the workspace to the temp file
            ZipFile.CreateFromDirectory(CurrentWorkspace, tempZip);

            // 3. Atomic Copy (Overwrite the actual project file)
            File.Copy(tempZip, targetFilePath, overwrite: true);

            // 4. Update Lock Timestamp (Keep session alive)
            var lockPath = Path.Combine(CurrentWorkspace, ".lock");
            IOUtils.Touch(lockPath);

            IsDirty = false;
            _logger.LogInformation("Project saved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save project.");
            throw new IOException($"Failed to save project: {ex.Message}", ex);
        }
        finally
        {
            // 5. Cleanup Temp Artifacts
            if (tempZip != null && File.Exists(tempZip))
            {
                try
                {
                    File.Delete(tempZip);
                }
                catch (Exception cleanupEx)
                {
                    _logger.LogWarning(cleanupEx, "Failed to clean up temp zip file: {Path}", tempZip);
                }
            }
        }
    }

    /// <summary>
    /// Closes the currently loaded project and releases resources.
    /// </summary>
    public void CloseProject()
    {
        if (CurrentWorkspace == null) return;

        _logger.LogInformation("Closing project: {Path}", ProjectFilePath);

        try
        {
            _currentSessionLock?.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing session lock.");
        }
        finally
        {
            _currentSessionLock = null;
            CurrentWorkspace = null;
            ProjectFilePath = null;
            IsDirty = false;
        }
    }

    public void Dispose()
    {
        CloseProject();
        GC.SuppressFinalize(this);
    }
}