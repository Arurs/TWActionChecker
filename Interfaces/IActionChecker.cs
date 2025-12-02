using TribalWarsCheckAPI.Models;

namespace TribalWarsCheckAPI.Interfaces;

public interface IActionChecker
{
    Task CheckAsync(string sqlPath);
    ScheduleAccuracy Check(bool isCheckFakesActive);
}
