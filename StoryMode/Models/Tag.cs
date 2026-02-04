using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StoryMode.Models;

/// <summary>
/// Represents a tag used to categorize or label various codex entries in the system.
/// A tag can include an identifier, name, color code, and associated codex entries.
/// </summary>
public class Tag
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    public string ColorHex { get; set; }

    public List<CodexEntry> Entries { get; set; } = new();
}