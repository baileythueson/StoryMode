using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StoryMode.Models;

public class CodexEntry
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty; // e.g., "Gandalf"

    public int CodexTypeId { get; set; }
    public CodexType CodexType { get; set; } = null!;

    public List<Tag> Tags { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
    
    // The Payload: { "age": 2000, "is_wizard": true }
    public string JsonData { get; set; } = "{}"; 
}