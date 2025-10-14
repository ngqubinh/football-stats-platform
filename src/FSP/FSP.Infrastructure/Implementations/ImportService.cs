using System;
using System.Globalization;
using System.Text;
using System.Text.Json;
using FSP.Domain.Entities;
using FSP.Domain.Entities.Core;
using FSP.Domain.Interfaces.Core;
using FSP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FSP.Infrastructure.Implementations;

public class ImportService : IImportService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<ImportService> _logger;

    public ImportService(ApplicationDbContext dbContext, ILogger<ImportService> logger)
    {
        this._dbContext = dbContext;
        this._logger = logger;
    }

public async Task<Result<bool>> ImportFromJsonAsync(string jsonContent, string dataType, string clubName, string leagueName, string nation, string season)
{
    string correlationId = Guid.NewGuid().ToString();
    using (this._logger.BeginScope(new Dictionary<string, object>
    {
        ["CorrelationId"] = correlationId,
        ["Operation"] = nameof(ImportFromJsonAsync)
    }))
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            this._logger.LogInformation("Starting import for {DataType} of club {ClubName} in season {Season}", dataType, clubName, season);
            string normalizedLeagueName = (leagueName ?? string.Empty).Trim();
            string normalizedNation = (nation ?? string.Empty).Trim();
            string normalizedClubName = (clubName ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(normalizedClubName))
            {
                this._logger.LogError("🚫 Import failed: Club name is empty for {DataType}", dataType);
                return Result<bool>.Fail("Club name cannot be empty.");
            }
            this._logger.LogInformation("Import parameters - DataType: {DataType}, Club: '{ClubName}', League: '{LeagueName}', Nation: '{Nation}', Season: '{Season}'",
                dataType, normalizedClubName, normalizedLeagueName, normalizedNation, season);
            var jsonDocument = JsonDocument.Parse(jsonContent);
            var dataArray = jsonDocument.RootElement.GetProperty("data");
            var league = await FindLeagueWithFallbackAsync(normalizedLeagueName, normalizedNation);
            if (league == null)
            {
                _logger.LogError("🚫 Import failed: League '{LeagueName}' in '{Nation}' not found in database",
                    normalizedLeagueName, normalizedNation);
                return Result<bool>.Fail($"League '{normalizedLeagueName}' not found in database.");
            }
            _logger.LogInformation("Using league: '{LeagueName}' (ID: {LeagueId}) for club '{ClubName}'",
                league.LeagueName, league.LeagueId, normalizedClubName);
            var club = await _dbContext.Clubs
                .FirstOrDefaultAsync(c =>
                    c.ClubName.ToLower() == normalizedClubName.ToLower()
                    && c.Nation.ToLower() == normalizedNation.ToLower());
            if (club != null)
            {
                _logger.LogInformation("Found existing club: '{ClubName}' (ID: {ClubId})", club.ClubName, club.ClubId);
                if (club.LeagueId != league.LeagueId)
                {
                    _logger.LogInformation("Updating club '{ClubName}' from LeagueId {OldLeagueId} to {NewLeagueId}",
                        club.ClubName, club.LeagueId, league.LeagueId);
                    club.LeagueId = league.LeagueId;
                    await _dbContext.SaveChangesAsync();
                }
            }
            else
            {
                _logger.LogInformation("Creating new club: '{ClubName}' with LeagueId: {LeagueId}", normalizedClubName, league.LeagueId);
                club = new Club
                {
                    ClubName = normalizedClubName,
                    Nation = normalizedNation,
                    LeagueId = league.LeagueId
                };
                _dbContext.Clubs.Add(club);
                await _dbContext.SaveChangesAsync();
            }
            int processedCount = 0;
            bool anySaved = false;
            foreach (var item in dataArray.EnumerateArray())
            {
                var playerName = item.TryGetProperty("playerName", out var nameElement)
                    ? nameElement.GetString() ?? string.Empty
                    : string.Empty;
                var fullName = item.TryGetProperty("FullName", out var fullNameElement)
                    ? fullNameElement.GetString() ?? string.Empty
                    : playerName;
                string jsonPlayerRefId = item.TryGetProperty("PlayerRefId", out var refIdElement)
                    ? refIdElement.GetString() ?? GenerateNumericPlayerRefId(playerName, club.ClubId.ToString())
                    : GenerateNumericPlayerRefId(playerName, club.ClubId.ToString());
                _logger.LogDebug("Processing item - PlayerName: '{PlayerName}', JSON PlayerRefId: '{PlayerRefId}', ClubId: {ClubId}, JSON: {JsonItem}",
                    playerName, jsonPlayerRefId, club.ClubId, item);
                if (dataType == "playerDetails" && string.IsNullOrEmpty(fullName))
                {
                    _logger.LogWarning("Skipping playerDetails item with empty FullName: {JsonItem}", item);
                    continue;
                }
                if (dataType == "playerDetails" && string.IsNullOrEmpty(jsonPlayerRefId))
                {
                    _logger.LogWarning("Skipping playerDetails item with invalid PlayerRefId: {JsonItem}", item);
                    continue;
                }
                if (dataType == "players")
                {
                    var validatedSeason = GetSeasonForClub(normalizedClubName, season);
                    if (validatedSeason != season)
                    {
                        _logger.LogWarning("Provided season '{ProvidedSeason}' not found for club '{ClubName}'. Using validated season '{ValidatedSeason}'",
                            season, normalizedClubName, validatedSeason);
                    }
                    await UpsertPlayerAsync(item, playerName, club.ClubId, validatedSeason);
                    processedCount++;
                    anySaved = true;
                }
                else if (dataType == "playerDetails")
                {
                    _logger.LogDebug("Processing PlayerDetails for FullName: '{FullName}', PlayerRefId: {PlayerRefId}, ClubId: {ClubId}",
                        fullName, jsonPlayerRefId, club.ClubId);
                    if (await UpsertPlayerDetailsAsync(item, fullName, jsonPlayerRefId, club.ClubId))
                    {
                        processedCount++;
                        anySaved = true;
                    }
                }
                else if (dataType == "goalkeeping" || dataType == "shooting")
                {
                    this._logger.LogDebug("Looking up player with PlayerName: '{PlayerName}', ClubId: {ClubId}, Season: {Season}", playerName, club.ClubId, season);
                    string normalizedPlayerName = RemoveDiacritics(playerName).ToLower();
                    var player = _dbContext.Players
                        .Where(p => p.ClubId == club.ClubId && p.Season == season)
                        .AsEnumerable()
                        .FirstOrDefault(p => RemoveDiacritics(p.PlayerName).ToLower().Contains(normalizedPlayerName));
                    if (player == null)
                    {
                        this._logger.LogWarning("Player '{PlayerName}' (normalized: '{NormalizedPlayerName}') not found for club '{ClubName}', skipping {DataType}. JSON item: {JsonItem}. Available players for ClubId {ClubId}: {Players}",
                            playerName, normalizedPlayerName, club.ClubName, dataType, item, club.ClubId,
                            string.Join(", ", _dbContext.Players
                                .Where(p => p.ClubId == club.ClubId && p.Season == season)
                                .AsEnumerable()
                                .Select(p => $"{p.PlayerName} (PlayerRefId: {p.PlayerRefId})")));
                        continue;
                    }
                    _logger.LogInformation("Found player '{PlayerName}' with PlayerRefId '{PlayerRefId}' using normalized PlayerName lookup for {DataType}", 
                        player.PlayerName, player.PlayerRefId, dataType);
                    string playerRefId = player.PlayerRefId; // Use PlayerRefId from Players table
                    if (dataType == "goalkeeping")
                    {
                        _logger.LogDebug("Processing Goalkeeping for PlayerName: '{PlayerName}', PlayerRefId: {PlayerRefId}", playerName, playerRefId);
                        await UpsertGoalkeepingAsync(item, playerName, playerRefId, player.PlayerId, season);
                        processedCount++;
                        anySaved = true;
                    }
                    else // dataType == "shooting"
                    {
                        _logger.LogDebug("Processing Shooting for PlayerName: '{PlayerName}', PlayerRefId: {PlayerRefId}", playerName, playerRefId);
                        await UpsertShootingAsync(item, playerName, playerRefId, player.PlayerId, season);
                        processedCount++;
                        anySaved = true;
                    }
                }
                else
                {
                    _logger.LogWarning("Unknown dataType '{DataType}' for PlayerName '{PlayerName}', JSON PlayerRefId '{PlayerRefId}', skipping item", 
                        dataType, playerName, jsonPlayerRefId);
                    continue;
                }
            }
            if (processedCount == 0 || !anySaved)
            {
                _logger.LogWarning("No items successfully saved for {DataType} import for club '{ClubName}'", dataType, club.ClubName);
                await transaction.RollbackAsync();
                return Result<bool>.Fail("No items were successfully saved during the import.");
            }
            _logger.LogInformation("Attempting to save changes for {DataType}, {Count} items processed", dataType, processedCount);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception saveEx)
            {
                _logger.LogError(saveEx, "Failed to save changes for {DataType}: {Message}, InnerException: {InnerMessage}",
                    dataType, saveEx.Message, saveEx.InnerException?.Message);
                throw;
            }
            await transaction.CommitAsync();
            _logger.LogInformation("Import completed successfully for {DataType}. Club '{ClubName}' → LeagueId: {LeagueId}, Processed {Count} items",
                dataType, club.ClubName, club.LeagueId, processedCount);
            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error importing {DataType}: {Message}, InnerException: {InnerMessage}",
                dataType, ex.Message, ex.InnerException?.Message);
            return Result<bool>.Fail($"Import failed: {ex.Message}");
        }
    }
}
    private async Task<bool> UpsertPlayerDetailsAsync(JsonElement item, string fullName, string playerRefId, int clubId)
    {
        _logger.LogDebug("Looking up player with PlayerRefId: {PlayerRefId}, ClubId: {ClubId}", playerRefId, clubId);

        var player = await _dbContext.Players
            .FirstOrDefaultAsync(p => p.PlayerRefId.Equals(playerRefId) && p.ClubId == clubId);

        if (player == null)
        {
            _logger.LogWarning("Player not found for PlayerDetails: FullName: '{FullName}', PlayerRefId: {PlayerRefId}, ClubId: {ClubId}. Available players for ClubId {ClubId}: {Players}", 
                fullName, playerRefId, clubId, clubId, 
                string.Join(", ", _dbContext.Players
                    .Where(p => p.ClubId == clubId)
                    .Select(p => $"{p.PlayerName} (PlayerRefId: {p.PlayerRefId})").ToList()));
            return false;
        }

        var existingPlayerDetails = await _dbContext.PlayerDetails
            .FirstOrDefaultAsync(pd => pd.PlayerId == player.PlayerId);

        PlayerDetails playerDetails = existingPlayerDetails!;

        if (existingPlayerDetails != null)
        {
            _logger.LogDebug("Updating existing player details for PlayerId: {PlayerId}, FullName: {FullName}, PlayerRefId: {PlayerRefId}", 
                player.PlayerId, fullName, playerRefId);
            existingPlayerDetails.FullName = fullName;
            existingPlayerDetails.OriginalName = item.GetProperty("OriginalName").GetString() ?? fullName;
            existingPlayerDetails.Position = item.GetProperty("Position").GetString() ?? string.Empty;
            existingPlayerDetails.Born = item.GetProperty("Born").GetString() ?? string.Empty;
            existingPlayerDetails.Citizenship = item.GetProperty("Citizenship").GetString() ?? string.Empty;
            existingPlayerDetails.Club = item.GetProperty("Club").GetString() ?? string.Empty;
            existingPlayerDetails.PlayerRefId = playerRefId;
        }
        else
        {
            _logger.LogDebug("Creating new player details for PlayerId: {PlayerId}, FullName: {FullName}, PlayerRefId: {PlayerRefId}", 
                player.PlayerId, fullName, playerRefId);
            playerDetails = new PlayerDetails
            {
                FullName = fullName,
                OriginalName = item.GetProperty("OriginalName").GetString() ?? fullName,
                Position = item.GetProperty("Position").GetString() ?? string.Empty,
                Born = item.GetProperty("Born").GetString() ?? string.Empty,
                Citizenship = item.GetProperty("Citizenship").GetString() ?? string.Empty,
                Club = item.GetProperty("Club").GetString() ?? string.Empty,
                PlayerRefId = playerRefId,
                PlayerId = player.PlayerId,
                Player = player
            };
            _dbContext.PlayerDetails.Add(playerDetails);
        }

        _logger.LogDebug("PlayerDetails entity state for {FullName}: {State}", fullName, _dbContext.Entry(playerDetails).State);
        return true;
    }

    private async Task UpsertPlayerAsync(JsonElement item, string playerName, int clubId, string season)
    {
        // DEBUG LOGGING
        _logger.LogInformation("=== DEBUG START ===");
        _logger.LogInformation("Incoming playerName parameter: {PlayerName}, Season: {Season}", playerName, season);
        // Log all properties in JSON item
        var allProperties = new List<string>();
        foreach (var property in item.EnumerateObject())
        {
            var value = property.Value.ValueKind == JsonValueKind.String
                ? property.Value.GetString()
                : property.Value.ValueKind.ToString();
            allProperties.Add($"{property.Name}: {value}");
            _logger.LogInformation("JSON Property: {PropertyName} = {Value}", property.Name, value);
        }
        _logger.LogInformation("All available properties: {Properties}", string.Join(", ", allProperties));
        // Check specific properties
        if (item.TryGetProperty("Nation", out var nationProp))
        {
            _logger.LogInformation("🔍 [NATION] Found 'Nation' property: {Value}", nationProp.GetString());
        }
        if (item.TryGetProperty("nation", out var nationLowerProp))
        {
            _logger.LogInformation("🔍 [NATION] Found 'nation' property: {Value}", nationLowerProp.GetString());
        }
        if (item.TryGetProperty("PlayerName", out var playerNameProp))
        {
            _logger.LogInformation("🔍 [PLAYERNAME] Found 'PlayerName' property: {Value}", playerNameProp.GetString());
        }
        if (item.TryGetProperty("playerName", out var playerNameLowerProp))
        {
            _logger.LogInformation("🔍 [PLAYERNAME] Found 'playerName' property: {Value}", playerNameLowerProp.GetString());
        }
        _logger.LogInformation("=== DEBUG END ===");

        string playerRefId = GetPropertyStringSafe(item, "PlayerRefId", "playerRefId");
        if (playerRefId == null)
        {
            playerRefId = GenerateNumericPlayerRefId(playerName, clubId.ToString());
        }

        var existingPlayer = await _dbContext.Players
            .FirstOrDefaultAsync(p => p.PlayerRefId.Equals(playerRefId) && p.ClubId == clubId && p.Season == season);

        if (existingPlayer != null)
        {
            _logger.LogDebug("Updating existing player: '{PlayerName}' (ID: {PlayerId}) for season {Season}",
                playerName, existingPlayer.PlayerId, season);
            // Update existing player
            existingPlayer.PlayerName = playerName;
            existingPlayer.PlayerRefId = playerRefId;
            existingPlayer.Season = season;
            existingPlayer.Nation = GetPropertyStringSafe(item, "Nation", "nation");
            existingPlayer.Position = GetPropertyStringSafe(item, "Position", "position");
            existingPlayer.Age = GetPropertyStringSafe(item, "Age", "age");
            existingPlayer.MatchPlayed = GetPropertyIntSafe(item, "MatchPlayed", "matchPlayed");
            existingPlayer.Starts = GetPropertyIntSafe(item, "Starts", "starts");
            existingPlayer.Minutes = GetPropertyIntSafe(item, "Minutes", "minutes");
            existingPlayer.NineteenMinutes = GetPropertyStringSafe(item, "NineteenMinutes", "nineteenMinutes");
            existingPlayer.Goals = GetPropertyIntSafe(item, "Goals", "goals");
            existingPlayer.Assists = GetPropertyIntSafe(item, "Assists", "assists");
            existingPlayer.GoalsAssists = GetPropertyIntSafe(item, "GoalsAssists", "goalsAssists");
            existingPlayer.NonPenaltyGoals = GetPropertyIntSafe(item, "NonPenaltyGoals", "nonPenaltyGoals");
            existingPlayer.PenaltyKicksMade = GetPropertyIntSafe(item, "PenaltyKicksMade", "penaltyKicksMade");
            existingPlayer.PenaltyKickAttempted = GetPropertyIntSafe(item, "PenaltyKickAttempted", "penaltyKickAttempted");
            existingPlayer.YellowCards = GetPropertyIntSafe(item, "YellowCards", "yellowCards");
            existingPlayer.RedCards = GetPropertyIntSafe(item, "RedCards", "redCards");
            existingPlayer.GoalsPer90s = GetPropertyStringSafe(item, "GoalsPer90s", "goalsPer90s");
            existingPlayer.AssistsPer90s = GetPropertyStringSafe(item, "AssistsPer90s", "assistsPer90s");
            existingPlayer.GoalsAssistsPer90s = GetPropertyStringSafe(item, "GoalsAssistsPer90s", "goalsAssistsPer90s");
            existingPlayer.NonPenaltyGoalsPer90s = GetPropertyStringSafe(item, "NonPenaltyGoalsPer90s", "nonPenaltyGoalsPer90s");
            existingPlayer.NonPenaltyGoalsAssistsPer90s = GetPropertyStringSafe(item, "NonPenaltyGoalsAssistsPer90s", "nonPenaltyGoalsAssistsPer90s");
            _logger.LogInformation("✅ AFTER UPDATE - PlayerName: {PlayerName}, Nation: {Nation}, Season: {Season}",
                existingPlayer.PlayerName, existingPlayer.Nation, existingPlayer.Season);
        }
        else
        {
            _logger.LogDebug("Creating new player: '{PlayerName}'", playerName);
            var nationValue = GetPropertyStringSafe(item, "Nation", "nation");
            _logger.LogInformation("🎯 CREATING NEW - PlayerName: {PlayerName}, Nation: {Nation}, Season: {Season}",
                playerName, nationValue, season);
            var player = new Player
            {
                PlayerName = playerName,
                PlayerRefId = playerRefId,
                Season = season, // Use the passed season
                Nation = nationValue,
                Position = GetPropertyStringSafe(item, "Position", "position"),
                Age = GetPropertyStringSafe(item, "Age", "age"),
                MatchPlayed = GetPropertyIntSafe(item, "MatchPlayed", "matchPlayed"),
                Starts = GetPropertyIntSafe(item, "Starts", "starts"),
                Minutes = GetPropertyIntSafe(item, "Minutes", "minutes"),
                NineteenMinutes = GetPropertyStringSafe(item, "NineteenMinutes", "nineteenMinutes"),
                Goals = GetPropertyIntSafe(item, "Goals", "goals"),
                Assists = GetPropertyIntSafe(item, "Assists", "assists"),
                GoalsAssists = GetPropertyIntSafe(item, "GoalsAssists", "goalsAssists"),
                NonPenaltyGoals = GetPropertyIntSafe(item, "NonPenaltyGoals", "nonPenaltyGoals"),
                PenaltyKicksMade = GetPropertyIntSafe(item, "PenaltyKicksMade", "penaltyKicksMade"),
                PenaltyKickAttempted = GetPropertyIntSafe(item, "PenaltyKickAttempted", "penaltyKickAttempted"),
                YellowCards = GetPropertyIntSafe(item, "YellowCards", "yellowCards"),
                RedCards = GetPropertyIntSafe(item, "RedCards", "redCards"),
                GoalsPer90s = GetPropertyStringSafe(item, "GoalsPer90s", "goalsPer90s"),
                AssistsPer90s = GetPropertyStringSafe(item, "AssistsPer90s", "assistsPer90s"),
                GoalsAssistsPer90s = GetPropertyStringSafe(item, "GoalsAssistsPer90s", "goalsAssistsPer90s"),
                NonPenaltyGoalsPer90s = GetPropertyStringSafe(item, "NonPenaltyGoalsPer90s", "nonPenaltyGoalsPer90s"),
                NonPenaltyGoalsAssistsPer90s = GetPropertyStringSafe(item, "NonPenaltyGoalsAssistsPer90s", "nonPenaltyGoalsAssistsPer90s"),
                ClubId = clubId
            };
            _dbContext.Players.Add(player);
            _logger.LogInformation("✅ NEW PLAYER CREATED - PlayerName: {PlayerName}, Nation: {Nation}, Season: {Season}",
                player.PlayerName, player.Nation, player.Season);
        }
    }

    private string GetSeasonForClub(string clubName, string targetSeason = null!)
    {
        _logger.LogInformation("🔍 Looking for club '{ClubName}' with target season '{TargetSeason}'", clubName, targetSeason);

        // Look in Premier League teams
        foreach (var team in PremierLeagueURLS.Urls)
        {
            if (team.Label.Equals(clubName, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Found club '{ClubName}' in Premier League. Available seasons: {Seasons}", 
                    clubName, string.Join(", ", team.SeasonUrls.Select(s => s.Season)));

                // If targetSeason is provided, prioritize it
                if (!string.IsNullOrEmpty(targetSeason))
                {
                    var matchingSeasonUrl = team.SeasonUrls.FirstOrDefault(s => s.Season.Equals(targetSeason, StringComparison.OrdinalIgnoreCase));
                    if (matchingSeasonUrl != null)
                    {
                        _logger.LogInformation("✅ Target season '{TargetSeason}' found for '{ClubName}' with URL: {Url}", 
                            targetSeason, clubName, matchingSeasonUrl.URL);
                        return targetSeason;
                    }
                    else
                    {
                        _logger.LogWarning("❌ Target season '{TargetSeason}' not found for '{ClubName}'. Falling back to first available season.", 
                            targetSeason, clubName);
                    }
                }

                // Fall back to first available season
                var fallbackSeason = team.SeasonUrls.FirstOrDefault()?.Season;
                _logger.LogInformation("Using fallback season '{FallbackSeason}' for '{ClubName}'", fallbackSeason, clubName);
                return fallbackSeason ?? "2025-2026";
            }
        }

        // Look in Romania Liga1 teams (similar logic)
        foreach (var team in RomaniaLiga1URLS.Urls)
        {
            if (team.Label.Equals(clubName, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Found club '{ClubName}' in Romania Liga1. Available seasons: {Seasons}", 
                    clubName, string.Join(", ", team.SeasonUrls.Select(s => s.Season)));

                if (!string.IsNullOrEmpty(targetSeason))
                {
                    var matchingSeasonUrl = team.SeasonUrls.FirstOrDefault(s => s.Season.Equals(targetSeason, StringComparison.OrdinalIgnoreCase));
                    if (matchingSeasonUrl != null)
                    {
                        _logger.LogInformation("✅ Target season '{TargetSeason}' found for '{ClubName}' with URL: {Url}", 
                            targetSeason, clubName, matchingSeasonUrl.URL);
                        return targetSeason;
                    }
                    else
                    {
                        _logger.LogWarning("❌ Target season '{TargetSeason}' not found for '{ClubName}'. Falling back to first available season.", 
                            targetSeason, clubName);
                    }
                }

                var fallbackSeason = team.SeasonUrls.FirstOrDefault()?.Season;
                _logger.LogInformation("Using fallback season '{FallbackSeason}' for '{ClubName}'", fallbackSeason, clubName);
                return fallbackSeason ?? "2025-2026";
            }
        }

        _logger.LogWarning("Club '{ClubName}' not found in any configuration. Using default season '{DefaultSeason}'", clubName, "2025-2026");
        return "2025-2026";
    }
    
    private string GetPropertyStringSafe(JsonElement element, string primaryName, string fallbackName)
    {
        // THÊM DEBUG ĐỂ XEM CÓ TÌM THẤY PROPERTY KHÔNG
        _logger.LogDebug("🔍 [GetPropertyStringSafe] Looking for: '{PrimaryName}' or '{FallbackName}'", primaryName, fallbackName);

        // ƯU TIÊN TÌM camelCase TRƯỚC (vì JSON của bạn dùng camelCase)
        if (element.TryGetProperty(fallbackName, out var fallbackProp) && fallbackProp.ValueKind != JsonValueKind.Null)
        {
            var value = fallbackProp.GetString();
            _logger.LogDebug("✅ Found fallback property '{PropertyName}': {Value}", fallbackName, value);
            return value ?? string.Empty;
        }

        // SAU ĐÓ TÌM PascalCase
        if (element.TryGetProperty(primaryName, out var prop) && prop.ValueKind != JsonValueKind.Null)
        {
            var value = prop.GetString();
            _logger.LogDebug("✅ Found primary property '{PropertyName}': {Value}", primaryName, value);
            return value ?? string.Empty;
        }

        _logger.LogWarning("❌ Property '{PrimaryName}' or '{FallbackName}' not found", primaryName, fallbackName);

        // DEBUG: LOG TẤT CẢ PROPERTIES CÓ SẴN
        var availableProps = element.EnumerateObject().Select(p => p.Name).ToList();
        _logger.LogWarning("📋 Available properties: {Properties}", string.Join(", ", availableProps));

        return string.Empty;
    }

    private int GetPropertyIntSafe(JsonElement element, string primaryName, string fallbackName)
    {
        if (element.TryGetProperty(primaryName, out var prop) && prop.ValueKind != JsonValueKind.Null)
        {
            return prop.ValueKind == JsonValueKind.Number ? prop.GetInt32() : 0;
        }
        
        if (element.TryGetProperty(fallbackName, out var fallbackProp) && fallbackProp.ValueKind != JsonValueKind.Null)
        {
            return fallbackProp.ValueKind == JsonValueKind.Number ? fallbackProp.GetInt32() : 0;
        }
        
        return 0;
    }

    private long GetPropertyLongSafe(JsonElement element, string primaryName, string fallbackName)
    {
        if (element.TryGetProperty(primaryName, out var prop) && prop.ValueKind != JsonValueKind.Null)
        {
            return prop.ValueKind == JsonValueKind.Number ? prop.GetInt64() : 0;
        }
        
        if (element.TryGetProperty(fallbackName, out var fallbackProp) && fallbackProp.ValueKind != JsonValueKind.Null)
        {
            return fallbackProp.ValueKind == JsonValueKind.Number ? fallbackProp.GetInt64() : 0;
        }
        
        return 0;
    }
