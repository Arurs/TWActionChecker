using TribalWarsCheckAPI.Interfaces;
using TribalWarsCheckAPI.Models;

namespace TribalWarsCheckAPI.Services;

public class ScheduleStorage : IScheduleStorage
{
    public List<AttackCommand> AttackCommands { get; set; } = new();
}
