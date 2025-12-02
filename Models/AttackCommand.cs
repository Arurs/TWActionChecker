namespace TribalWarsCheckAPI.Models;

public record AttackCommand
{
    public DateTime MinTime { get; set; }
    public DateTime MaxTime { get; set; }
    public Target? Target { get; set; }
    public CommandType CommandType { get; set; }
    public required Village Source { get; set; }
    public required Village Destination { get; set;}
    public int TargetId { get; set; }
    public string? Description { get; set; }

    public DateTime? PotencialComeBackTime()
    {
        if (Target == null) return null;
   return Target.MaxTime.Add(Target.MaxTime - MinTime);
    }
}
