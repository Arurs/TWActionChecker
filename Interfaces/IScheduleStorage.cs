using TribalWarsCheckAPI.Models;

namespace TribalWarsCheckAPI.Interfaces;

public interface IScheduleStorage
{
    List<AttackCommand> AttackCommands { get; set; }
}
