using System.Threading.Tasks;
using StoryMode.Models;

namespace StoryMode.Services;

/// <summary>
/// Provides services to manage Codex entries within the application context.
/// This class handles operations such as adding new entries and updating the project state
/// to reflect these changes. It utilizes a database context for data persistence and ensures
/// project metadata consistency by marking the project as modified upon updates.
/// </summary>
public class CodexService
{
    private readonly ProjectService _projectService;

    public CodexService(ProjectService projectService)
    {
        _projectService = projectService;
    }

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
        
        _projectService.MarkDirty();
    }
}