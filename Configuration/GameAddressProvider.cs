using TribalWarsCheckAPI.Models;

namespace TribalWarsCheckAPI.Configuration;

public static class GameAddressProvider
{
    public static string Villages => $"https://{ActionSettings.World}.plemiona.pl/map/village.txt";
    public static string Players => $"https://{ActionSettings.World}.plemiona.pl/map/player.txt";
    public static string Tribes => $"https://{ActionSettings.World}.plemiona.pl/map/ally.txt";
    public static string Conquer => $"https://{ActionSettings.World}.plemiona.pl/map/conquer.txt";
}
