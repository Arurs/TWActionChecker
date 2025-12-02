using TribalWarsCheckAPI.Interfaces;
using TribalWarsCheckAPI.Models;

namespace TribalWarsCheckAPI.Services;

public class CommandGenerator : ICommandGenerator
{
    public AttackCommand Create(Village source, Target target)
    {
var timePerSquare = GetTimePerSquare(target.CommandType);
 var distance = source.CalculateDistance(target.village);
        var commandDuration = distance * timePerSquare;

        var minTime = target.MinTime.Subtract(commandDuration);
        var maxTime = target.MaxTime.Subtract(commandDuration);

        if (target.village.Player is not null && target.village.Player.Points * 10 < source.Player!.Points)
    {
  minTime = new DateTime(2020, 01, 01);
  maxTime = new DateTime(2020, 01, 01);
        }

        if (source.Army?.ComeBackTime is not null && target.CommandType != CommandType.FAKEDEFF)
{
         if (minTime < source.Army.ComeBackTime)
  {
    minTime = new DateTime(2020, 01, 01);
         maxTime = new DateTime(2020, 01, 01);
          }
        }

        return new AttackCommand()
        {
          CommandType = target.CommandType,
          Source = source,
         Destination = target.village,
   MinTime = minTime,
            MaxTime = maxTime,
            Target = target,
    };
    }

    public List<AttackCommand> GenerateAllCombinations(List<Village> sources, List<Target> targets)
    {
    List<AttackCommand> attackCommands = new();
        foreach (var source in sources)
        {
  foreach (var target in targets)
                attackCommands.Add(Create(source, target));
 }
      return attackCommands;
    }

    public TimeSpan GetTimePerSquare(CommandType commandType)
    {
   switch (commandType)
 {
  case CommandType.Zwiad: return TimeSpan.FromMinutes(9); 
            case CommandType.Zwiadu5: return TimeSpan.FromMinutes(9);
       case CommandType.FAKEOFF: return TimeSpan.FromMinutes(30);
            case CommandType.FAKEDEFF: return TimeSpan.FromMinutes(30);
 case CommandType.FAKEFRONT: return TimeSpan.FromMinutes(30);
  case CommandType.FakeFiller: return TimeSpan.FromMinutes(30);
case CommandType.SpecialFake: return TimeSpan.FromMinutes(30);
            case CommandType.ZwiadZTaranem: return TimeSpan.FromMinutes(30);
      case CommandType.OFF: return TimeSpan.FromMinutes(30);
            case CommandType.OPENEROFF: return TimeSpan.FromMinutes(30);
     case CommandType.Catas: return TimeSpan.FromMinutes(30);
            case CommandType.KatapultyKoszary: return TimeSpan.FromMinutes(30);
case CommandType.KatapultyRatusz: return TimeSpan.FromMinutes(30);
            case CommandType.KatapultyKuznia: return TimeSpan.FromMinutes(30);
 case CommandType.KatapultySpichlerz: return TimeSpan.FromMinutes(30);
      case CommandType.KatapultyZagroda: return TimeSpan.FromMinutes(30);
   case CommandType.KatapultyStajnia: return TimeSpan.FromMinutes(30);
  case CommandType.XOFFX: return TimeSpan.FromMinutes(30);
            case CommandType.GrubyZPolowkaOffa: return TimeSpan.FromMinutes(35);
          case CommandType.GrubyZFullOffem: return TimeSpan.FromMinutes(35);
 case CommandType.GrubyZCwiartkaOffa: return TimeSpan.FromMinutes(35);
 case CommandType.GrubyZZagrodaDeffa: return TimeSpan.FromMinutes(35);
            case CommandType.GrubyPlus150Topa: return TimeSpan.FromMinutes(35);
            case CommandType.GrubyPlus100CK: return TimeSpan.FromMinutes(35);
    case CommandType.GrubyDalekiZMalaObstawa: return TimeSpan.FromMinutes(35);
            case CommandType.GrubyZDiodyZObstawa: return TimeSpan.FromMinutes(35);
   case CommandType.GrubyNaMs: return TimeSpan.FromMinutes(35);
case CommandType.GrubyBezObstawy: return TimeSpan.FromMinutes(35);
        case CommandType.Kareta: return TimeSpan.FromMinutes(35);
   case CommandType.FakeKareta: return TimeSpan.FromMinutes(35);
       default: throw new NotImplementedException(nameof(commandType));
     }
    }
}
