namespace TribalWarsCheckAPI.Models;

public class ActionSettings
{
    public static string Name { get; set; } = "Sprawdzanie";
    public DateTime StartActionTime { get; set; } = new DateTime(2025, 11, 27, 12, 0, 0);
    public DateTime Date { get; set; } = new DateTime(2025, 11, 29);
    public TimeSpan GetDelay(CommandType commandType)
    {
        return TimeSpan.FromMinutes(90);
    }
    public static World World { get; set; } = World.pl218;
    public List<int> AllyIds { get; set; } = [260, 189, 309, 286, 210, 310, 197, 239, 164];
    public List<int> EnemyIds { get; set; } = [176, 268, 644, 208];
    public int MaxNobleDistance { get; set; } = 49;
    public OFfSettings OFfSettings { get; set; } = new();
    public CatasSettings CatasSettings { get; set; } = new();
    public FakeOffSettings FakeOffSettings { get; set; } = new();
    public FakeDeffSettings FakeDeffSettings { get; set; } = new();
  public QuadraNobleSettings QuadraNobleSettings { get; set; } = new();
    public FakeQuadraNobleSettings FakeQuadraNobleSettings { get; set; } = new();
}

public class OFfSettings
{
    public int MinOffUnits { get; set; } = 10000;
    public int MinDistanceFromFront { get; set; } = 10;
}

public class CatasSettings
{
    public int MinCatasNumber { get; set; } = 49;
    public int MinDistanceFromFront { get; set; } = 10;
    public int MaxOffUnits { get; set; } = 2000;
}

public class FakeOffSettings
{
    public int MinOffUnits { get; set; } = 5000;
    public int MinDistanceFromFront { get; set; } = 10;
}

public class FakeDeffSettings
{
    public int MaxOffUnits { get; set; } = 5000;
    public int MinDistanceFromFront { get; set; } = 10;
}

public class QuadraNobleSettings
{
    public int MinUnits { get; set; } = 400;
    public int MinDistanceFromFront { get; set; } = 0;
}

public class FakeQuadraNobleSettings
{
    public int MinUnits { get; set; } = 400;
    public int MinDistanceFromFront { get; set; } = 10;
}
