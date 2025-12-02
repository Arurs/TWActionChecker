namespace TribalWarsCheckAPI.Models;

public class Village
{
    public int Id { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Points { get; set; }
    public int PlayerID { get; set; }
    public Player? Player { get; set; }
    public float DistanceToFront { get; set; }
    public List<AttackCommand> AttackCommnadsFromThisVillage = new List<AttackCommand>();
    public float DistanceToTarget { get; set; }
    public Army? Army { get; set; }
    public Buildings? Buildings { get; set; }  
    
    public float CalculateDistance(Village other)
    {
        int deltaX = other.X - X;
        int deltaY = other.Y - Y;
  return ((float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY));
    }
    
    public void PrintCords()
    {
        Console.WriteLine($"{X}|{Y}");
    }
    
    public string Coords => $"{X}|{Y}";
}
