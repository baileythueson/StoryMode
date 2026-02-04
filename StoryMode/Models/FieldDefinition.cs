using System.ComponentModel.DataAnnotations;

namespace StoryMode.Models;

public enum FieldDataType
{
    Text = 0,       // Simple one-line text
    MultiLine = 1,  // Large text block
    Number = 2,     // Integer/Double
    Boolean = 3,    // Checkbox
    Reference = 4,  // Link to another Entry ID
    Image = 5
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