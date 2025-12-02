using Microsoft.AspNetCore.Mvc;
using TribalWarsCheckAPI.Configuration;
using TribalWarsCheckAPI.Interfaces;
using TribalWarsCheckAPI.Models;

namespace TribalWarsCheckAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AllyAttackController : ControllerBase
{
    private readonly IAttackStorage _attackStorage;
    private readonly IActionChecker _actionChecker;

    public AllyAttackController(IAttackStorage attackStorage, IActionChecker actionChecker)
    {
        _attackStorage = attackStorage;
        _actionChecker = actionChecker;
    }

 [HttpPost("AddList")]
    public ActionResult PostList(List<Attack> attacks)
    {
        foreach (var attack in attacks)
       _attackStorage.Attacks.Add(attack);
        Console.WriteLine($"Added {attacks.Count} attacks");
  return Ok(new { message = "Attacks added successfully", count = attacks.Count });
    }

    [HttpDelete("Clear")]
    public ActionResult Clear()
    {
        _attackStorage.Attacks.Clear();
        Console.WriteLine("Cleared all attacks");
        return NoContent();
    }

    [HttpGet("CheckAction")]
    public async Task<ActionResult> CheckAction()
    {
        await _actionChecker.CheckAsync(ExportPathProvider.SQLPath);
        return Ok(new { message = "Check completed", attackCount = _attackStorage.Attacks.Count });
    }

    [HttpGet("Count")]
    public ActionResult GetCount()
    {
        return Ok(new { count = _attackStorage.Attacks.Count });
    }
}
