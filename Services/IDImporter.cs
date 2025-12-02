using TribalWarsCheckAPI.Interfaces;
using TribalWarsCheckAPI.Models;
using TribalWarsCheckAPI.Configuration;
using System.Net;

namespace TribalWarsCheckAPI.Services;

public class IDImporter : IIDImporter
{
    private Dictionary<int, Village>? _lazyIdVillages;
    private Dictionary<int, Village>? _lazyCoordsVillages;
    private Dictionary<int, Player>? _lazyPlayers;
    private List<Tribe>? _lazyTribes;

    public async Task<List<Tribe>> ImportTribesWithDependencyAsync(List<string> shortNames)
    {
var tasks = shortNames.Select(ImportTribesWithDependencyAsync);
        return [.. (await Task.WhenAll(tasks))];
    }

    public async Task<Tribe> ImportTribesWithDependencyAsync(int tribeId)
    {
        var tribe = (await ImportTribesAsync().ConfigureAwait(false))
 .FirstOrDefault(e => e.Id == tribeId)
      ?? throw new KeyNotFoundException($"Tribe with ID {tribeId} not found.");

        await FillTribeAsync(tribe).ConfigureAwait(false);
        return tribe;
    }

    public async Task<Tribe> ImportTribesWithDependencyAsync(string shortName)
    {
        var tribe = (await ImportTribesAsync().ConfigureAwait(false))
            .FirstOrDefault(e => e.ShortName == shortName)
?? throw new KeyNotFoundException($"Tribe with short name {shortName} not found.");

   await FillTribeAsync(tribe).ConfigureAwait(false);
        return tribe;
    }

    private async Task FillTribeAsync(Tribe tribe)
    {
 var players = await ImportIdPlayersAsync().ConfigureAwait(false);
  tribe.Players = players.Values.Where(e => e.TribeID == tribe.Id).ToList();

        var villages = (await ImportIdVillagesAsync().ConfigureAwait(false)).Values;
        foreach (var player in tribe.Players)
  {
     player.Villages = villages.Where(e => e.PlayerID == player.Id).ToList();
        }

        foreach (var village in tribe.Players.SelectMany(e => e.Villages))
        {
      village.Player = tribe.Players.Single(e => e.Id == village.PlayerID);
        }
    }

    public async Task<Village?> GetVillageFromId(int id)
    {
   var villages = await ImportIdVillagesAsync().ConfigureAwait(false);
        return villages.TryGetValue(id, out var village) ? village : null;
    }

    public async Task<Village?> GetVillageForCordinateAsync(int x, int y)
    {
        var villages = await ImportCoordsVillagesAsync().ConfigureAwait(false);
        var key = x * 1000 + y;

        if (villages.TryGetValue(key, out var village))
            return village;

        return null;
    }

    private async Task<Dictionary<int, Village>> ImportIdVillagesAsync()
    {
        if (_lazyIdVillages != null) return _lazyIdVillages;
        await InitVillageDictionariesAsync().ConfigureAwait(false);
        return _lazyIdVillages!;
    }

    private async Task<Dictionary<int, Village>> ImportCoordsVillagesAsync()
    {
        if (_lazyCoordsVillages != null) return _lazyCoordsVillages;
      await InitVillageDictionariesAsync().ConfigureAwait(false);
      return _lazyCoordsVillages!;
    }

    private async Task InitVillageDictionariesAsync()
    {
 _lazyIdVillages = [];
        _lazyCoordsVillages = [];

        var responseString = await FetchDataAsync(GameAddressProvider.Villages).ConfigureAwait(false);
        var players = await ImportIdPlayersAsync();
        foreach (var line in responseString.Split("\n"))
        {
       var values = line.Split(',');
            if (values.Length > 6 && int.TryParse(values[0], out var id))
      {
        var playerId = int.Parse(values[^3]);
       players.TryGetValue(playerId, out var player);
    var village = new Village
   {
            Id = id,
    Points = int.Parse(values[^2]),
           PlayerID = playerId,
          Y = int.Parse(values[^4]),
 X = int.Parse(values[^5]),
 Player = player
   };
 _lazyIdVillages[id] = village;
      _lazyCoordsVillages[village.X * 1000 + village.Y] = village;
            }
            else
     {
           Console.WriteLine($"Error in village import: {line}");
   }
        }
    }

    private async Task<Dictionary<int, Player>> ImportIdPlayersAsync()
    {
      if (_lazyPlayers != null) return _lazyPlayers;

   _lazyPlayers = [];
    var responseString = await FetchDataAsync(GameAddressProvider.Players).ConfigureAwait(false);
        foreach (var line in responseString.Split("\n"))
        {
     var values = line.Split(',');
            if (values.Length > 2 && int.TryParse(values[0], out var id))
    {
                var nick = WebUtility.UrlDecode(values[1]);
   _lazyPlayers.Add(id, new Player
    {
             Id = id,
          Nick = nick,
        TribeID = int.Parse(values[2])
  });
     }
     else
   {
        Console.WriteLine($"Error in player import: {line}");
            }
   }
        return _lazyPlayers;
    }

  private async Task<List<Tribe>> ImportTribesAsync()
    {
   if (_lazyTribes != null) return _lazyTribes;

  _lazyTribes = [];
        var responseString = await FetchDataAsync(GameAddressProvider.Tribes).ConfigureAwait(false);
        foreach (var line in responseString.Split("\n"))
        {
            var values = line.Split(',');
    if (values.Length > 2 && int.TryParse(values[0], out var id))
            {
                _lazyTribes.Add(new Tribe
           {
             Id = id,
        Name = values[1],
  ShortName = values[2]
      });
  }
      else
            {
        Console.WriteLine($"Error in tribe import: {line}");
        }
        }
 return _lazyTribes;
    }

    private static async Task<string> FetchDataAsync(string url)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
    }

    public async Task<List<Player>> ImportPlayersAsync()
 {
     var playersIds = await ImportIdPlayersAsync();
        return playersIds.Values.ToList();
    }

    public async Task<List<Conquer>> ImportsConquersAsync()
    {
        var counquereds = new List<Conquer>();
        var responseString = await FetchDataAsync(GameAddressProvider.Conquer).ConfigureAwait(false);
        foreach (var line in responseString.Split("\n"))
        {
            var values = line.Split(',');
            if (values.Length == 4)
 {
   counquereds.Add(new Conquer(
         DateTimeOffset.FromUnixTimeSeconds(long.Parse(values[1])).DateTime,
          int.Parse(values[0]),
         int.Parse(values[2]),
int.Parse(values[3])
            ));
          }
        }
 return counquereds;
    }
}
