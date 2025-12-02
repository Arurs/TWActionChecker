using TribalWarsCheckAPI.Models;

namespace TribalWarsCheckAPI.Interfaces;

public interface ICommandGenerator
{
    AttackCommand Create(Village source, Target target);
    List<AttackCommand> GenerateAllCombinations(List<Village> sources, List<Target> targets);
  TimeSpan GetTimePerSquare(CommandType commandType);
}
