using System.Text.Json;
using FSP.Domain.Entities;
using FSP.Domain.Entities.Core;
using FSP.Domain.Interfaces.Core;
using Microsoft.Extensions.Logging;

namespace FSP.Infrastructure.Implementations;

public class SimpleCrawlerService : ISimpleCrawlerService
{
    private readonly HttpClient _httpClient;
    private readonly IHtmlParserService _htmlParser;
    private readonly IDataStorageService _dataStorage;
    private readonly ILogger<SimpleCrawlerService> _logger;

    public SimpleCrawlerService(
        HttpClient httpClient,
        IHtmlParserService htmlParser,
        IDataStorageService dataStorage,
        ILogger<SimpleCrawlerService> logger)
    {
        this._httpClient = httpClient;
        this._htmlParser = htmlParser;
        this._dataStorage = dataStorage;
        this._logger = logger;

        this._httpClient.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
        this._httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task<Result<string>> GetRawHtmlAsync(string url)
    {
        try
        {
            _logger.LogInformation("Fetching HTML from: {Url}", url);

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch URL {Url}: {StatusCode}", url, response.StatusCode);
                return Result<string>.Fail($"HTTP error: {response.StatusCode}");
            }

            var html = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully fetched HTML from {Url} ({Length} characters)", url, html.Length);

            return Result<string>.Ok(html);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching HTML from {Url}: {Message}", url, ex.Message);
            return Result<string>.Fail($"Error fetching URL: {ex.Message}");
        }
    }

    public async Task<Result<List<Player>>> ExtractPlayersFromUrlAsync(string url, string selector)
    {
        try
        {
            var htmlResult = await GetRawHtmlAsync(url);
            if (!htmlResult.Success)
            {
                return Result<List<Player>>.Fail(htmlResult.Message);
            }

            var players = await _htmlParser.ExtractPlayersTableAsync(htmlResult.Data, selector);
            _logger.LogInformation("Extracted {Count} players from {Url}", players.Count, url);

            return Result<List<Player>>.Ok(players);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting players from {Url}: {Message}", url, ex.Message);
            return Result<List<Player>>.Fail($"Error extracting players: {ex.Message}");
        }
    }

