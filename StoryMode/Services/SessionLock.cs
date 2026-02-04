using System;
using System.IO;

namespace StoryMode.Services;

/// <summary>
/// Represents a mechanism to ensure exclusive access to a session directory by creating a lock file.
/// This class prevents multiple instances from accessing the same session simultaneously.
/// </summary>
public class SessionLock : IDisposable
{
    private FileStream? _lockStream;

    /// <summary>
    /// Represents a mechanism to enforce an exclusive lock on a session directory by creating a lock file.
    /// This ensures that only one instance can access the specified session directory at a time.
    /// </summary>
    public SessionLock(string sessionDirectory)
    {
        var lockFilePath = Path.Combine(sessionDirectory, ".lock");

        // Try to open the file with Exclusive access.
        // FileShare.None is the key: it prevents anyone else from opening it.
        try 
        {
            _lockStream = new FileStream(
                lockFilePath, 
                FileMode.Create, 
                FileAccess.ReadWrite, 
                FileShare.None, 
                4096, 
                FileOptions.DeleteOnClose); // Auto-delete when disposed/closed
        }
        catch (IOException)
        {
            // If we land here, it means someone else is ALREADY holding the lock.
            throw new InvalidOperationException("Session is already active in another window.");
        }
    }

    public void Dispose()
    {
        _lockStream?.Dispose(); // This releases the OS lock
        _lockStream = null;
        // File is auto-deleted due to FileOptions.DeleteOnClose
    }
}