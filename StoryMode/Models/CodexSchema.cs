using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace StoryMode.Models;

public enum FieldDataType
{
    Text = 0,       // Simple one-line text
    MultiLine = 1,  // Large text block
    Number = 2,     // Integer/Double
    Boolean = 3,    // Checkbox
    Reference = 4   // Link to another Entry ID
}

public class CodexType
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty; // e.g., "Character"

    public string Emoji { get; set; } = "📄"; // e.g., "👤" or "🏰"

    public string ColorHex { get; set; } = "#CCCCCC";

    // Navigation Property: The fields that make up this type
    public List<FieldDefinition> Fields { get; set; } = new();
}

public class FieldDefinition
{
    [Key]
    public int Id { get; set; }

    public int CodexTypeId { get; set; } // The Parent Type

    [Required]
    public string Name { get; set; } = string.Empty; // Label: "Hit Points"

    [Required]
    public string Key { get; set; } = string.Empty; // JSON Key: "hp"

    public FieldDataType DataType { get; set; }

    // If DataType == Reference, this restricts it to a specific Type 
    // (e.g., only link to "Factions")
    public int? TargetCodexTypeId { get; set; } 
}

public class CodexEntry
{
    [Key]
    public int Id { get; set; }

    public int CodexTypeId { get; set; }
    public CodexType CodexType { get; set; } = null!;

    [Required]
    public string Name { get; set; } = string.Empty; // e.g., "Gandalf"

    // The Payload: { "age": 2000, "is_wizard": true }
    public string JsonData { get; set; } = "{}"; 

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
}