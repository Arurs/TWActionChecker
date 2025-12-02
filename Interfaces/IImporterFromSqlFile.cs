using TribalWarsCheckAPI.Models;

namespace TribalWarsCheckAPI.Interfaces;

public interface IImporterFromSqlFile
{
    List<AttackCommand> Import(string path);
    List<AttackCommand> ImportFromPawrcioFormat(string path);
    List<AttackCommand> ParseFromPawrcioFormat(List<string> leanes);
}
