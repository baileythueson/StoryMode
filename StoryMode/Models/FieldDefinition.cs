using System.ComponentModel.DataAnnotations;

namespace StoryMode.Models;

/// <summary>
/// Defines the various data types that can be assigned to a field within a Codex entry.
/// These data types dictate the kind of content that the field can hold and the way
/// the field is expected to be displayed or processed in the application.
/// </summary>
public enum FieldDataType
{
    Text = 0,       // Simple one-line text
    MultiLine = 1,  // Large text block
    Number = 2,     // Integer/Double
    Boolean = 3,    // Checkbox
    Reference = 4,  // Link to another Entry ID
    Image = 5
}

/// <summary>
/// Represents the definition of a field within a codex structure,
/// including its data type, associated parent type, and optional target type.
/// </summary>
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