private async Task UpsertGoalkeepingAsync(JsonElement item, string playerName, string playerRefId, int playerId, string season)
{
    _logger.LogDebug("JSON properties for Goalkeeping: {Properties}", 
        string.Join(", ", item.EnumerateObject().Select(p => $"{p.Name}: {p.Value.ValueKind}")));
    var existingGoalkeeping = await _dbContext.Goalkeepings
        .FirstOrDefaultAsync(g => g.PlayerRefId == playerRefId && g.Season == season);

    if (existingGoalkeeping != null)
    {
        _logger.LogDebug("Updating existing goalkeeping for PlayerRefId: {PlayerRefId}, Season: {Season}", playerRefId, season);
        existingGoalkeeping.PlayerName = playerName;
        existingGoalkeeping.Nation = GetPropertyStringSafe(item, "Nation", "nation");
        existingGoalkeeping.Position = GetPropertyStringSafe(item, "Position", "position");
        existingGoalkeeping.Age = GetPropertyStringSafe(item, "Age", "age");
        existingGoalkeeping.MatchPlayed = GetPropertyIntSafe(item, "MatchPlayed", "matchPlayed");
        existingGoalkeeping.Starts = GetPropertyIntSafe(item, "Starts", "starts");
        existingGoalkeeping.Minutes = GetPropertyStringSafe(item, "Minutes", "minutes");
        existingGoalkeeping.NineteenMinutes = GetPropertyStringSafe(item, "NineteenMinutes", "nineteenMinutes");
        existingGoalkeeping.GoalsAgainst = GetPropertyIntSafe(item, "GoalsAgainst", "goalsAgainst");
        existingGoalkeeping.GoalsAssistsPer90s = GetPropertyStringSafe(item, "GoalsAssistsPer90s", "goalsAssistsPer90s");
        existingGoalkeeping.ShotsOnTargetAgainst = GetPropertyStringSafe(item, "ShotsOnTargetAgainst", "shotsOnTargetAgainst");
        existingGoalkeeping.Saves = GetPropertyStringSafe(item, "Saves", "saves");
        existingGoalkeeping.SavePercentage = GetPropertyStringSafe(item, "SavePercentage", "savePercentage");
        existingGoalkeeping.Wins = GetPropertyIntSafe(item, "Wins", "wins");
        existingGoalkeeping.Draws = GetPropertyIntSafe(item, "Draws", "draws");
        existingGoalkeeping.Losses = GetPropertyIntSafe(item, "Losses", "losses");
        existingGoalkeeping.CleanSheets = GetPropertyIntSafe(item, "CleanSheets", "cleanSheets");
        existingGoalkeeping.CleanSheetsPercentage = GetPropertyStringSafe(item, "CleanSheetsPercentage", "cleanSheetsPercentage");
        existingGoalkeeping.PenaltyKicksAttempted = GetPropertyStringSafe(item, "PenaltyKicksAttempted", "penaltyKicksAttempted");
        existingGoalkeeping.PenaltyKicksAllowed = GetPropertyStringSafe(item, "PenaltyKicksAllowed", "penaltyKicksAllowed");
        existingGoalkeeping.PenaltyKicksSaved = GetPropertyStringSafe(item, "PenaltyKicksSaved", "penaltyKicksSaved");
        existingGoalkeeping.PenaltyKicksMissed = GetPropertyStringSafe(item, "PenaltyKicksMissed", "penaltyKicksMissed");
        existingGoalkeeping.PenaltyKicksSavedPercentage = GetPropertyStringSafe(item, "PenaltyKicksSavedPercentage", "penaltyKicksSavedPercentage");
        existingGoalkeeping.PlayerRefId = playerRefId;
        existingGoalkeeping.PlayerId = playerId;
        existingGoalkeeping.Season = season;
    }
    else
    {
        _logger.LogDebug("Creating new goalkeeping for PlayerRefId: {PlayerRefId}, Season: {Season}", playerRefId, season);
        var goalkeeping = new Goalkeeping
        {
            PlayerName = playerName,
            Nation = GetPropertyStringSafe(item, "Nation", "nation"),
            Position = GetPropertyStringSafe(item, "Position", "position"),
            Age = GetPropertyStringSafe(item, "Age", "age"),
            MatchPlayed = GetPropertyIntSafe(item, "MatchPlayed", "matchPlayed"),
            Starts = GetPropertyIntSafe(item, "Starts", "starts"),
            Minutes = GetPropertyStringSafe(item, "Minutes", "minutes"),
            NineteenMinutes = GetPropertyStringSafe(item, "NineteenMinutes", "nineteenMinutes"),
            GoalsAgainst = GetPropertyIntSafe(item, "GoalsAgainst", "goalsAgainst"),
            GoalsAssistsPer90s = GetPropertyStringSafe(item, "GoalsAssistsPer90s", "goalsAssistsPer90s"),
            ShotsOnTargetAgainst = GetPropertyStringSafe(item, "ShotsOnTargetAgainst", "shotsOnTargetAgainst"),
            Saves = GetPropertyStringSafe(item, "Saves", "saves"),
            SavePercentage = GetPropertyStringSafe(item, "SavePercentage", "savePercentage"),
            Wins = GetPropertyIntSafe(item, "Wins", "wins"),
            Draws = GetPropertyIntSafe(item, "Draws", "draws"),
            Losses = GetPropertyIntSafe(item, "Losses", "losses"),
            CleanSheets = GetPropertyIntSafe(item, "CleanSheets", "cleanSheets"),
            CleanSheetsPercentage = GetPropertyStringSafe(item, "CleanSheetsPercentage", "cleanSheetsPercentage"),
            PenaltyKicksAttempted = GetPropertyStringSafe(item, "PenaltyKicksAttempted", "penaltyKicksAttempted"),
            PenaltyKicksAllowed = GetPropertyStringSafe(item, "PenaltyKicksAllowed", "penaltyKicksAllowed"),
            PenaltyKicksSaved = GetPropertyStringSafe(item, "PenaltyKicksSaved", "penaltyKicksSaved"),
            PenaltyKicksMissed = GetPropertyStringSafe(item, "PenaltyKicksMissed", "penaltyKicksMissed"),
            PenaltyKicksSavedPercentage = GetPropertyStringSafe(item, "PenaltyKicksSavedPercentage", "penaltyKicksSavedPercentage"),
            PlayerRefId = playerRefId,
            PlayerId = playerId,
            Season = season
        };
        _dbContext.Goalkeepings.Add(goalkeeping);
    }
}

