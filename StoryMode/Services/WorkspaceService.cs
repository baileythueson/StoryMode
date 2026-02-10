using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StoryMode.Utils;

namespace StoryMode.Services;

/// <summary>
/// Responsible for the physical management of the temporary session directory.
/// Handles paths, file extraction, and directory cleanup.
/// </summary>
public class WorkspaceService
{
    private readonly ILogger<WorkspaceService> _logger;
    private readonly string _rootTempPath = Path.Combine(Path.GetTempPath(), "StoryMode");

    public WorkspaceService(ILogger<WorkspaceService> logger)
    {
        _logger = logger;
    }

    public string GetWorkspacePath(string projectFilePath)
    {
        var sessionName = Path.GetFileNameWithoutExtension(projectFilePath);
        return Path.Combine(_rootTempPath, sessionName);
    }

    public bool WorkspaceExists(string workspacePath) => Directory.Exists(workspacePath);

    public bool IsWorkspaceLocked(string workspacePath)
    {
        var lockPath = Path.Combine(workspacePath, ".lock");
        if (!File.Exists(lockPath)) return false;
        return !IOUtils.Touch(lockPath); // Returns false if OS denies access
    }

    /// <summary>
    /// Deletes any existing content and extracts the project fresh.
    /// This is the "Happy Path" loader.
    /// </summary>
    public async Task InitializeFreshWorkspaceAsync(string projectFilePath, string workspacePath)
    {
        _logger.LogInformation("Initializing fresh workspace at {Path}", workspacePath);
        
        await Task.Run(() => 
        {
            if (Directory.Exists(workspacePath))
            {
                // Force clean
                if (!IOUtils.RmDirectoryExists(workspacePath))
                    Directory.Delete(workspacePath, true);
            }
            
            Directory.CreateDirectory(workspacePath);
            ZipFile.ExtractToDirectory(projectFilePath, workspacePath);
        });
    }
}