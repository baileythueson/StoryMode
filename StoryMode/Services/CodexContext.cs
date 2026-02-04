using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using StoryMode.Models;

namespace StoryMode.Services;

public class CodexContext : DbContext
{
    public DbSet<CodexEntry> Entries { get; set; }
    public DbSet<CodexType> Types { get; set; }
    public DbSet<FieldDefinition> Fields { get; set; }
    
    public string DbPath { get; private init; }

    public CodexContext()
    {
        var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "StoryMode");
        Directory.CreateDirectory(folder);
        DbPath = Path.Combine(folder, "codex.db");
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
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
    }
}