using System;
using System.Collections.Generic;
using System.IO;
using StoryMode.Utils;

namespace StoryMode.Services;

/// <summary>
/// Provides functionality to scan for crash recovery sessions and manage their recovery or cleanup.
/// The service identifies potential crash data in a designated temporary directory and processes recovery sessions.
/// </summary>
public class RecoveryService
{
    private readonly string _rootTempPath = Path.Combine(Path.GetTempPath(), "StoryMode");

    /// <summary>
    /// Scans for crash recovery sessions within the designated temporary directory and attempts to identify
    /// potential crash data based on the presence of specific files. If a recovery session is found, it is added
    /// to the result set. Empty directories are cleaned up during the scan.
    /// </summary>
    /// <returns>
    /// A list of <see cref="RecoverySession"/> objects representing the identified crash recovery sessions.
    /// Returns an empty list if no sessions are found or the temporary directory does not exist.
    /// </returns>
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