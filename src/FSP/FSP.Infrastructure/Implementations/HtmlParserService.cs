using FSP.Domain.Entities.Core;
using FSP.Domain.Interfaces.Core;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace FSP.Infrastructure.Implementations;

public class HtmlParserService : IHtmlParserService
{
    private readonly ILogger<HtmlParserService> _logger;

    public HtmlParserService(ILogger<HtmlParserService> logger)
    {
        _logger = logger;
    }

    public async Task<List<Goalkeeping>> ExtractGoalkeepingTableAsync(string html, string selector)
    {
        return await Task.Run(() =>
        {
            var goalkeepers = new List<Goalkeeping>();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var table = htmlDoc.DocumentNode.SelectSingleNode(selector);
            if (table == null) return goalkeepers;

            var rows = table.SelectNodes(".//tbody/tr[not(contains(@class, 'thead'))]");
            if (rows == null) return goalkeepers;

            foreach (var row in rows)
            {
                try
                {
                    var cells = row.SelectNodes(".//td|.//th");
                    if (cells == null || cells.Count < 5) continue;

                    var goalkeeper = new Goalkeeping
                    {
                        PlayerName = CleanText(cells[0].InnerText),
                        Nation = CleanText(cells[1].InnerText),
                        Position = CleanText(cells[2].InnerText),
                        Age = CleanText(cells[3].InnerText),
                        MatchPlayed = ParseInt(cells[4].InnerText),
                        Minutes = CleanText(cells[6].InnerText),
                        GoalsAgainst = ParseInt(cells[8].InnerText),
                        CleanSheets = ParseInt(cells[15].InnerText)
                    };

                    if (!string.IsNullOrEmpty(goalkeeper.PlayerName) && goalkeeper.PlayerName != "Player")
                    {
                        goalkeepers.Add(goalkeeper);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error parsing goalkeeper row");
                }
            }

            return goalkeepers;
        });
    }

    public async Task<List<MatchLog>> ExtractMatchLogTableAsync(string html, string selector)
    {
        return await Task.Run(() =>
        {
            var matchLogs = new List<MatchLog>();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var table = htmlDoc.DocumentNode.SelectSingleNode(selector);
            if (table == null) return matchLogs;

            var rows = table.SelectNodes(".//tbody/tr[not(contains(@class, 'thead'))]");
            if (rows == null) return matchLogs;

            foreach (var row in rows)
            {
                try
                {
                    var cells = row.SelectNodes(".//td");
                    if (cells == null || cells.Count < 10) continue;

                    var matchLog = new MatchLog
                    {
                        Date = CleanText(cells[0].InnerText),
                        Competition = CleanText(cells[1].InnerText),
                        Round = CleanText(cells[2].InnerText),
                        Venue = CleanText(cells[3].InnerText),
                        Result = CleanText(cells[4].InnerText),
                        GoalsFor = CleanText(cells[5].InnerText),
                        GoalsAgainst = CleanText(cells[6].InnerText),
                        Opponent = CleanText(cells[7].InnerText),
                        Formation = CleanText(cells[10].InnerText)
                    };

                    matchLogs.Add(matchLog);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error parsing match log row");
                }
            }

            return matchLogs;
        });
    }

    public async Task<List<Player>> ExtractPlayersTableAsync(string html, string selector)
    {
        return await Task.Run(() =>
        {
            var players = new List<Player>();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var table = htmlDoc.DocumentNode.SelectSingleNode(selector);
            if (table == null)
            {
                _logger.LogWarning("Table not found with selector: {Selector}", selector);
                return players;
            }

            var rows = table.SelectNodes(".//tbody/tr[not(contains(@class, 'thead')) and ./th[@data-stat='player' and .//a]]");
            if (rows == null || rows.Count == 0)
            {
                _logger.LogWarning("No valid player rows found in table");
                return players;
            }

            foreach (var row in rows)
            {
                try
                {
                    var cells = row.SelectNodes("./th | ./td");
                    if (cells == null || cells.Count < 33) continue;

                    var playerLink = cells[0].SelectSingleNode(".//a");
                    var playerRefId = playerLink?.GetAttributeValue("href", "")
                        .Split('/', StringSplitOptions.RemoveEmptyEntries)
                        .ElementAtOrDefault(3) ?? string.Empty;

                    var player = new Player
                    {
                        PlayerName = CleanText(cells[0].InnerText),
                        Nation = CleanText(cells[1].InnerText),
                        Position = CleanText(cells[2].InnerText),
                        Age = CleanText(cells[3].InnerText),
                        MatchPlayed = ParseInt(cells[4].InnerText),
                        Starts = ParseInt(cells[5].InnerText),
                        Minutes = ParseInt(cells[6].InnerText),
                        NineteenMinutes = CleanText(cells[7].InnerText),
                        Goals = ParseInt(cells[8].InnerText),
                        Assists = ParseInt(cells[9].InnerText),
                        GoalsAssists = ParseInt(cells[10].InnerText),
                        NonPenaltyGoals = ParseInt(cells[11].InnerText),
                        PenaltyKicksMade = ParseInt(cells[12].InnerText),
                        PenaltyKickAttempted = ParseInt(cells[13].InnerText),
                        YellowCards = ParseInt(cells[14].InnerText),
                        RedCards = ParseInt(cells[15].InnerText),
                        ExpectedGoals = ParseFloat(cells[16].InnerText),
                        NonPenaltyExpectedGoals = ParseFloat(cells[17].InnerText),
                        ExpectedAssistedGoals = ParseFloat(cells[18].InnerText),
                        NonPenaltyExpectedGoalsPlusAssistedGoals = ParseFloat(cells[19].InnerText),
                        ProgressiveCarries = ParseInt(cells[20].InnerText),
                        ProgressivePasses = ParseInt(cells[21].InnerText),
                        ProgressiveReceptions = ParseInt(cells[22].InnerText),
                        GoalsPer90s = CleanText(cells[23].InnerText),
                        AssistsPer90s = CleanText(cells[24].InnerText),
                        GoalsAssistsPer90s = CleanText(cells[25].InnerText),
                        NonPenaltyGoalsPer90s = CleanText(cells[26].InnerText),
                        NonPenaltyGoalsAssistsPer90s = CleanText(cells[27].InnerText),
                        ExpectedGoalsPer90 = CleanText(cells[28].InnerText),
                        ExpectedAssistedGoalsPer90 = CleanText(cells[29].InnerText),
                        ExpectedGoalsPlusAssistedGoalsPer90 = CleanText(cells[30].InnerText),
                        NonPenaltyExpectedGoalsPer90 = CleanText(cells[31].InnerText),
                        NonPenaltyExpectedGoalsPlusAssistedGoalsPer90 = CleanText(cells[32].InnerText),
                        PlayerRefId = playerRefId
                    };

                    if (!string.IsNullOrEmpty(player.PlayerName) && !player.PlayerName.Equals("Player", StringComparison.OrdinalIgnoreCase))
                    {
                        players.Add(player);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error parsing player row - skipping");
                }
            }

            _logger.LogInformation("Extracted {Count} players from HTML", players.Count);
            return players;
        });
    }

    public async Task<List<SquadStandard>> ExtractSquadStandardTableAsync(string html, string selector)
    {
        return await Task.Run(() =>
        {
            var squads = new List<SquadStandard>();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var table = htmlDoc.DocumentNode.SelectSingleNode(selector);
            if (table == null)
            {
                _logger.LogWarning("Table not found with selector: {Selector}", selector);
                return squads;
            }

            var rows = table.SelectNodes(".//tbody/tr[not(contains(@class, 'thead'))]");
            if (rows == null)
            {
                _logger.LogWarning("No data rows found in table");
                return squads;
            }

            foreach (var row in rows)
            {
                try
                {
                    var cells = row.SelectNodes(".//td|.//th");
                    if (cells == null || cells.Count < 31) continue;

                    var squad = new SquadStandard
                    {
                        Squad = CleanText(cells[0].InnerText),
                        NumberOfPlayer = ParseInt(cells[1].InnerText),
                        AverageAge = ParseFloat(cells[2].InnerText),
                        Possession = ParseFloat(cells[3].InnerText),
                        MatchesPlayed = ParseInt(cells[4].InnerText),
                        Starts = ParseInt(cells[5].InnerText),
                        Minutes = ParseInt(cells[6].InnerText),
                        Nineties = ParseFloat(cells[7].InnerText),
                        Goals = ParseInt(cells[8].InnerText),
                        Assists = ParseInt(cells[9].InnerText),
                        GoalsPlusAssists = ParseInt(cells[10].InnerText),
                        GoalsMinusPenaltyKicks = ParseInt(cells[11].InnerText),
                        PenaltyKicks = ParseInt(cells[12].InnerText),
                        PenaltyKickAttempts = ParseInt(cells[13].InnerText),
                        YellowCards = ParseInt(cells[14].InnerText),
                        RedCards = ParseInt(cells[15].InnerText),
                        ExpectedGoals = ParseFloat(cells[16].InnerText),
                        NonPenaltyExpectedGoals = ParseFloat(cells[17].InnerText),
                        ExpectedAssistedGoals = ParseFloat(cells[18].InnerText),
                        NonPenaltyExpectedGoalsPlusAssistedGoals = ParseFloat(cells[19].InnerText),
                        ProgressiveCarries = ParseInt(cells[20].InnerText),
                        ProgressivePasses = ParseInt(cells[21].InnerText),
                        GoalsPer90 = ParseFloat(cells[22].InnerText),
                        AssistsPer90 = ParseFloat(cells[23].InnerText),
                        GoalsPlusAssistsPer90 = ParseFloat(cells[24].InnerText),
                        GoalsMinusPenaltyKicksPer90 = ParseFloat(cells[25].InnerText),
                        GoalsPlusAssistsMinusPenaltyKicksPer90 = ParseFloat(cells[26].InnerText),
                        ExpectedGoalsPer90 = ParseFloat(cells[27].InnerText),
                        ExpectedAssistedGoalsPer90 = ParseFloat(cells[28].InnerText),
                        ExpectedGoalsPlusAssistedGoalsPer90 = ParseFloat(cells[29].InnerText),
                        NonPenaltyExpectedGoalsPer90 = ParseFloat(cells[30].InnerText),
                        NonPenaltyExpectedGoalsPlusAssistedGoalsPer90 = ParseFloat(cells[31].InnerText)
                    };

                    if (!string.IsNullOrEmpty(squad.Squad) && squad.Squad != "Squad")
                    {
                        squads.Add(squad);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error parsing squad row");
                }
            }

            _logger.LogInformation("Extracted {Count} squads from HTML", squads.Count);
            return squads;
        });
    }

    public async Task<List<Shooting>> ExtractShootingTableAsync(string html, string selector)
    {
        return await Task.Run(() =>
        {
            var shootings = new List<Shooting>();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var table = htmlDoc.DocumentNode.SelectSingleNode(selector);
            if (table == null) return shootings;

            var rows = table.SelectNodes(".//tbody/tr[not(contains(@class, 'thead'))]");
            if (rows == null) return shootings;

            foreach (var row in rows)
            {
                try
                {
                    var cells = row.SelectNodes(".//td|.//th");
                    if (cells == null || cells.Count < 5) continue;

                    var shooting = new Shooting
                    {
                        PlayerName = CleanText(cells[0].InnerText),
                        Nation = CleanText(cells[1].InnerText),
                        Position = CleanText(cells[2].InnerText),
                        Age = CleanText(cells[3].InnerText),
                        Goals = ParseInt(cells[5].InnerText),
                        ShotsTotal = ParseInt(cells[6].InnerText),
                        ShotsOnTarget = ParseInt(cells[7].InnerText),
                        PenaltyKicksMade = ParseInt(cells[14].InnerText),
                        PenaltyKicksAttempted = ParseInt(cells[15].InnerText)
                    };

                    if (!string.IsNullOrEmpty(shooting.PlayerName) && shooting.PlayerName != "Player")
                    {
                        shootings.Add(shooting);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error parsing shooting row");
                }
            }

            return shootings;
        });
    }

    public async Task<PlayerDetails> ExtractPlayerDetailsAsync(string html, string selector, string clubName)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var infoNode = doc.DocumentNode.SelectSingleNode(selector);
        if (infoNode == null)
        {
            _logger.LogWarning("No <div id='info'> found for selector {Selector}", selector);
            return null!;
        }

        var playerDetails = new PlayerDetails();

        var fullNameNode = infoNode.SelectSingleNode(".//h1//span") ?? doc.DocumentNode.SelectSingleNode("//meta[@name='description']/@content");
        playerDetails.FullName = fullNameNode?.InnerText.Trim() ?? string.Empty;
        playerDetails.OriginalName = playerDetails.FullName;

        if (string.IsNullOrEmpty(playerDetails.FullName))
        {
            _logger.LogWarning("Empty or null FullName extracted from <div id='info'>");
            return null!;
        }

        var positionNode = infoNode.SelectSingleNode(".//p[strong[text()='Position:']]");
        if (positionNode != null)
        {
            var positionText = positionNode.InnerText.Replace("Position:", "").Trim();
            playerDetails.Position = positionText.Split('▪').First().Trim();
        }

        var bornNode = infoNode.SelectSingleNode(".//p[strong[text()='Born:']]//span[@data-birth]");
        playerDetails.Born = bornNode?.GetAttributeValue("data-birth", "") ?? string.Empty;

        var citizenshipNode = infoNode.SelectSingleNode(".//p[strong[text()='Citizenship:']]");
        playerDetails.Citizenship = citizenshipNode?.InnerText.Replace("Citizenship:", "").Trim() ?? string.Empty;

        playerDetails.PlayerRefId = GenerateNumericPlayerRefId(playerDetails.FullName, clubName);
        _logger.LogDebug("Generated PlayerRefId for {FullName}: {PlayerRefId}", playerDetails.FullName, playerDetails.PlayerRefId);

        return playerDetails;
    }

    #region helpers
    private string GenerateNumericPlayerRefId(string playerName, string clubName)
    {
        var input = $"{playerName}_{clubName}";
        using var md5 = System.Security.Cryptography.MD5.Create();
        var hashBytes = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
        long result = BitConverter.ToInt64(hashBytes, 0) & long.MaxValue;
        return result.ToString();
    }

    private string CleanText(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;

        return System.Net.WebUtility.HtmlDecode(text)
            .Replace("\n", "")
            .Replace("\t", "")
            .Replace("\r", "")
            .Trim();
    }

    private int ParseInt(string text)
    {
        if (string.IsNullOrEmpty(text)) return 0;

        var cleanText = CleanText(text);
        if (int.TryParse(cleanText, out int result))
        {
            return result;
        }
        return 0;
    }

    private float ParseFloat(string text)
    {
        if (string.IsNullOrEmpty(text)) return 0.0f;

        var cleanText = CleanText(text);
        if (float.TryParse(cleanText, out float result))
        {
            return result;
        }
        return 0.0f;
    }

    private decimal ParseDecimal(string text)
    {
        if (string.IsNullOrEmpty(text)) return 0;

        var cleanText = CleanText(text);
        if (decimal.TryParse(cleanText, out decimal result))
        {
            return result;
        }
        return 0;
    }
    #endregion
}
