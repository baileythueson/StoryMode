using System;
using System.Collections.Generic;
using System.IO;
using StoryMode.Utils;

namespace StoryMode.Services;

public class RecoveryService
{
    private readonly string _rootTempPath = Path.Combine(Path.GetTempPath(), "StoryMode");

    public List<RecoverySession> ScanForCrashes()
    {
        var crashes = new List<RecoverySession>();
        if (!Directory.Exists(_rootTempPath)) return crashes;
        
        var dirs = Directory.GetDirectories(_rootTempPath);

        foreach (var dir in dirs)
        {
            var info = new DirectoryInfo(dir);
            
            var lockFile = Path.Combine(dir, ".lock");
            var dbFile = Path.Combine(dir, "codex.db");

            if (File.Exists(lockFile))
            {
                if (IOUtils.IsFileLocked(lockFile)) continue;
                crashes.Add(new RecoverySession(dir, info.LastWriteTime));
            }
            else if (File.Exists(dbFile))
            {
                crashes.Add(new RecoverySession(dir, info.LastWriteTime));
            }
            else
            {
                // delete empty folders
                IOUtils.DeleteIfEmpty(dir);
            }
        }
        
        return crashes;
    }
    
    public record RecoverySession(string Path, DateTime LastWriteTime);
}