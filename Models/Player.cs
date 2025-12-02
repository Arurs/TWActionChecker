namespace TribalWarsCheckAPI.Models;

public class Player
{
    public int Id { get; set; }
    public string? Nick { get; set; }
    public int TribeID { get; set; }
    public List<Village> Villages { get; set; } = new();
    private int? _points;
    public int Points
    {
        get
        {
      if (_points.HasValue)
            {
   return _points.Value;
  }
            _points = Villages.Sum(e => e.Points);
            return _points.Value;
        }
    }

 public NobleLimit NobleLimit { get; set; } = new();
}

public class NobleLimit
{
    public int Limit { get; set; } = 0;
    public int Used { get; set; } = 0;
    public int Left => Limit - Used;
}
