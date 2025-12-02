namespace TribalWarsCheckAPI.Models;

public class Tribe
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string ShortName { get; set; }
    public List<Player> Players { get; set; } = new();
}
