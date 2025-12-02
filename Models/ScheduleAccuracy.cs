namespace TribalWarsCheckAPI.Models;

public record MissingCommands(string Coords, string Nick, int CommandNumber, CommandType CommandType);

public record ScheduleAccuracy(List<PlayerRating> PlayerRatings, List<MissingCommands> MissingCommands);
