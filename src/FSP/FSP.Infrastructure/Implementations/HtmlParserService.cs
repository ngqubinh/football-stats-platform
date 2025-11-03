using System.Text.RegularExpressions;
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

    public async Task<List<Player>> ExtractPlayersTableAsync(string html, string selector = null!)
    {
        return await Task.Run(() =>
        {
            var players = new List<Player>();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // DEBUG: Log all table IDs
            var allTables = doc.DocumentNode.SelectNodes("//table[@id]");
            if (allTables != null)
            {
                var tableIds = allTables.Select(t => t.GetAttributeValue("id", "no-id")).ToList();
                _logger.LogInformation("ALL TABLE IDs FOUND: {TableIds}", string.Join(", ", tableIds));
            }

            HtmlNode? table = null;

            // 1. Explicit selector
            if (!string.IsNullOrWhiteSpace(selector))
            {
                _logger.LogInformation("Searching with explicit selector: {Selector}", selector);
                table = doc.DocumentNode.SelectSingleNode(selector);
                _logger.LogInformation("Explicit selector result: {Result}", table != null ? "FOUND" : "NOT FOUND");
            }

            // 2. Fallback: squad ID
            if (table == null)
            {
                _logger.LogWarning("Falling back to squad ID extraction");
                var squadId = ExtractSquadId(doc);
                _logger.LogInformation("Extracted squad ID: {SquadId}", squadId);

                if (squadId != null)
                {
                    var fallbackSelector = $"//table[@id='stats_standard_{squadId}']";
                    _logger.LogInformation("Trying fallback selector: {Selector}", fallbackSelector);
                    table = doc.DocumentNode.SelectSingleNode(fallbackSelector);
                    _logger.LogInformation("Fallback selector result: {Result}", table != null ? "FOUND" : "NOT FOUND");
                }
            }

            // 3. Ultimate fallback: header pattern
            if (table == null)
            {
                _logger.LogWarning("Using ultimate fallback - pattern matching");
                table = FindTableByHeaderPattern(doc, new[] { "player", "nation", "pos", "age", "mp" }, "Player Standard");
                _logger.LogInformation("Ultimate fallback result: {Result}", table != null ? "FOUND" : "NOT FOUND");
            }

            if (table == null)
            {
                _logger.LogError("NO PLAYER TABLE FOUND AFTER ALL ATTEMPTS");
                return players;
            }

            var tableId = table.GetAttributeValue("id", "unknown");
            _logger.LogInformation("USING TABLE: {TableId}", tableId);

            // === STEP 1: Extract headers ===
            var headerRow = table.SelectSingleNode(".//thead/tr[last()]");
            if (headerRow == null)
            {
                _logger.LogError("No header row found in table {TableId}", tableId);
                return players;
            }

            var headerCells = headerRow.SelectNodes("./th | ./td");
            if (headerCells == null || headerCells.Count == 0)
            {
                _logger.LogError("No header cells found in table {TableId}", tableId);
                return players;
            }

            var headers = headerCells
                .Select(h => CleanHeader(h.InnerText))
                .ToList();

            _logger.LogInformation("TABLE HEADERS ({Count}): {Headers}",
                headers.Count, string.Join(" | ", headers.Take(20)));

            // === STEP 2: Build MULTI-INDEX MAP (supports duplicates) ===
            var colIndex = new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < headers.Count; i++)
            {
                var header = headers[i];
                if (string.IsNullOrEmpty(header)) continue;

                if (!colIndex.ContainsKey(header))
                    colIndex[header] = new List<int>();

                colIndex[header].Add(i);
            }

            // DEBUG: Log all header indices
            foreach (var kvp in colIndex.OrderBy(k => k.Value[0]))
            {
                _logger.LogInformation("Header '{Header}' → indices [{Indices}]",
                    kvp.Key, string.Join(", ", kvp.Value));
            }

            // === STEP 3: Get data rows ===
            var rows = table.SelectNodes(".//tbody/tr[not(contains(@class, 'thead')) and @data-row]") ??
                       table.SelectNodes(".//tbody/tr[not(contains(@class, 'thead'))]") ??
                       table.SelectNodes(".//tbody/tr");

            if (rows == null || rows.Count == 0)
            {
                _logger.LogWarning("No data rows found in table {TableId}", tableId);
                return players;
            }

            _logger.LogInformation("Processing {RowCount} rows from table {TableId}", rows.Count, tableId);

            // === STEP 4: Process each row ===
            foreach (var row in rows)
            {
                try
                {
                    var cells = row.SelectNodes("./th | ./td");
                    if (cells == null || cells.Count < 5) continue;

                    var playerName = GetCellText(cells, colIndex, "Player");
                    if (string.IsNullOrWhiteSpace(playerName) ||
                        playerName.Equals("Player", StringComparison.OrdinalIgnoreCase) ||
                        playerName.Contains("Squad Total", StringComparison.OrdinalIgnoreCase) ||
                        playerName.Contains("Opponent Total", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var playerLink = cells[0].SelectSingleNode(".//a");
                    var playerRefId = playerLink?.GetAttributeValue("href", "")
                        .Split('/', StringSplitOptions.RemoveEmptyEntries)
                        .ElementAtOrDefault(3) ?? string.Empty;

                    // Minutes logic
                    var minText = GetCellText(cells, colIndex, "Min");
                    var ninetyText = GetCellText(cells, colIndex, "90s");
                    int minutes = ParseInt(minText);
                    if (minutes == 0 && !string.IsNullOrWhiteSpace(ninetyText) && float.TryParse(ninetyText, out var nineties))
                    {
                        minutes = (int)(nineties * 90);
                    }

                    var player = new Player
                    {
                        PlayerName = playerName,
                        Nation = GetCellText(cells, colIndex, "Nation"),
                        Position = GetCellText(cells, colIndex, "Pos"),
                        Age = GetCellText(cells, colIndex, "Age"),
                        MatchPlayed = ParseInt(GetCellText(cells, colIndex, "MP")),
                        Starts = ParseInt(GetCellText(cells, colIndex, "Starts")),
                        Minutes = minutes,
                        NineteenMinutes = ninetyText,
                        PlayerRefId = playerRefId
                    };

                    // === TOTALS (first occurrence) ===
                    player.Goals = ParseInt(GetCellText(cells, colIndex, "Gls", 0));
                    player.Assists = ParseInt(GetCellText(cells, colIndex, "Ast", 0));
                    player.GoalsAssists = ParseInt(GetCellText(cells, colIndex, "G+A", 0));
                    player.NonPenaltyGoals = ParseInt(GetCellText(cells, colIndex, "G-PK", 0));
                    player.PenaltyKicksMade = ParseInt(GetCellText(cells, colIndex, "PK", 0));
                    player.PenaltyKickAttempted = ParseInt(GetCellText(cells, colIndex, "PKatt", 0));
                    player.YellowCards = ParseInt(GetCellText(cells, colIndex, "CrdY", 0));
                    player.RedCards = ParseInt(GetCellText(cells, colIndex, "CrdR", 0));

                    // === PER 90 (second occurrence) ===
                    player.GoalsPer90s = GetCellText(cells, colIndex, "Gls", 1);
                    player.AssistsPer90s = GetCellText(cells, colIndex, "Ast", 1);
                    player.GoalsAssistsPer90s = GetCellText(cells, colIndex, "G+A", 1);
                    player.NonPenaltyGoalsPer90s = GetCellText(cells, colIndex, "G-PK", 1);
                    player.NonPenaltyGoalsAssistsPer90s = GetCellText(cells, colIndex, "G+A-PK"); // only once

                    // === xG & Advanced (only if exist) ===
                    if (colIndex.ContainsKey(CleanHeader("xG")))
                    {
                        player.ExpectedGoals = ParseFloat(GetCellText(cells, colIndex, "xG", 0));
                        player.NonPenaltyExpectedGoals = ParseFloat(GetCellText(cells, colIndex, "npxG", 0));
                        player.ExpectedAssistedGoals = ParseFloat(GetCellText(cells, colIndex, "xAG", 0));
                        player.NonPenaltyExpectedGoalsPlusAssistedGoals = ParseFloat(GetCellText(cells, colIndex, "npxG+xAG", 0));
                    }

                    if (colIndex.ContainsKey(CleanHeader("PrgC")))
                    {
                        player.ProgressiveCarries = ParseInt(GetCellText(cells, colIndex, "PrgC"));
                        player.ProgressivePasses = ParseInt(GetCellText(cells, colIndex, "PrgP"));
                        player.ProgressiveReceptions = ParseInt(GetCellText(cells, colIndex, "PrgR"));
                    }

                    // === Per90 xG (second group) ===
                    if (colIndex.ContainsKey(CleanHeader("xG")) && colIndex[CleanHeader("xG")].Count > 1)
                    {
                        player.ExpectedGoalsPer90 = GetCellText(cells, colIndex, "xG", 1);
                        player.ExpectedAssistedGoalsPer90 = GetCellText(cells, colIndex, "xAG", 1);
                        player.ExpectedGoalsPlusAssistedGoalsPer90 = GetCellText(cells, colIndex, "xG+xAG", 1);
                        player.NonPenaltyExpectedGoalsPer90 = GetCellText(cells, colIndex, "npxG", 1);
                        player.NonPenaltyExpectedGoalsPlusAssistedGoalsPer90 = GetCellText(cells, colIndex, "npxG+xAG", 1);
                    }

                    players.Add(player);
                    _logger.LogInformation("Added player: {PlayerName} ({Position}) - {Nation}",
                        player.PlayerName, player.Position, player.Nation);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error parsing player row in table {TableId}", tableId);
                }
            }

            _logger.LogInformation("Extracted {Count} players from table {TableId}", players.Count, tableId);
            return players;
        });
    }
private string CleanHeader(string text)
{
    if (string.IsNullOrWhiteSpace(text)) return string.Empty;

    return Regex.Replace(text, @"\s+", " ")
               .Replace("-", "")
               .Replace("+", "")
               .Trim()
               .ToLowerInvariant();
}

private string GetCellText(
    HtmlNodeCollection cells,
    Dictionary<string, List<int>> colIndex,
    string header,
    int occurrence = 0)
{
    var key = CleanHeader(header);

    if (!colIndex.TryGetValue(key, out var indices) || occurrence >= indices.Count)
        return string.Empty;

    int idx = indices[occurrence];
    return idx < cells.Count ? CleanText(cells[idx].InnerText) : string.Empty;
}
    // ----  NEW METHOD  ----------------------------------------------------
    private string? ExtractSquadId(HtmlDocument doc)
    {
        // FBref always has a link to the squad page in the header:
        // <a href="/en/squads/defd54ac/2025-2026/FC-Metaloglobus-Bucuresti-Stats">FC Metaloglobus București</a>
        var link = doc.DocumentNode.SelectSingleNode("//a[contains(@href,'/en/squads/')]");
        if (link == null) return null;

        var href = link.GetAttributeValue("href", "");
        var parts = href.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 3) return null;               // safety

        var id = parts[2];                               // e.g. "defd54ac"
        _logger.LogDebug("Extracted squad ID {SquadId} from {Href}", id, href);
        return id;
    }

    // Optional: generic fallback that looks for a table whose first header row contains a known column
    private HtmlNode? FindTableByHeaderPattern(HtmlDocument doc, string[] headerNeedles, string tablePurpose)
    {
        var tables = doc.DocumentNode.SelectNodes("//table");
        if (tables == null) return null;

        foreach (var t in tables)
        {
            var th = t.SelectSingleNode(".//thead//th");
            if (th == null) continue;

            var text = CleanText(th.InnerText).ToLowerInvariant();
            if (headerNeedles.Any(n => text.Contains(n.ToLowerInvariant())))
            {
                _logger.LogDebug("Fallback – found {Purpose} table by header pattern", tablePurpose);
                return t;
            }
        }
        return null;
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
