using System;
using System.IO;
using System.IO.Compression;
using CommunityToolkit.Mvvm.ComponentModel;

namespace StoryMode.Services;

/// <summary>
/// Provides functionality to manage the current project and workspace in the application.
/// This class is responsible for loading, saving, and closing projects, as well as
/// maintaining the state of the project, including its workspace and file paths.
/// Implements the Singleton pattern for centralized access.
/// </summary>
public class ProjectManager : IDisposable
{
    public static ProjectManager Instance { get; } = new ProjectManager();
    
    public string? CurrentWorkspace { get; set; }
    public string? CurrentProject { get; set; }
    
    public bool IsDirty { get; private set; } = false;
    public string CurrentProjectPath => Path.Combine(CurrentWorkspace ?? "", CurrentProject ?? "");
    public string CurrentProjectFileName => Path.GetFileName(CurrentProjectPath);

    private SessionLock? _currentSessionLock;
    
    public void MarkDirty() => IsDirty = true;

    /// <summary>
    /// Loads a project from the specified file path into the current workspace.
    /// This method extracts the project's contents, initializes the workspace, and sets the database path for the project.
    /// </summary>
    /// <param name="storyFilePath">The file path of the project to load. The path must point to a valid project archive.</param>
    /// <exception cref="IOException">Thrown if the project file cannot be found or accessed.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown if the application lacks sufficient permissions to access the file or create directories.</exception>
    /// <exception cref="InvalidDataException">Thrown if the project archive is corrupted or cannot be extracted.</exception>
    public void LoadProject(string storyFilePath)
    {
        var sessionName = Guid.NewGuid().ToString();
        var tempRoot = Path.Combine(Path.GetTempPath(), "StoryMode-", sessionName);
        Directory.CreateDirectory(tempRoot);
        
        if (File.Exists(Path.Combine(tempRoot, storyFilePath)))
        {
            ZipFile.ExtractToDirectory(storyFilePath, tempRoot);
        }

        _currentSessionLock = new SessionLock(tempRoot);
        
        CurrentWorkspace = tempRoot;
        CurrentProject = storyFilePath;
        
        CodexContext.SetDatabasePath(Path.Combine(tempRoot, "codex.db"));
    }

    /// <summary>
    /// Saves the currently loaded project to the specified file path.
    /// This method creates an archive of the project directory and writes it to the target location.
    /// </summary>
    /// <param name="storyFilePath">The file path where the project should be saved. The path must be valid and writable.</param>
    /// <exception cref="ApplicationException">Thrown when a temporary file required for saving cannot be created.</exception>
    public void SaveProject(string storyFilePath)
    {
        if (string.IsNullOrWhiteSpace(CurrentWorkspace) || string.IsNullOrWhiteSpace(CurrentProject)) return;

        // atomic save
        var tempZip = Path.GetTempFileName();
        if (tempZip is null) throw new ApplicationException(); // TODO: LOGGING
        
        ZipFile.CreateFromDirectory(tempZip, CurrentProject);
        File.Copy(tempZip, CurrentProject, overwrite: true);
        File.Delete(tempZip);
        IsDirty = false;
    }

    /// <summary>
    /// Closes the currently loaded project and releases any resources associated with it.
    /// This method will dispose of the current session lock if one exists and reset the workspace and project state.
    /// </summary>
    /// <remarks>
    /// After calling this method, both the CurrentWorkspace and CurrentProject will be set to null,
    /// and any exclusive locks on the session directory will be released. This ensures the cleanup
    /// of resources associated with the current project.
    /// </remarks>
    public void CloseProject()
    {
        _currentSessionLock?.Dispose();
        _currentSessionLock = null;
        
        CurrentWorkspace = null;
        CurrentProject = null;
    }

    public void Dispose()
    {
        CloseProject();
        GC.SuppressFinalize(this);
    }
}