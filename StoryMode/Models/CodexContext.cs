using System;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace StoryMode.Models;

/// <summary>
/// Represents the application's database context for managing Codex-related entities.
/// This context is responsible for managing interactions with the underlying SQLite database,
/// providing DbSet properties for accessing Codex entries, types, and field definitions.
/// </summary>
public class CodexContext : DbContext
{
    public DbSet<CodexEntry> Entries { get; set; }
    public DbSet<CodexType> Types { get; set; }
    public DbSet<FieldDefinition> Fields { get; set; }
    
    public string DbPath { get; private init; }
    
    private static string _currentDbPath = string.Empty;

    public CodexContext()
    {
        var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "StoryMode");
        Directory.CreateDirectory(folder);
        DbPath = Path.Combine(folder, "codex.db");
    }

    /// <summary>
    /// Sets the path to the database file for the application's database context.
    /// This method is primarily used to configure the database path dynamically
    /// during runtime based on specific application needs.
    /// </summary>
    /// <param name="path">The complete file system path to the database file.</param>
    public static void SetDatabasePath(string path)
    {
        _currentDbPath = path;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // Fallback for design-time or first run
        options.UseSqlite(string.IsNullOrEmpty(_currentDbPath)
            ? "Data Source=fallback.db"
            : $"Data Source={_currentDbPath}");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- SEEDING DEFAULT TYPES ---

        // 1. Define "Character" Type (ID: 1)
        modelBuilder.Entity<CodexType>().HasData(
            new CodexType { Id = 1, Name = "Character", Emoji = "👤", ColorHex = "#FF5733" }
        );

        // 2. Define "Location" Type (ID: 2)
        modelBuilder.Entity<CodexType>().HasData(
            new CodexType { Id = 2, Name = "Location", Emoji = "🏰", ColorHex = "#33FF57" }
        );

        // --- SEEDING FIELDS ---

        modelBuilder.Entity<FieldDefinition>().HasData(
            // Character Fields
            new FieldDefinition { Id = 1, CodexTypeId = 1, Name = "Age", Key = "age", DataType = FieldDataType.Number },
            new FieldDefinition { Id = 2, CodexTypeId = 1, Name = "Biography", Key = "bio", DataType = FieldDataType.MultiLine },
            new FieldDefinition { Id = 3, CodexTypeId = 1, Name = "Is Alive?", Key = "is_alive", DataType = FieldDataType.Boolean },
            
            // Location Fields
            new FieldDefinition { Id = 4, CodexTypeId = 2, Name = "Region", Key = "region", DataType = FieldDataType.Text },
            new FieldDefinition { Id = 5, CodexTypeId = 2, Name = "Population", Key = "population", DataType = FieldDataType.Number }
        );
        
        // OPTIONAL: Seed some default Tags if you want
        modelBuilder.Entity<Tag>().HasData(
            new Tag { Id = 1, Name = "WIP", ColorHex = "#FFD700" }, // Gold
            new Tag { Id = 2, Name = "Major", ColorHex = "#FF5733" } // Red
        );
    }
}