namespace TribalWarsCheckAPI.Models;

public class PlayerRating
{
    public string Nick { get; set; } = string.Empty;
    public int GeneratedCommandNumber { get; set; }
    public int SentAttackNumber { get; set; }
    public int SentInTime { get; set; }
    public float SentInTimePercet => SentInTime / (float)GeneratedCommandNumber * 100.0F;
    public float SentPercent => SentAttackNumber / (float)GeneratedCommandNumber * 100.0F;
    public List<Village> NotAttacked { get; set; } = [];
    public List<Village> AttackedSoLate { get; set; } = [];
    public List<SthWrong> SthWrongs { get; set; } = [];

    public void Print()
    {
    Console.WriteLine($"[*]{Nick}[||]{SentInTimePercet}[||]{SentPercent}[||]{SentInTime}[||]{SentAttackNumber}[||]{GeneratedCommandNumber}[/*]");
    }
    public string GetSummary()
    {
        return $"[*]{Nick}[||]{SentInTimePercet}[||]{SentPercent}[||]{SentInTime}[||]{SentAttackNumber}[||]{GeneratedCommandNumber}[/*]";
    }
}

public class SthWrong
{
    public List<AttackCommand> DemandsCommands { get; set; } = [];
    public List<Attack> SentAttacks { get; set; } = new();
}
