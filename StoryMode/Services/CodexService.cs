using System.Threading.Tasks;
using StoryMode.Models;

namespace StoryMode.Services;

/// <summary>
/// Provides services for managing Codex entries, enabling creation and storage of entries
/// within the application's database. This service interacts with the database context
/// to persist data, and notifies the project manager when changes occur.
/// </summary>
public class CodexService
{
    /// <summary>
    /// Asynchronously adds a new codex entry to the database and marks the project as modified.
    /// </summary>
    /// <param name="entry">The codex entry to be added to the database. This includes metadata, tags, and associated data.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task AddEntryAsync(CodexEntry entry)
    {
        using var db = new CodexContext();
        db.Entries.Add(entry);
        await db.SaveChangesAsync();
        
        ProjectManager.Instance.MarkDirty();
    }
}