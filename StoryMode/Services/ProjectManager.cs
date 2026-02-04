using System;
using System.IO;
using System.IO.Compression;

namespace StoryMode.Services;

public class ProjectManager
{
    public static ProjectManager Instance { get; } = new ProjectManager();
    
    public string? CurrentWorkspace { get; set; }
    public string? CurrentProject { get; set; }

    public void LoadProject(string storyFilePath)
    {
        var sessionName = Guid.NewGuid().ToString();
        var tempRoot = Path.Combine(Path.GetTempPath(), "StoryMode-", sessionName);
        Directory.CreateDirectory(tempRoot);
        
        if (File.Exists(Path.Combine(tempRoot, storyFilePath)))
        {
            ZipFile.ExtractToDirectory(storyFilePath, tempRoot);
        }
        
        CurrentWorkspace = tempRoot;
        CurrentProject = storyFilePath;
        
        CodexContext.SetDatabasePath(Path.Combine(tempRoot, "codex.db"));
    }

    public void SaveProject(string storyFilePath)
    {
        if (string.IsNullOrWhiteSpace(CurrentWorkspace) || string.IsNullOrWhiteSpace(CurrentProject)) return;

        // atomic save
        var tempZip = Path.GetTempFileName();
        if (tempZip is null) throw new ApplicationException(); // TODO: LOGGING
        
        ZipFile.CreateFromDirectory(tempZip, CurrentProject);
        File.Copy(tempZip, CurrentProject, overwrite: true);
        File.Delete(tempZip);
    }
}