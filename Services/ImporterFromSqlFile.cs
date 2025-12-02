using TribalWarsCheckAPI.Interfaces;
using TribalWarsCheckAPI.Models;

namespace TribalWarsCheckAPI.Services;

public class ImporterFromSqlFile : IImporterFromSqlFile
{
    private readonly IIDImporter _idImporter;
    private readonly ICommandGenerator _commandGenerator;
    
    public ImporterFromSqlFile(IIDImporter iDImporter, ICommandGenerator commandGenerator)
    {
        _idImporter = iDImporter;
        _commandGenerator = commandGenerator;
    }

    public List<AttackCommand> Import(string path)
    {
        List<AttackCommand> commands = new List<AttackCommand>();
  var leanes = File.ReadAllLines(path);

        leanes = leanes.Skip(3).ToArray();

        foreach (var leane in leanes)
      {
         var x = leane.Replace("\"", string.Empty).Substring(1);
            x = x.Replace("\'", string.Empty);
            var parts = x.Split(",");
         string[] sourceParts = parts[4].Split('|');
            string[] destinationParts = parts[6].Split('|');
            AttackCommand command = new AttackCommand
  {
                MinTime = GetDateTimeFromSqlFormat(parts[2]),
     MaxTime = GetDateTimeFromSqlFormat(parts[3]),
         Source = _idImporter.GetVillageForCordinateAsync(int.Parse(sourceParts[0]), int.Parse(sourceParts[1])).Result!,
 Destination = _idImporter.GetVillageForCordinateAsync(int.Parse(destinationParts[0]), int.Parse(destinationParts[1])).Result!,
           CommandType = GetCommandType(parts[8]),
       };
  var minTargetTime = GetDateTimeFromSqlFormat(parts[14]);
      command.Source.Player!.Nick = parts[12];
            command.Target = new Target(1, command.Destination, minTargetTime, minTargetTime.AddHours(2), command.CommandType, DistanceType.RANDOM, 0);
            commands.Add(command);
        }
        return commands;
    }
    
    public List<AttackCommand> ParseFromPawrcioFormat(List<string> leanes)
    {
   _idImporter.ImportPlayersAsync().Wait();
        leanes = leanes.Skip(1).ToList();

     List<AttackCommand> commands = new List<AttackCommand>();
        foreach (var leane in leanes)
        {
    var x = leane.Replace("\"", string.Empty).Substring(1);
            x = x.Replace("\'", string.Empty);
            var parts = x.Split(",");
          
    var source = _idImporter.GetVillageFromId(int.Parse(parts[5])).Result;
  var destination = _idImporter.GetVillageFromId(int.Parse(parts[7])).Result;
            
      if (source == null || destination == null)
  continue;
              
  AttackCommand command = new AttackCommand
         {
            MinTime = GetDateTimeFromSqlFormat(parts[2]),
             MaxTime = GetDateTimeFromSqlFormat(parts[3]),
        Source = source,
Destination = destination,
           CommandType = GetCommandType(parts[1]),
  };
            
         try
     {
        var distance = command.Source.CalculateDistance(command.Destination);
        var length = _commandGenerator.GetTimePerSquare(command.CommandType) * distance;

    command.Target = new Target(1, command.Destination, command.MinTime.Add(length), command.MaxTime.Add(length), command.CommandType, DistanceType.RANDOM, 0);
    commands.Add(command);
            }
 catch (Exception ex)
            {
   Console.WriteLine(ex.Message);
        }
  }
    return commands;
    }
    
    public List<AttackCommand> ImportFromPawrcioFormat(string path)
    {
   var leanes = File.ReadAllLines(path);
  leanes = leanes.Skip(1).ToArray();
        var commands = ParseFromPawrcioFormat(leanes.ToList());
        return commands;
    }

    private DateTime GetDateTimeFromSqlFormat(string sqlFormat)
    {
    return DateTime.Parse(sqlFormat);
    }
    
    private CommandType GetCommandType(string typeStr)
    {
        if (Enum.TryParse<CommandType>(typeStr.ToString(), true, out var commandType))
            return commandType;
        if (typeStr.ToLower().Contains("fejk szlachta") || typeStr.ToLower().Contains("szlachcic ma³a obstawa") || typeStr.ToLower().Contains("grubydalekizmalaobstawa"))
 return CommandType.GrubyDalekiZMalaObstawa;
        if (typeStr.ToLower().Contains("defoszlachta"))
    return CommandType.GrubyZZagrodaDeffa;
        if (typeStr.ToLower().Contains("szla") || typeStr.ToLower().Contains("gruby") || typeStr.ToLower().Contains("snob1z4"))
    return CommandType.GrubyZFullOffem;
      if (typeStr.ToLower().Contains("fejk") || typeStr.ToLower().Contains("fake") || typeStr.ToLower().Contains("taran"))
     return CommandType.FAKEOFF;
        if (typeStr.ToLower().Contains("off"))
            return CommandType.OFF;
        if (typeStr.ToLower().Contains("cata") || typeStr.ToLower().Contains("kata"))
        return CommandType.Catas;
    throw new NotImplementedException();
 }
}
