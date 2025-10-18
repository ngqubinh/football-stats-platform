using System.Text.Json;
using FSP.Domain.Entities.Core;
using FSP.Domain.Interfaces.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FSP.Infrastructure.Implementations;

public class DataStorageService : IDataStorageService
{
    private readonly ILogger<DataStorageService> _logger;
    private readonly string _baseDataPath;

    public DataStorageService(ILogger<DataStorageService> logger, IConfiguration configuration)
    {
        this._logger = logger;
        this._baseDataPath = configuration["DataStorage:BasePath"] ?? "./Data";
    }

    private async Task<string> SaveToJsonAsync<T>(List<T> data, string label, League league, string dataType)
    {
        try
        {
            // Create safe directory and file names
            string safeLeagueName = SanitizeFileName(league.LeagueName);
            string safeLabel = SanitizeFileName(label);
            string safeNation = SanitizeFileName(league.Nation);

            // Create directory structure: Data/Nation/LeagueName/
            string directoryPath = Path.Combine(_baseDataPath, safeNation, safeLeagueName);
            Directory.CreateDirectory(directoryPath);

            // Create filename: ClubName_DataType_Timestamp.json
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"{safeLabel}_{dataType}_{timestamp}.json";
            string fullPath = Path.Combine(directoryPath, fileName);

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var jsonData = new
            {
                Metadata = new
                {
                    Club = label,
                    League = league.LeagueName,
                    Nation = league.Nation,
                    DataType = dataType,
                    ExportDate = DateTime.UtcNow,
                    RecordCount = data.Count
                },
                Data = data
            };

            string jsonString = JsonSerializer.Serialize(jsonData, options);
            await File.WriteAllTextAsync(fullPath, jsonString);

            this._logger.LogInformation("Saved {Count} {DataType} records for {Label} to {Path}",
                data.Count, dataType, label, fullPath);

            return fullPath;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Failed to save {DataType} data for {Label}", dataType, label);
            throw;
        }
    }

    public async Task<string> SavePlayersToJsonAsync(List<Player> players, string label, League league)
    {
        return await SaveToJsonAsync(players, label, league, "players");
    }

    public async Task<string> SaveGoalkeepersToJsonAsync(List<Goalkeeping> goalkeepers, string label, League league)
    {
        return await SaveToJsonAsync(goalkeepers, label, league, "goalkeeping");
    }

    public async Task<string> SaveShootingsToJsonAsync(List<Shooting> shootings, string label, League league)
    {
        return await SaveToJsonAsync(shootings, label, league, "shooting");
    }

    public async Task SaveMatchLogsToJsonAsync(List<MatchLog> matchLogs, string label, League league)
    {
        await SaveToJsonAsync(matchLogs, label, league, "matchlogs");
    }

    public async Task<string> SavePlayerDetailsToJsonAsync(PlayerDetails playerDetails, string playerName)
    {
        var fileName = $"{playerName.Replace(" ", "_")}_playerDetails.json";
        var filePath = Path.Combine("Data", fileName);
        var jsonContent = JsonSerializer.Serialize(new { data = new[] { playerDetails } }, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, jsonContent);
        return filePath;
    }

    #region helpers
    private string SanitizeFileName(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return "unknown";

        var invalidChars = Path.GetInvalidFileNameChars();
        return string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries))
            .Trim()
            .Replace(" ", "_")
            .ToLower();
    }
    #endregion
}
