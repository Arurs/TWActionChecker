namespace TribalWarsCheckAPI.Models;

public class Target
{ 
    public int CommandNumber { get; set; }
    public Village village { get; set; }
    public DateTime MinTime { get; set; }
    public DateTime MaxTime { get; set; }
    public CommandType CommandType { get; set; }
    public DistanceType DistanceType { get; set; }
    public int Priority { get; set; }
    
    public Target(int CommandNumber, Village village, DateTime MinTime, DateTime MaxTime, CommandType CommandType, DistanceType DistanceType, int Priority)
    {
        this.CommandNumber = CommandNumber;
        this.village = village;
        this.MinTime = MinTime;
        this.MaxTime = MaxTime;
        this.CommandType = CommandType;
        this.DistanceType = DistanceType;
this.Priority = Priority;
    }
}
