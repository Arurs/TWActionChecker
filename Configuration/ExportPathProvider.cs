namespace TribalWarsCheckAPI.Configuration;

public static class ExportPathProvider
{
    public static string MainDirectory => Path.Combine(Directory.GetCurrentDirectory(), "ActionData");
    public static string SQLPath => Path.Combine(MainDirectory, "sql.txt");
}
