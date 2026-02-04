using System;
using System.IO;

namespace StoryMode.Utils;

/// <summary>
/// Provides utility methods for input and output operations.
/// </summary>
public static class IOUtils
{
    /// <summary>
    /// Deletes the specified file or directory if it exists and is empty.
    /// </summary>
    /// <param name="path">The full path of the file or directory to check and delete if empty.</param>
    /// <returns>
    /// True if the specified path was successfully deleted because it was empty;
    /// otherwise, false if the path does not exist or contains content.
    /// </returns>
    public static bool DeleteIfEmpty(string path)
    {
        // check if file or directory exists
        var exists = Directory.Exists(path) || File.Exists(path);
        if (exists) return false;
        if (Directory.Exists(path)) DeleteDirectoryIfEmpty(path);
        else if (File.Exists(path)) DeleteFileIfEmpty(path);
        return true;
    }

    /// <summary>
    /// Deletes the directory at the specified path if it exists and is empty.
    /// </summary>
    /// <param name="path">The full path of the directory to check and delete if empty.</param>
    private static void DeleteDirectoryIfEmpty(string path)
    {
        if (!Directory.Exists(path)) return;
        if (Directory.GetFileSystemEntries(path).Length == 0) Directory.Delete(path, true);
    }
    
    /// <summary>
    /// Deletes the file at the specified path if it exists and is empty.
    /// </summary>
    /// <param name="path">The full path of the file to check and delete.</param>
    private static void DeleteFileIfEmpty(string path)
    {
        if (!File.Exists(path)) return;
        if (new FileInfo(path).Length == 0) File.Delete(path);
    }
    
    /// <summary>
    /// Deletes the directory at the specified path if it exists.
    /// </summary>
    /// <param name="path">The full path of the directory to delete.</param>
    /// <returns>True if the directory was successfully deleted; otherwise, false if the directory does not exist.</returns>
    public static bool RmDirectoryExists(string path)
    {
        if (!Directory.Exists(path)) return false;
        Directory.Delete(path, true);
        return true;
    }
    
    /// <summary>
    /// Deletes the file at the specified path if it exists.
    /// </summary>
    /// <param name="path">The full path of the file to delete.</param>
    /// <returns>True if the file was successfully deleted; otherwise, false if the file does not exist.</returns>
    public static bool RmFileExists(string path)
    {
        if (!File.Exists(path)) return false;
        File.Delete(path);
        return true;
    }
    
    /// <summary>
    /// Updates the last write time of the file at the specified path to the current UTC time.
    /// </summary>
    /// <param name="path">The full path of the file to update.</param>
    /// <returns>True if the last write time was successfully updated; otherwise, false.</returns>
    public static bool Touch(string path)
    {
        try
        {
            File.SetLastWriteTimeUtc(path, DateTime.UtcNow);
            return true;
        }
        catch (IOException)
        {
            return false;
        }
    }
    
    /// <summary>
    /// Determines whether a file at the specified path is currently locked or in use by another process.
    /// </summary>
    /// <param name="path">The full path of the file to check.</param>
    /// <returns>True if the file is locked; otherwise, false.</returns>
    public static bool IsFileLocked(string path)
    {
        if (!File.Exists(path)) return false;
        
        try
        {
            using (File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None)) return false;
        }
        catch (IOException)
        {
            return true;
        }
    }
}