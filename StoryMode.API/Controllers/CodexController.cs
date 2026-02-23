using Microsoft.AspNetCore.Mvc;
using StoryMode.Core.Models;

namespace StoryMode.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CodexController : ControllerBase
{
    private static readonly List<CodexEntry> _mockDb = new()
    {
        new CodexEntry()
        {
            Id = 1,
            Name = "Gandalf (SRE CHECK)",
            JsonData = "{ \"status\": \"You shall not crash\" }"
        }
    };

    private readonly ILogger<CodexController> _logger;
    
    public CodexController(ILogger<CodexController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<List<CodexEntry>> Get()
    {
        _logger.LogInformation("Codex accessed from Kubernetes!");
        return _mockDb;
    }
    
    [HttpGet("{id}")]
    public IEnumerable<CodexEntry> Get(int id)
    {
        _logger.LogInformation($"Retrieving Codex entry with ID: {id}");
        return _mockDb;
    }

    [HttpGet("stress")]
    public IActionResult Stress()
    {
        // simulate slow db query
        Thread.Sleep(500);
        return Ok("That was heavy...");
    }
}