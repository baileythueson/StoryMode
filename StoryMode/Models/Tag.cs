using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StoryMode.Models;

public class Tag
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    public string ColorHex { get; set; }

    public List<CodexEntry> Entries { get; set; } = new();
}