    public async Task<Result<List<Goalkeeping>>> ExtractGoalkeepingFromUrlAsync(string url, string selector)
    {
        try
        {
            var htmlResult = await GetRawHtmlAsync(url);
            if (!htmlResult.Success)
            {
                return Result<List<Goalkeeping>>.Fail(htmlResult.Message);
            }

            var goalkeepers = await _htmlParser.ExtractGoalkeepingTableAsync(htmlResult.Data, selector);
            _logger.LogInformation("Extracted {Count} goalkeeping records from {Url}", goalkeepers.Count, url);

            return Result<List<Goalkeeping>>.Ok(goalkeepers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting goalkeeping data from {Url}: {Message}", url, ex.Message);
            return Result<List<Goalkeeping>>.Fail($"Error extracting goalkeeping data: {ex.Message}");
        }
    }

    public async Task<Result<List<Shooting>>> ExtractShootingFromUrlAsync(string url, string selector)
    {
        try
        {
            var htmlResult = await GetRawHtmlAsync(url);
            if (!htmlResult.Success)
            {
                return Result<List<Shooting>>.Fail(htmlResult.Message);
            }

            var shootings = await _htmlParser.ExtractShootingTableAsync(htmlResult.Data, selector);
            _logger.LogInformation("Extracted {Count} shooting records from {Url}", shootings.Count, url);

            return Result<List<Shooting>>.Ok(shootings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting shooting data from {Url}: {Message}", url, ex.Message);
            return Result<List<Shooting>>.Fail($"Error extracting shooting data: {ex.Message}");
        }
    }

    public async Task<Result<List<MatchLog>>> ExtractMatchLogFromUrlAsync(string url, string selector)
    {
        try
        {
            var htmlResult = await GetRawHtmlAsync(url);
            if (!htmlResult.Success)
            {
                return Result<List<MatchLog>>.Fail(htmlResult.Message);
            }

            var matchLogs = await _htmlParser.ExtractMatchLogTableAsync(htmlResult.Data, selector);
            _logger.LogInformation("Extracted {Count} match logs from {Url}", matchLogs.Count, url);

            return Result<List<MatchLog>>.Ok(matchLogs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting match logs from {Url}: {Message}", url, ex.Message);
            return Result<List<MatchLog>>.Fail($"Error extracting match logs: {ex.Message}");
        }
    }

    public async Task<Result<PlayerDetails>> ExtractPlayerDetailsFromUrlAsync(string url, string selector, string clubName = null)
    {
        try
        {
            var htmlResult = await GetRawHtmlAsync(url);
            if (!htmlResult.Success)
            {
                return Result<PlayerDetails>.Fail(htmlResult.Message!);
            }

            var playerDetails = await _htmlParser.ExtractPlayerDetailsAsync(htmlResult.Data!, selector, clubName);
            if (playerDetails == null)
            {
                return Result<PlayerDetails>.Fail("Failed to extract player details from HTML");
            }

            _logger.LogInformation("Extracted player details for {PlayerName} from {Url}", playerDetails.FullName, url);
            return Result<PlayerDetails>.Ok(playerDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting player details from {Url}: {Message}", url, ex.Message);
            return Result<PlayerDetails>.Fail($"Error extracting player details: {ex.Message}");
        }
    }

    public async Task<Result<CompleteTeamData>> ExtractAllDataFromUrlAsync(string url, string id)
    {
        try
        {
            this._logger.LogInformation("Starting complete data extraction from: {Url} with ID: {Id}", url, id);

            var htmlResult = await GetRawHtmlAsync(url);
            if (!htmlResult.Success)
                return Result<CompleteTeamData>.Fail(htmlResult.Message!);

            string teamName = this.ExtractTeamNameFromUrl(url);

            CompleteTeamData completeTeamData = new CompleteTeamData
            {
                RawHtml = htmlResult.Data!,
                TeamName = teamName,
                TeamId = id,
                SourceUrl = url,
                ExtractedAt = DateTime.UtcNow
            };

            // var completeTeamData = new CompleteTeamData
            // {
            //     RawHtml = htmlResult.Data!,
            // };

            // Use the same ID for all selectors (as you said ID is same in all cases)
            var tasks = new List<Task>
            {
                Task.Run(async () =>
                {
                    string selector = $"//div[@id='div_stats_standard_{id}']//table";
                    var players = await _htmlParser.ExtractPlayersTableAsync(htmlResult.Data!, selector);
                    completeTeamData.Players = players;
                    _logger.LogInformation("Extracted {Count} players", players.Count);
                }),
                Task.Run(async () =>
                {
                    var selector = $"//div[@id='div_stats_keeper_{id}']//table";
                    var goalkeeping = await _htmlParser.ExtractGoalkeepingTableAsync(htmlResult.Data!, selector);
                    completeTeamData.Goalkeeping = goalkeeping;
                    _logger.LogInformation("Extracted {Count} goalkeeping records", goalkeeping.Count);
                }),
                Task.Run(async () =>
                {
                    var selector = $"//div[@id='div_stats_shooting_{id}']//table";
                    var shooting = await _htmlParser.ExtractShootingTableAsync(htmlResult.Data!, selector);
                    completeTeamData.Shooting = shooting;
                    _logger.LogInformation("Extracted {Count} shooting records", shooting.Count);
                }),
                Task.Run(async () =>
                {
                    var selector = "//table[contains(@id, 'matchlogs')]";
                    var matchLogs = await _htmlParser.ExtractMatchLogTableAsync(htmlResult.Data!, selector);
                    completeTeamData.MatchLogs = matchLogs;
                    _logger.LogInformation("Extracted {Count} match logs", matchLogs.Count);
                })
            };

            await Task.WhenAll(tasks);
            _logger.LogInformation("Completed all data extraction from {Url}", url);

            //await this.SaveCompleteTeamData(completeTeamData, teamName);

            return Result<CompleteTeamData>.Ok(completeTeamData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting all data from {Url}: {Message}", url, ex.Message);
            return Result<CompleteTeamData>.Fail($"Error extracting all data: {ex.Message}");
        }
    }

    #region helpers
    private readonly Dictionary<string, string> _defaultSelectors = new()
    {
        ["players"] = "//div[contains(@id, 'div_stats_standard')]//table",
        ["goalkeeping"] = "//div[contains(@id, 'div_stats_keeper')]//table",
        ["shooting"] = "//div[contains(@id, 'div_stats_shooting')]//table",
        ["matchLogs"] = "//table[contains(@id, 'matchlogs')]"
    };
    
    private async Task SaveCompleteTeamData(CompleteTeamData teamData, string teamName)
    {
        try
        {
            // Create a generic league for user-provided URLs
            var league = new League
            {
                LeagueName = "User Provided",
                Nation = "Unknown"
            };

            // Save each data type using your existing DataStorageService methods
            if (teamData.Players.Any())
            {
                await _dataStorage.SavePlayersToJsonAsync(teamData.Players, teamName, league);
            }

            if (teamData.Goalkeeping.Any())
            {
                await _dataStorage.SaveGoalkeepersToJsonAsync(teamData.Goalkeeping, teamName, league);
            }

            if (teamData.Shooting.Any())
            {
                await _dataStorage.SaveShootingsToJsonAsync(teamData.Shooting, teamName, league);
            }

            if (teamData.MatchLogs.Any())
            {
                await _dataStorage.SaveMatchLogsToJsonAsync(teamData.MatchLogs, teamName, league);
            }

            // Save the complete team data as a separate JSON file
            await SaveCompleteTeamDataToJson(teamData, teamName);

            _logger.LogInformation("✅ Saved all data for {TeamName} to JSON files", teamName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save complete team data for {TeamName}", teamName);
        }
    }

    private async Task SaveCompleteTeamDataToJson(CompleteTeamData teamData, string teamName)
    {
        try
        {
            var directoryPath = Path.Combine(_dataStorage.GetType().GetField("_baseDataPath", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(_dataStorage) as string ?? "./Data", 
                "CompleteTeams");
            
            Directory.CreateDirectory(directoryPath);

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileName = $"{SanitizeFileName(teamName)}_complete_{timestamp}.json";
            var fullPath = Path.Combine(directoryPath, fileName);

            var jsonData = new
            {
                Metadata = new
                {
                    TeamName = teamName,
                    TeamId = teamData.TeamId,
                    SourceUrl = teamData.SourceUrl,
                    DataType = "complete_team_data",
                    ExportDate = teamData.ExtractedAt,
                    PlayersCount = teamData.Players.Count,
                    GoalkeepingCount = teamData.Goalkeeping.Count,
                    ShootingCount = teamData.Shooting.Count,
                    MatchLogsCount = teamData.MatchLogs.Count
                },
                Data = teamData
            };

            var options = new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var jsonString = JsonSerializer.Serialize(jsonData, options);
            await File.WriteAllTextAsync(fullPath, jsonString);

            _logger.LogInformation("Saved complete team data for {TeamName} to {Path}", teamName, fullPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save complete team data JSON for {TeamName}", teamName);
        }
    }

    private string ExtractTeamNameFromUrl(string url)
    {
        try
        {
            var uri = new Uri(url);
            var segments = uri.AbsolutePath.Split('/');
            var lastSegment = segments.LastOrDefault();
            return lastSegment?.Replace("-Stats", "")?.Replace("-", " ") ?? "UnknownTeam";
        }
        catch
        {
            return "UnknownTeam";
        }
    }

    private string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries))
            .Trim()
            .Replace(" ", "_")
            .ToLower();
    }
    #endregion
}
