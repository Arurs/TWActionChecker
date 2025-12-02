using TribalWarsCheckAPI.Interfaces;
using TribalWarsCheckAPI.Models;
using TribalWarsCheckAPI.Helpers;
using System.Diagnostics;
using System.Text.Json;

namespace TribalWarsCheckAPI.Services;

public class ActionChecker : IActionChecker
{
    private const string urlsOpenerPath = "UrlsOpener.exe";
    private const int numberOfCommendToCkeck = 10;
    private readonly IImporterFromSqlFile _importerFromSqlFile;
    private readonly IIDImporter _idImporter;
    private readonly IAttackStorage _attackStorage;
    private readonly ICommandGenerator _commandGenerator;
    private readonly IScheduleStorage _scheduleStorage;

    public ActionChecker(
        IImporterFromSqlFile importerFromSqlFile,
        IIDImporter iDImporter,
        IAttackStorage attackStorage,
    ICommandGenerator commandGenerator,
        IScheduleStorage scheduleStorage)
    {
        _importerFromSqlFile = importerFromSqlFile;
        _idImporter = iDImporter;
        _attackStorage = attackStorage;
        _commandGenerator = commandGenerator;
        _scheduleStorage = scheduleStorage;
    }

    public async Task CheckAsync(string sqlPath)
    {
        var actionSettings = new ActionSettings();
        foreach (var id in actionSettings.AllyIds)
            await _idImporter.ImportTribesWithDependencyAsync(id);

        var commands = _importerFromSqlFile.ImportFromPawrcioFormat(sqlPath);
        var conquers = await _idImporter.ImportsConquersAsync();
        conquers = conquers.Where(e => e.DateTime > DateTime.Now.AddDays(-3)).ToList();
        commands = commands.Where(e => actionSettings.AllyIds.Contains(e.Source.Player?.TribeID ?? 0)).ToList();
        commands = commands.Where(e => e.Destination.PlayerID != 0).ToList();
        commands = commands.Where(e => e.Source.Player?.Nick != "pawcio231").ToList();
        commands = commands.Where(item => !actionSettings.AllyIds.Contains(item.Destination.Player?.TribeID ?? 0)).ToList();
        commands = commands.Where(e => e.Target != null && e.Target.MinTime > DateTime.Now).ToList();
        commands = commands.Where(e => !conquers.Any(conquer => conquer.VillageID == e.Source.Id)).ToList();
        commands = commands.Where(e => e.MaxTime < DateTime.Now.AddMinutes(-20)).ToList();

        var commandsByDest = commands.GroupBy(e => e.Destination).OrderByDescending(g => g.Count()).ToList();
        var commandsByDestToCheck = commandsByDest.Take(numberOfCommendToCkeck);

        try
        {
            for (int i = 0; i < commandsByDestToCheck.Count(); i += 40)
                await OpenBrowsersAsync(commandsByDestToCheck.Take(new Range(i, Math.Min(i + 40, commandsByDestToCheck.Count()))).Select(e => e.Key).ToList());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        Console.WriteLine($"Attacks before delay {_attackStorage.Attacks.Count}");
        await Task.Delay(10000);
        Console.WriteLine($"Attacks after delay {_attackStorage.Attacks.Count}");

        var attacksByDest = _attackStorage.Attacks.GroupBy(e => e.Destination).ToList();
        foreach (var attacks in attacksByDest)
        {
            Console.WriteLine($"{attacks.Key} {attacks.Count()}");
        }

        List<PlayerRating> playerRatings = new List<PlayerRating>();

        foreach (var commandsDest in commandsByDestToCheck)
        {
            try
            {
                string coords = $"{commandsDest.Key.X}|{commandsDest.Key.Y}";
                var attacksOnThisVillage = attacksByDest.FirstOrDefault(e => e.Key == coords);
                if (attacksOnThisVillage is null)
                {
                    Console.WriteLine($"{commandsDest.Key.X}|{commandsDest.Key.Y} brak rozkazow");
                    continue;
                }

                var commnadsByPlayers = commandsDest.GroupBy(e => e.Source.Player).ToList();
                foreach (var commandsByPlayer in commnadsByPlayers)
                {
                    if (commandsByPlayer.Key == null) continue;

                    var attacksPlayer = attacksOnThisVillage.Where(e => e.Player == commandsByPlayer.Key.Nick).ToList();
                    int attacksent = attacksPlayer.Count();
                    if (attacksent > commandsByPlayer.Count())
                        attacksent = commandsByPlayer.Count();

                    var playerRaing = playerRatings.FirstOrDefault(e => e.Nick == commandsByPlayer.Key.Nick);
                    if (playerRaing is null)
                    {
                        playerRaing = new PlayerRating() { Nick = commandsByPlayer.Key.Nick! };
                        playerRatings.Add(playerRaing);
                    }

                    var dateTimes = GetDateTimes(attacksPlayer);
                    dateTimes.Sort();
                    int sentInTimeOnThisVillage = 0;
                    bool sthWrong = false;

                    foreach (var command in commandsByPlayer.OrderBy(e => e.Target?.MinTime))
                    {
                        if (command.Target == null) continue;

                        var correspond = dateTimes.FirstOrDefault(e => e > command.Target.MinTime.AddMinutes(-10) && e < command.Target.MaxTime.AddMinutes(10));

                        if (correspond.Year > 2000)
                        {
                            playerRaing.SentInTime += 1;
                            sentInTimeOnThisVillage++;
                            dateTimes.Remove(correspond);
                        }
                        else
                            sthWrong = true;
                    }

                    if (sthWrong)
                    {
                        playerRaing.SthWrongs.Add(new SthWrong()
                        {
                            DemandsCommands = commandsByPlayer.ToList(),
                            SentAttacks = attacksPlayer
                        });
                    }

                    playerRaing.SentAttackNumber += attacksent;
                    playerRaing.GeneratedCommandNumber += commandsByPlayer.Count();
                    var NotAttackedCount = commandsByPlayer.Count() - attacksPlayer.Count();
                    if (NotAttackedCount < 0)
                        NotAttackedCount = 0;
                    int soLate = commandsByPlayer.Count() - NotAttackedCount - sentInTimeOnThisVillage;

                    if (NotAttackedCount > 0)
                    {
                        for (int i = 0; i < NotAttackedCount; i++)
                        {
                            playerRaing.NotAttacked.Add(commandsByPlayer.First().Destination);
                        }
                        Console.WriteLine($"{coords}, {commandsByPlayer.First().CommandType} [player]{commandsByPlayer.Key.Nick}[/player] should be {commandsByPlayer.Count()}, is {attacksPlayer.Count()}");
                    }

                    for (int i = 0; i < soLate; i++)
                    {
                        playerRaing.AttackedSoLate.Add(commandsByPlayer.First().Destination);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        playerRatings = playerRatings.OrderByDescending(e => e.GeneratedCommandNumber).OrderByDescending(e => e.SentInTimePercet).ToList();

        List<string> summary = [];

        summary.Add($"1. Sprawdzono komendy na {numberOfCommendToCkeck} wiosek wroga.\r\n" +
                 "2. Komendy z max timami po czasie publikowania statystyk nie by³y sprawdzane.\r\n" +
                 "3. Nie by³y sprawdzane rozkazy wykluczone przez naszych KO.\r\n" +
                 "4. Nie by³y sprawdzane rozkazy, które mia³y wyjœæ z wiosek przejêtych w ci¹gu ostatnich 3 dni.\r\n" +
       "5. Nie by³y sprawdzane rozkazy na wioski przejête przez gracz AHA w czasie wysy³ki.\r\n");

        summary.Add("[table]\r\n[*]Nick[||]Procent w wide³ki[||]Procent wys³anych[||]Ataki w wide³ki[||]Ataki wys³ane[||]Ataki wygenerowane[/*]");
        foreach (var rating in playerRatings)
        {
            summary.Add(rating.GetSummary());
        }
        summary.Add("[/table]");

        File.WriteAllLines("ActionCheckerOutput\\table.txt", summary);

        List<string> leanes = [];
        foreach (var rating in playerRatings)
        {
            if (rating.NotAttacked.Count > 0 || rating.AttackedSoLate.Count > 0)
            {
                leanes.Add($"[spoiler={rating.Nick}]");
                if (rating.NotAttacked.Count > 0)
                {
                    leanes.Add($"Wioski wroga na które brakuje rozkazów:");
                    foreach (var village in rating.NotAttacked)
                    {
                        leanes.Add(village.Coords);
                    }
                }
                if (rating.AttackedSoLate.Count > 0)
                {
                    leanes.Add($"Wioski wroga na które s¹ rozkazy poza wide³kami:");
                    foreach (var village in rating.AttackedSoLate)
                    {
                        leanes.Add(village.Coords);
                    }
                }
                leanes.Add("[/spoiler]");
            }

            if (rating.SthWrongs.Count == 0)
                continue;

            leanes.Add($"[spoiler={rating.Nick} - coœ posz³o nie tak]");

            foreach (var sthWrong in rating.SthWrongs)
            {
                leanes.Add($"Wioska: ");
                leanes.Add(sthWrong.DemandsCommands.First().Destination.Coords);
                leanes.Add($"Rozkazy do wys³ania:");
                foreach (var command in sthWrong.DemandsCommands)
                {
                    if (command.Target != null)
                    {
                        leanes.Add($"{command.Target.MinTime:dd.MM HH:mm} - {command.Target.MaxTime:dd.MM HH:mm}");
                    }
                }
                leanes.Add($"Wys³ane ataki:");
                foreach (var attack in sthWrong.SentAttacks)
                {
                    leanes.Add($"{attack.Time}");
                }
                leanes.Add("");
            }
            leanes.Add("[/spoiler]");
        }

        File.WriteAllLines("ActionCheckerOutput\\linczDescription.txt", leanes);

        var importantAttacks = _attackStorage.Attacks.Where(e => e.Type != "fake").ToList();

        List<ComebackArmy> comebackArmies = [];
        foreach (var attack in importantAttacks)
        {
            try
            {
                // Simplified - can be extended if needed
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        comebackArmies = comebackArmies.OrderByDescending(e => e.Time).ToList();
        string jsonString = JsonSerializer.Serialize(comebackArmies);
        File.WriteAllText("comebackTime.json", jsonString);

        Console.WriteLine("Check completed");
    }

    public ScheduleAccuracy Check(bool isCheckFakesActive)
    {
        var attacksByDest = _attackStorage.Attacks.GroupBy(e => e.Destination);
        var commands = _scheduleStorage.AttackCommands.Where(e => e.MaxTime < DateTime.Now.AddMinutes(-20)).ToList();

        if (!isCheckFakesActive)
            commands = commands.Where(e => e.CommandType != CommandType.FAKEOFF).ToList();

        var commandsByDest = commands.GroupBy(e => e.Destination).ToList();

        List<PlayerRating> playerRatings = new List<PlayerRating>();
        List<MissingCommands> missingCommands = new List<MissingCommands>();

        foreach (var commandsDest in commandsByDest)
        {
            try
            {
                string coords = $"{commandsDest.Key.X}|{commandsDest.Key.Y}";
                var attacksOnThisVillage = attacksByDest.FirstOrDefault(e => e.Key == coords);
                if (attacksOnThisVillage is null)
                    continue;

                var commnadsByPlayers = commandsDest.GroupBy(e => e.Source.Player).ToList();
                foreach (var commandsByPlayer in commnadsByPlayers)
                {
                    if (commandsByPlayer.Key == null) continue;

                    var attacksPlayer = attacksOnThisVillage.Where(e => e.Player == commandsByPlayer.Key.Nick).ToList();
                    int attacksent = attacksPlayer.Count();
                    if (attacksent > commandsByPlayer.Count())
                        attacksent = commandsByPlayer.Count();

                    var playerRaing = playerRatings.FirstOrDefault(e => e.Nick == commandsByPlayer.Key.Nick);
                    if (playerRaing is null)
                    {
                        playerRaing = new PlayerRating() { Nick = commandsByPlayer.Key.Nick! };
                        playerRatings.Add(playerRaing);
                    }

                    var dateTimes = GetDateTimes(attacksPlayer);
                    dateTimes.Sort();

                    foreach (var command in commandsByPlayer.OrderBy(e => e.Target?.MinTime))
                    {
                        if (command.Target == null) continue;

                        var correspond = dateTimes.FirstOrDefault(e => e > command.Target.MinTime);

                        if (correspond.Year > 2000)
                        {
                            playerRaing.SentInTime += 1;
                            dateTimes.Remove(correspond);
                        }
                    }

                    playerRaing.SentAttackNumber += attacksent;
                    playerRaing.GeneratedCommandNumber += commandsByPlayer.Count();

                    if (commandsByPlayer.Count() > attacksPlayer.Count())
                    {
                        Console.WriteLine($"{coords}, {commandsByPlayer.First().CommandType} [player]{commandsByPlayer.Key.Nick}[/player] should be {commandsByPlayer.Count()}, is {attacksPlayer.Count()}");
                        missingCommands.Add(new MissingCommands(coords, commandsByPlayer.Key.Nick, commandsByPlayer.Count() - attacksPlayer.Count(), commandsByPlayer.First().CommandType));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        playerRatings = playerRatings.OrderByDescending(e => e.GeneratedCommandNumber).OrderByDescending(e => e.SentInTimePercet).ToList();
        return new ScheduleAccuracy(playerRatings, missingCommands);
    }

    private List<DateTime> GetDateTimes(List<Attack> attacks)
    {
        List<DateTime> result = new List<DateTime>();

        foreach (var attack in attacks)
        {
            result.Add(GetDateTime(attack.Time));
        }

        return result;
    }

    private DateTime GetDateTime(string text)
    {
        var data = DateTime.Today;
        if (text.Contains("jutro"))
            data = data.AddDays(1);
        else if (text.Contains("dzisiaj"))
            data = DateTime.Today;
        else
        {
            var splited = text.Split(' ', '.');
            if (splited.Length > 3)
            {
                var month = int.Parse(splited[2]);
                var day = int.Parse(splited[1]);
                data = new DateTime(DateTime.Today.Year, month, day);
            }
        }

        var indexOfStartHour = text.IndexOf(" o ");
        var times = text.Substring(indexOfStartHour + 2, text.Length - 2 - indexOfStartHour);
        var timeParts = times.Split(':');

        if (timeParts.Length > 4 || timeParts.Length < 3)
            Console.WriteLine("dziwna liczba timeParts " + text);

        int hour = Convert.ToInt32(timeParts[0]);
        int minute = Convert.ToInt32(timeParts[1]);
        int second = Convert.ToInt32(timeParts[2]);
        int ms = timeParts.Length == 4 ? Convert.ToInt32(timeParts[3]) : 0;

        return new DateTime(data.Year, data.Month, data.Day, hour, minute, second, ms);
    }

    private async Task OpenBrowsersAsync(List<Village> villages)
    {
        var baseUrl = $"https://{ActionSettings.World}.plemiona.pl/game.php?village=36762&screen=info_village&id=";
        List<string> urls = new List<string>();

        foreach (Village village in villages)
        {
            var url = $"{baseUrl}{village.Id}";
            urls.Add(url);
        }

        Process.Start(urlsOpenerPath, urls);
        await Task.Delay(1300 * (urls.Count + 2));
    }
}
