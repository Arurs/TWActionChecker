using TribalWarsCheckAPI.Models;

namespace TribalWarsCheckAPI.Interfaces;

public interface IIDImporter
{
 Task<Tribe> ImportTribesWithDependencyAsync(string shortName);
    Task<Tribe> ImportTribesWithDependencyAsync(int tribeId);
    Task<Village?> GetVillageForCordinateAsync(int x, int y);
    Task<Village?> GetVillageFromId(int id);
    Task<List<Tribe>> ImportTribesWithDependencyAsync(List<string> shortNames);
    Task<List<Player>> ImportPlayersAsync();
 Task<List<Conquer>> ImportsConquersAsync();
}
