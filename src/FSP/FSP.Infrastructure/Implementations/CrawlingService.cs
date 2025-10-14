using System.Net;
using FSP.Domain.Entities;
using FSP.Domain.Entities.Core;
using FSP.Domain.Interfaces.Core;
using FSP.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace FSP.Infrastructure.Implementations;

public class CrawlingService : ICrawlingService
{
    private readonly HttpClient _httpClient;
    private readonly IHtmlParserService _htmlParser;
    private readonly IDataStorageService _dataStorage;
    private readonly IImportService _importService;
    private readonly ILogger<CrawlingService> _logger;

    public CrawlingService(
        HttpClient httpClient,
        IHtmlParserService htmlParser,
        IDataStorageService dataStorage,
        IImportService importService,
        ILogger<CrawlingService> logger)
    {
        this._httpClient = httpClient;
        this._htmlParser = htmlParser;
        this._dataStorage = dataStorage;
        _logger = logger;
        this._importService = importService;

        this._httpClient.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
        this._httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task<Result<bool>> IsServerAliveAsync()
    {
        string correlationId = Guid.NewGuid().ToString();
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Operation"] = nameof(IsServerAliveAsync)
        }))
        {
            const string testUrl = "https://fbref.com/en/";
            try
            {
                _logger.LogInformation("Checking server status for {Url}", testUrl);
                var response = await _httpClient.GetAsync(testUrl);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Server fbref.com is alive (Status: {StatusCode})", response.StatusCode);
                    return Result<bool>.Ok(true);
                }

                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    _logger.LogWarning("Server fbref.com blocked crawling (403 Forbidden)");
                    return Result<bool>.Fail("Server blocked crawling (403 Forbidden).");
                }

                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    _logger.LogWarning("Server fbref.com rate-limited crawling (429 Too Many Requests)");
                    return Result<bool>.Fail("Server rate-limited crawling (429 Too Many Requests).");
                }

                _logger.LogWarning("Server fbref.com returned unexpected status: {StatusCode}", response.StatusCode);
                return Result<bool>.Fail($"Unexpected server status: {response.StatusCode}");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to connect to fbref.com: {Message}", ex.Message);
                return Result<bool>.Fail($"Failed to connect to server: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Request to fbref.com timed out");
                return Result<bool>.Fail("Request timed out.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error checking fbref.com: {Message}", ex.Message);
                return Result<bool>.Fail($"Unexpected error: {ex.Message}");
            }
        }
    }

    public async Task<Result<List<URLInformation>>> CrawlPremierLeagueAsync()
    {
        string correlationId = Guid.NewGuid().ToString();
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Operation"] = nameof(CrawlPremierLeagueAsync)
        }))
        {
            _logger.LogInformation("Starting Premier League crawl");

            var serverStatus = await IsServerAliveAsync();
            if (!serverStatus.Success)
            {
                _logger.LogError("Cannot proceed with crawling: {Message}", serverStatus.Message);
                return Result<List<URLInformation>>.Fail(serverStatus.Message ?? "Server is not accessible.");
            }

            var results = new List<URLInformation>();

            foreach (var tagged in PremierLeagueURLS.Urls)
            {
                foreach (var seasonUrl in tagged.SeasonUrls)
                {
                    var status = new URLInformation
                    {
                        Label = $"{tagged.Label} - {seasonUrl.Season}",
                        URL = seasonUrl.URL,
                        League = tagged.League,
                        Season = seasonUrl.Season
                    };

                    try
                    {
                        _logger.LogInformation("Processing {Label} [{LeagueName}] Season {Season}",
                            tagged.Label, tagged.League.LeagueName, seasonUrl.Season);

                        var response = await _httpClient.GetAsync(seasonUrl.URL);
                        status.StatusCode = (int)response.StatusCode;
                        status.Status = response.StatusCode.ToString();

                        _logger.LogInformation("Received response: {Status} ({StatusCode})", status.Status, status.StatusCode);

                        if (!response.IsSuccessStatusCode)
                        {
                            _logger.LogWarning("Failed to fetch data: {Status}", response.StatusCode);
                            results.Add(status);
                            continue;
                        }

                        var html = await response.Content.ReadAsStringAsync();
                        var tableIds = tagged.TableIds ?? new TeamTableIds();

                        // Extract and import players
                        var playersSelector = $"//div[@id='div_stats_standard_{tableIds.StandardStats}']//table";
                        var players = await _htmlParser.ExtractPlayersTableAsync(html, playersSelector);
                        if (players != null && players.Any())
                        {
                            // Gán season và numeric PlayerRefId cho mỗi player trước khi lưu
                            foreach (var player in players)
                            {
                                player.Season = seasonUrl.Season;
                                player.PlayerRefId = GenerateNumericPlayerRefId(player.PlayerName, tagged.Label);
                            }

                            var filePath = await _dataStorage.SavePlayersToJsonAsync(players, tagged.Label, tagged.League);
                            var jsonContent = await File.ReadAllTextAsync(filePath);

                            var importResult = await _importService.ImportFromJsonAsync(
                                jsonContent, "players", tagged.Label, tagged.League.LeagueName, tagged.League.Nation, seasonUrl.Season);

                            if (importResult.Success)
                            {
                                _logger.LogInformation("✅ Imported {Count} players for {Team} season {Season} to database",
                                    players.Count, tagged.Label, seasonUrl.Season);
                            }
                            else
                            {
                                _logger.LogWarning("⚠️ Failed to import players for {Team} season {Season}: {Message}",
                                    tagged.Label, seasonUrl.Season, importResult.Message);
                            }
                        }
                        else
                        {
                            _logger.LogWarning("❌ No players found for {Team} season {Season} with selector: {Selector}",
                                tagged.Label, seasonUrl.Season, playersSelector);
                        }

                        // Extract and import goalkeeping stats
                        var goalkeepingSelector = $"//div[@id='div_stats_keeper_{tableIds.Goalkeeping}']//table";
                        var goalkeepers = await _htmlParser.ExtractGoalkeepingTableAsync(html, goalkeepingSelector);
                        if (goalkeepers != null && goalkeepers.Any())
                        {
                            // Gán season cho mỗi goalkeeping stat
                            foreach (var goalkeeper in goalkeepers)
                            {
                                goalkeeper.Season = seasonUrl.Season;
                            }

                            var filePath = await _dataStorage.SaveGoalkeepersToJsonAsync(goalkeepers, tagged.Label, tagged.League);
                            var jsonContent = await File.ReadAllTextAsync(filePath);
                            var importResult = await _importService.ImportFromJsonAsync(
                                jsonContent, "goalkeeping", tagged.Label, tagged.League.LeagueName, tagged.League.Nation, seasonUrl.Season);

                            if (importResult.Success)
                            {
                                _logger.LogInformation("✅ Imported goalkeeping stats for {Team} season {Season} to database",
                                    tagged.Label, seasonUrl.Season);
                            }
                            else
                            {
                                _logger.LogWarning("⚠️ Failed to import goalkeeping stats for {Team} season {Season}: {Message}",
                                    tagged.Label, seasonUrl.Season, importResult.Message);
                            }
                        }
                        else
                        {
                            _logger.LogWarning("❌ No goalkeeping stats found for {Team} season {Season}",
                                tagged.Label, seasonUrl.Season);
                        }

                        // Extract and import shooting stats
                        var shootingSelector = $"//div[@id='div_stats_shooting_{tableIds.Shooting}']//table";
                        var shootings = await _htmlParser.ExtractShootingTableAsync(html, shootingSelector);
                        if (shootings != null && shootings.Any())
                        {
                            // Gán season cho mỗi shooting stat
                            foreach (var shooting in shootings)
                            {
                                shooting.Season = seasonUrl.Season;
                            }

                            var filePath = await _dataStorage.SaveShootingsToJsonAsync(shootings, tagged.Label, tagged.League);
                            var jsonContent = await File.ReadAllTextAsync(filePath);
                            var importResult = await _importService.ImportFromJsonAsync(
                                jsonContent, "shooting", tagged.Label, tagged.League.LeagueName, tagged.League.Nation, seasonUrl.Season);

                            if (importResult.Success)
                            {
                                _logger.LogInformation("✅ Imported shooting stats for {Team} season {Season} to database",
                                    tagged.Label, seasonUrl.Season);
                            }
                            else
                            {
                                _logger.LogWarning("⚠️ Failed to import shooting stats for {Team} season {Season}: {Message}",
                                    tagged.Label, seasonUrl.Season, importResult.Message);
                            }
                        }
                        else
                        {
                            _logger.LogWarning("❌ No shooting stats found for {Team} season {Season}",
                                tagged.Label, seasonUrl.Season);
                        }

                        results.Add(status);
                    }
                    catch (Exception ex)
                    {
                        status.StatusCode = 0;
                        status.Status = $"Error: {ex.Message}";
                        _logger.LogError(ex, "❌ Error processing {Label} season {Season}: {Message}",
                            tagged.Label, seasonUrl.Season, ex.Message);
                        results.Add(status);
                    }
                }
            }

            _logger.LogInformation("🎉 Completed Premier League crawl with {Count} results", results.Count);
            return Result<List<URLInformation>>.Ok(results);
        }
    }

    public async Task<Result<List<URLInformation>>> CrawlRomaniaLiga1Async()
    {
        string correlationId = Guid.NewGuid().ToString();
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Operation"] = nameof(CrawlRomaniaLiga1Async)
        }))
        {
            _logger.LogInformation("Starting Romania Liga 1 crawl");

            var serverStatus = await IsServerAliveAsync();
            if (!serverStatus.Success)
            {
                _logger.LogError("Cannot proceed with crawling: {Message}", serverStatus.Message);
                return Result<List<URLInformation>>.Fail(serverStatus.Message ?? "Server is not accessible.");
            }

            var results = new List<URLInformation>();

            foreach (var tagged in RomaniaLiga1URLS.Urls)
            {
                foreach (var seasonUrl in tagged.SeasonUrls)
                {
                    var status = new URLInformation
                    {
                        Label = $"{tagged.Label}",
                        URL = seasonUrl.URL,
                        League = tagged.League!,
                        Season = seasonUrl.Season
                    };

                    try
                    {
                        _logger.LogInformation("Processing team {Label} [League: {LeagueName}]", 
                            tagged.Label, tagged.League?.LeagueName ?? "Liga 1");

                        var response = await _httpClient.GetAsync(seasonUrl.URL);
                        status.StatusCode = (int)response.StatusCode;
                        status.Status = response.StatusCode.ToString();

                        _logger.LogInformation("Received response: {Status} ({StatusCode})", status.Status, status.StatusCode);

                        if (!response.IsSuccessStatusCode)
                        {
                            _logger.LogWarning("Failed to fetch team data: {Status}", response.StatusCode);
                            results.Add(status);
                            continue;
                        }

                        var html = await response.Content.ReadAsStringAsync();
                        var tableIds = tagged.TableIds ?? new TeamTableIds();

                        var playersSelector = $"//div[@id='div_stats_standard_{tableIds.StandardStats}']//table";
                        var players = await _htmlParser.ExtractPlayersTableAsync(html, playersSelector);
                        if (players != null && players.Any())
                        {
                            foreach (var player in players)
                            {
                                player.PlayerRefId = GenerateNumericPlayerRefId(player.PlayerName, tagged.Label);
                                _logger.LogDebug("Generated PlayerRefId for {PlayerName}: {PlayerRefId}", player.PlayerName, player.PlayerRefId);
                            }

                            var filePath = await _dataStorage.SavePlayersToJsonAsync(players, tagged.Label, tagged.League!);
                            var jsonContent = await File.ReadAllTextAsync(filePath);

                            _logger.LogDebug("Importing players with LeagueName: {LeagueName}, Nation: {Nation}", 
                                tagged.League?.LeagueName ?? "Liga 1", tagged.League?.Nation ?? "Romania");

                            var importResult = await _importService.ImportFromJsonAsync(
                                jsonContent, "players", tagged.Label, tagged.League?.LeagueName ?? "Liga 1", tagged.League?.Nation ?? "Romania", seasonUrl.Season);

                            if (importResult.Success)
                            {
                                _logger.LogInformation("✅ Imported {Count} players for {Team} to database", 
                                    players.Count, tagged.Label);
                            }
                            else
                            {
                                _logger.LogWarning("⚠️ Failed to import players for {Team}: {Message}", 
                                    tagged.Label, importResult.Message);
                            }
                        }
                        else
                        {
                            _logger.LogWarning("❌ No players found for {Team} with selector: {Selector}", 
                                tagged.Label, playersSelector);
                        }

                        results.Add(status);
                    }
                    catch (Exception ex)
                    {
                        status.StatusCode = 0;
                        status.Status = $"Error: {ex.Message}";
                        _logger.LogError(ex, "❌ Error processing team {Label}: {Message}", 
                            tagged.Label, ex.Message);
                        results.Add(status);
                    }
                }
            }

            foreach (var playerTag in RomaniaLiga1URLS.PlayerUrls)
    {
        foreach (var playerUrl in playerTag.PlayerUrls)
        {
            var clubName = playerTag.PlayerDetails?.Club?.Trim();
            if (string.IsNullOrEmpty(clubName))
            {
                _logger.LogWarning("Skipping player URL {Url} due to empty club name in playerTag.PlayerDetails", playerUrl.URL);
                results.Add(new URLInformation
                {
                    Label = "Unknown Player - Current",
                    URL = playerUrl.URL,
                    League = new League { LeagueName = StaticLeague.Liga1, Nation = StaticNation.Romania.ToString() },
                    Status = "Skipped: Empty club name",
                    StatusCode = 0
                });
                continue;
            }
            var status = new URLInformation
            {
                Label = $"{clubName} Player - Current",
                URL = playerUrl.URL,
                League = new League { LeagueName = StaticLeague.Liga1, Nation = StaticNation.Romania.ToString() }
            };
            try
            {
                _logger.LogInformation("Processing player URL {Url} for Club {Club}", playerUrl.URL, clubName);
                var response = await _httpClient.GetAsync(playerUrl.URL);
                status.StatusCode = (int)response.StatusCode;
                status.Status = response.StatusCode.ToString();
                _logger.LogInformation("Received response for player: {Status} ({StatusCode})", status.Status, status.StatusCode);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to fetch player data: {Status}", response.StatusCode);
                    results.Add(status);
                    continue;
                }
                var html = await response.Content.ReadAsStringAsync();
                var playerDetails = await _htmlParser.ExtractPlayerDetailsAsync(html, "//div[@id='info']", clubName);
                if (playerDetails == null || string.IsNullOrEmpty(playerDetails.FullName))
                {
                    _logger.LogWarning("❌ No player details or empty FullName for {Club} at URL {Url}", clubName, playerUrl.URL);
                    status.Status = "No player details or empty FullName";
                    results.Add(status);
                    continue;
                }
                playerDetails.Club = clubName;
                var filePath = await _dataStorage.SavePlayerDetailsToJsonAsync(playerDetails, playerDetails.FullName);
                var jsonContent = await File.ReadAllTextAsync(filePath);
                _logger.LogDebug("Generated JSON for player {PlayerName} (PlayerRefId: {PlayerRefId}): {JsonContent}",
                    playerDetails.FullName, playerDetails.PlayerRefId, jsonContent);
                // Dynamically select the most recent season for the club
                string season = "2025-2026"; // Fallback
                var team = RomaniaLiga1URLS.Urls.FirstOrDefault(t => t.Label.Equals(clubName, StringComparison.OrdinalIgnoreCase));
                if (team != null)
                {
                    // Sort SeasonUrls by season (assuming format "YYYY-YYYY") in descending order
                    var latestSeason = team.SeasonUrls
                        .OrderByDescending(s => s.Season)
                        .FirstOrDefault()?.Season ?? "2025-2026";
                    season = latestSeason;
                    _logger.LogInformation("Dynamically selected season {Season} for club {ClubName} in playerDetails import", season, clubName);
                }
                else
                {
                    _logger.LogWarning("Club {ClubName} not found in RomaniaLiga1URLS.Urls, using fallback season {Season}", clubName, season);
                }
                var importResult = await _importService.ImportFromJsonAsync(
                    jsonContent, "playerDetails", clubName, StaticLeague.Liga1, StaticNation.Romania.ToString(), season);
                if (importResult.Success)
                {
                    _logger.LogInformation("✅ Imported player details for {PlayerName} (PlayerRefId: {PlayerRefId}) to database with season {Season}",
                        playerDetails.FullName, playerDetails.PlayerRefId, season);
                }
                else
                {
                    _logger.LogWarning("⚠️ Failed to import player details for {PlayerName} (PlayerRefId: {PlayerRefId}): {Message}",
                        playerDetails.FullName, playerDetails.PlayerRefId, importResult.Message);
                }
                results.Add(status);
                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                status.StatusCode = 0;
                status.Status = $"Error: {ex.Message}";
                _logger.LogError(ex, "❌ Error processing player URL {Url} for Club {Club}: {Message}",
                    playerUrl.URL, clubName, ex.Message);
                results.Add(status);
            }
        }
    }
            _logger.LogInformation("🎉 Completed Romania Liga 1 crawl with {Count} results", results.Count);
            return Result<List<URLInformation>>.Ok(results);
        }
    }

    private string GenerateNumericPlayerRefId(string playerName, string clubName)
    {
        var input = $"{playerName}_{clubName}";
        using var md5 = System.Security.Cryptography.MD5.Create();
        var hashBytes = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
        var result = BitConverter.ToInt64(hashBytes, 0) & long.MaxValue;
        return result.ToString();
    }

}