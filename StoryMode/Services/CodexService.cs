using System.Threading.Tasks;
using StoryMode.Models;

namespace StoryMode.Services;

public class CodexService
{
    public async Task AddEntryAsync(CodexEntry entry)
    {
        using var db = new CodexContext();
        db.Entries.Add(entry);
        await db.SaveChangesAsync();
        
        ProjectManager.Instance.MarkDirty();
    }
}