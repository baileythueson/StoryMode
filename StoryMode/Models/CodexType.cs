using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace StoryMode.Models;

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