private async Task UpsertShootingAsync(JsonElement item, string playerName, string playerRefId, int playerId, string season)
{
    _logger.LogDebug("JSON properties for Shooting: {Properties}", 
        string.Join(", ", item.EnumerateObject().Select(p => $"{p.Name}: {p.Value.ValueKind}")));
    var existingShooting = await _dbContext.Shootings
        .FirstOrDefaultAsync(s => s.PlayerRefId == playerRefId && s.Season == season);

    if (existingShooting != null)
    {
        _logger.LogDebug("Updating existing shooting for PlayerRefId: {PlayerRefId}, Season: {Season}", playerRefId, season);
        existingShooting.PlayerName = playerName;
        existingShooting.Nation = GetPropertyStringSafe(item, "Nation", "nation");
        existingShooting.Position = GetPropertyStringSafe(item, "Position", "position");
        existingShooting.Age = GetPropertyStringSafe(item, "Age", "age");
        existingShooting.NineteenMinutes = GetPropertyStringSafe(item, "NineteenMinutes", "nineteenMinutes");
        existingShooting.Goals = GetPropertyIntSafe(item, "Goals", "goals");
        existingShooting.ShotsTotal = GetPropertyIntSafe(item, "ShotsTotal", "shotsTotal");
        existingShooting.ShotsOnTarget = GetPropertyIntSafe(item, "ShotsOnTarget", "shotsOnTarget");
        existingShooting.ShotsOnTargetPercentage = GetPropertyStringSafe(item, "ShotsOnTargetPercentage", "shotsOnTargetPercentage");
        existingShooting.ShotsTotalPer90 = GetPropertyStringSafe(item, "ShotsTotalPer90", "shotsTotalPer90");
        existingShooting.ShotsOnTargetPer90 = GetPropertyStringSafe(item, "ShotsOnTargetPer90", "shotsOnTargetPer90");
        existingShooting.GoalsShots = GetPropertyStringSafe(item, "GoalsShots", "goalsShots");
        existingShooting.GoalsShotsOnTarget = GetPropertyStringSafe(item, "GoalsShotsOnTarget", "goalsShotsOnTarget");
        existingShooting.AverageShotDistance = GetPropertyStringSafe(item, "AverageShotDistance", "averageShotDistance");
        existingShooting.PenaltyKicksMade = GetPropertyIntSafe(item, "PenaltyKicksMade", "penaltyKicksMade");
        existingShooting.PenaltyKicksAttempted = GetPropertyIntSafe(item, "PenaltyKicksAttempted", "penaltyKicksAttempted");
        existingShooting.PlayerRefId = playerRefId;
        existingShooting.PlayerId = playerId;
        existingShooting.Season = season;
    }
    else
    {
        _logger.LogDebug("Creating new shooting for PlayerRefId: {PlayerRefId}, Season: {Season}", playerRefId, season);
        var shooting = new Shooting
        {
            PlayerName = playerName,
            Nation = GetPropertyStringSafe(item, "Nation", "nation"),
            Position = GetPropertyStringSafe(item, "Position", "position"),
            Age = GetPropertyStringSafe(item, "Age", "age"),
            NineteenMinutes = GetPropertyStringSafe(item, "NineteenMinutes", "nineteenMinutes"),
            Goals = GetPropertyIntSafe(item, "Goals", "goals"),
            ShotsTotal = GetPropertyIntSafe(item, "ShotsTotal", "shotsTotal"),
            ShotsOnTarget = GetPropertyIntSafe(item, "ShotsOnTarget", "shotsOnTarget"),
            ShotsOnTargetPercentage = GetPropertyStringSafe(item, "ShotsOnTargetPercentage", "shotsOnTargetPercentage"),
            ShotsTotalPer90 = GetPropertyStringSafe(item, "ShotsTotalPer90", "shotsTotalPer90"),
            ShotsOnTargetPer90 = GetPropertyStringSafe(item, "ShotsOnTargetPer90", "shotsOnTargetPer90"),
            GoalsShots = GetPropertyStringSafe(item, "GoalsShots", "goalsShots"),
            GoalsShotsOnTarget = GetPropertyStringSafe(item, "GoalsShotsOnTarget", "goalsShotsOnTarget"),
            AverageShotDistance = GetPropertyStringSafe(item, "AverageShotDistance", "averageShotDistance"),
            PenaltyKicksMade = GetPropertyIntSafe(item, "PenaltyKicksMade", "penaltyKicksMade"),
            PenaltyKicksAttempted = GetPropertyIntSafe(item, "PenaltyKicksAttempted", "penaltyKicksAttempted"),
            PlayerRefId = playerRefId,
            PlayerId = playerId,
            Season = season
        };
        _dbContext.Shootings.Add(shooting);
    }
}

    // Helper method to remove diacritics for name comparison
    private static string RemoveDiacritics(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    private async Task<League?> FindLeagueWithFallbackAsync(string leagueName, string nation)
    {
        string normalizedLeagueName = (leagueName ?? string.Empty).Trim();
        string normalizedNation = (nation ?? string.Empty).Trim();

        _logger.LogInformation("🎯 [FIND_LEAGUE] Searching for: '{LeagueName}' in '{Nation}'", normalizedLeagueName, normalizedNation);

        if (string.IsNullOrEmpty(normalizedLeagueName))
        {
            _logger.LogError("❌ [FIND_LEAGUE] Invalid input: leagueName is empty");
            return await GetDefaultLeagueAsync();
        }

        var league = await _dbContext.Leagues
            .FirstOrDefaultAsync(l =>
                l.LeagueName.ToLower() == normalizedLeagueName.ToLower()
                && l.Nation.ToLower() == normalizedNation.ToLower());

        if (league != null)
        {
            _logger.LogInformation("✅ [FALLBACK 1] FOUND exact match: '{LeagueName}' (ID: {LeagueId})",
                league.LeagueName, league.LeagueId);
            return league;
        }

        if (StaticLeague.IsSystemLeagueName(normalizedLeagueName))
        {
            league = await _dbContext.Leagues
                .FirstOrDefaultAsync(l => l.LeagueName.ToLower() == normalizedLeagueName.ToLower());

            if (league != null)
            {
                _logger.LogInformation("✅ [FALLBACK 2] FOUND system league by name: '{LeagueName}' (ID: {LeagueId})",
                    league.LeagueName, league.LeagueId);
                return league;
            }

            _logger.LogError("❌ [FALLBACK 3] System league '{LeagueName}' not found in database. Using default league.",
                normalizedLeagueName);
            return await GetDefaultLeagueAsync();
        }

        _logger.LogWarning("🔄 [FALLBACK 4] Creating new league: '{LeagueName}' in '{Nation}'", normalizedLeagueName, normalizedNation);

        var existingLeague = await _dbContext.Leagues
            .FirstOrDefaultAsync(l =>
                l.LeagueName.ToLower() == normalizedLeagueName.ToLower()
                && l.Nation.ToLower() == normalizedNation.ToLower());

        if (existingLeague != null)
        {
            _logger.LogInformation("✅ [FALLBACK 4] Found existing league after re-check: '{LeagueName}' (ID: {LeagueId})",
                existingLeague.LeagueName, existingLeague.LeagueId);
            return existingLeague;
        }

        try
        {
            league = new League
            {
                LeagueName = normalizedLeagueName,
                Nation = normalizedNation
            };

            _dbContext.Leagues.Add(league);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("✅ [FALLBACK 4] Created new league: '{LeagueName}' (ID: {LeagueId})",
                league.LeagueName, league.LeagueId);
            return league;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [FALLBACK 4] Failed to create new league '{LeagueName}': {Message}",
                normalizedLeagueName, ex.Message);
            return await GetDefaultLeagueAsync();
        }
    }

    private async Task<League> GetDefaultLeagueAsync()
    {
        var premierLeague = await _dbContext.Leagues
            .FirstOrDefaultAsync(l => l.LeagueName.ToLower() == StaticLeague.PremierLeague.ToLower());

        if (premierLeague != null)
        {
            _logger.LogInformation("🏠 [DEFAULT] Using Premier League: '{LeagueName}' (ID: {LeagueId})",
                premierLeague.LeagueName, premierLeague.LeagueId);
            return premierLeague;
        }

        _logger.LogWarning("🏠 [DEFAULT] Premier League not found, creating it...");
        premierLeague = new League
        {
            LeagueName = StaticLeague.PremierLeague,
            Nation = StaticNation.England.ToString()
        };
        _dbContext.Leagues.Add(premierLeague);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("🏠 [DEFAULT] Created Premier League: '{LeagueName}' (ID: {LeagueId})",
            premierLeague.LeagueName, premierLeague.LeagueId);
        return premierLeague;
    }

    private string GenerateNumericPlayerRefId(string playerName, string clubId)
    {
        var input = $"{playerName}_{clubId}";
        using var md5 = System.Security.Cryptography.MD5.Create();
        var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
        var result = BitConverter.ToInt64(hashBytes, 0) & long.MaxValue;
        return result.ToString();
    }
}
