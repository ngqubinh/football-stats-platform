// FSP.Application.Services
using FSP.Application.DTOs.Core;
using FSP.Application.DTOs.Football;
using FSP.Application.Mappings;
using FSP.Domain.Entities;
using FSP.Domain.Entities.Core;
using FSP.Domain.Interfaces.Core;
using Microsoft.Extensions.Logging;

namespace FSP.Application.Services;

public interface ISimpleCrawlerAppService
{
    Task<Result<List<PlayerDto>>> ExtractPlayersAsync(string url, string selector);
    Task<Result<List<GoalkeepingDto>>> ExtractGoalkeepingAsync(string url, string selector);
    Task<Result<List<ShootingDto>>> ExtractShootingAsync(string url, string selector);
    Task<Result<List<MatchLogDto>>> ExtractMatchLogsAsync(string url, string selector);
    Task<Result<PlayerDetails>> ExtractPlayerDetailsAsync(string url, string selector, string clubName = null!);
    Task<Result<string>> GetRawHtmlAsync(string url);
    Task<Result<CompleteTeamDataDto>> ExtractAllDataAsync(string url, string id);
}

public class SimpleCrawlerAppService : ISimpleCrawlerAppService
{
    private readonly ISimpleCrawlerService _crawlerService;
    private readonly ICoreMappingService _mappingService;
    private readonly IPlayerMappingService _playerMappingService;
    private readonly IMatchLogMappingService _matchLogMappingService;
    private readonly ILogger<SimpleCrawlerAppService> _logger;

    public SimpleCrawlerAppService(
        ISimpleCrawlerService crawlerService,
        ICoreMappingService mappingService,
        ILogger<SimpleCrawlerAppService> logger,
        IPlayerMappingService playerMappingService,
        IMatchLogMappingService matchLogMappingService)
    {
        this._crawlerService = crawlerService;
        this._mappingService = mappingService;
        this._logger = logger;
        this._playerMappingService = playerMappingService;
        this._matchLogMappingService = matchLogMappingService;
    }

    public async Task<Result<List<PlayerDto>>> ExtractPlayersAsync(string url, string selector)
    {
        try
        {
            _logger.LogInformation("Extracting players from {Url}", url);
            
            var domainResult = await _crawlerService.ExtractPlayersFromUrlAsync(url, selector);
            if (!domainResult.Success)
            {
                return Result<List<PlayerDto>>.Fail(domainResult.Message!);
            }

            var playerDtos = this._playerMappingService.ToPlayerDtos(domainResult.Data!).ToList();
            
            _logger.LogInformation("Successfully extracted {Count} players from {Url}", playerDtos.Count, url);
            return Result<List<PlayerDto>>.Ok(playerDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting players from {Url}: {Message}", url, ex.Message);
            return Result<List<PlayerDto>>.Fail($"Error extracting players: {ex.Message}");
        }
    }

    public async Task<Result<List<GoalkeepingDto>>> ExtractGoalkeepingAsync(string url, string selector)
    {
        try
        {
            _logger.LogInformation("Extracting goalkeeping data from {Url}", url);
            
            var domainResult = await _crawlerService.ExtractGoalkeepingFromUrlAsync(url, selector);
            if (!domainResult.Success)
            {
                return Result<List<GoalkeepingDto>>.Fail(domainResult.Message!);
            }

            var goalkeepingDtos = this._playerMappingService.ToGoalkeepingDtos(domainResult.Data!).ToList();
            
            _logger.LogInformation("Successfully extracted {Count} goalkeeping records from {Url}", goalkeepingDtos.Count, url);
            return Result<List<GoalkeepingDto>>.Ok(goalkeepingDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting goalkeeping data from {Url}: {Message}", url, ex.Message);
            return Result<List<GoalkeepingDto>>.Fail($"Error extracting goalkeeping data: {ex.Message}");
        }
    }

    public async Task<Result<List<ShootingDto>>> ExtractShootingAsync(string url, string selector)
    {
        try
        {
            _logger.LogInformation("Extracting shooting data from {Url}", url);
            
            var domainResult = await _crawlerService.ExtractShootingFromUrlAsync(url, selector);
            if (!domainResult.Success)
            {
                return Result<List<ShootingDto>>.Fail(domainResult.Message!);
            }

            var shootingDtos = this._playerMappingService.ToShootingDtos(domainResult.Data!).ToList();
            
            _logger.LogInformation("Successfully extracted {Count} shooting records from {Url}", shootingDtos.Count, url);
            return Result<List<ShootingDto>>.Ok(shootingDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting shooting data from {Url}: {Message}", url, ex.Message);
            return Result<List<ShootingDto>>.Fail($"Error extracting shooting data: {ex.Message}");
        }
    }

    public async Task<Result<List<MatchLogDto>>> ExtractMatchLogsAsync(string url, string selector)
    {
        try
        {
            this._logger.LogInformation("Extracting match logs from {Url}", url);
            
            var domainResult = await this._crawlerService.ExtractMatchLogFromUrlAsync(url, selector);
            if (!domainResult.Success)
            {
                return Result<List<MatchLogDto>>.Fail(domainResult.Message!);
            }

            var matchLogDtos = this._matchLogMappingService.ToMatchLogDtos(domainResult.Data!).ToList();

            this._logger.LogInformation("Successfully extracted {Count} match logs from {Url}", matchLogDtos.Count, url);
            return Result<List<MatchLogDto>>.Ok(matchLogDtos);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error extracting match logs from {Url}: {Message}", url, ex.Message);
            return Result<List<MatchLogDto>>.Fail($"Error extracting match logs: {ex.Message}");
        }
    }

    public async Task<Result<PlayerDetails>> ExtractPlayerDetailsAsync(string url, string selector, string clubName = null!)
    {
        try
        {
            this._logger.LogInformation("Extracting player details from {Url}", url);
            
            var domainResult = await this._crawlerService.ExtractPlayerDetailsFromUrlAsync(url, selector, clubName);
            if (!domainResult.Success)
            {
                return Result<PlayerDetails>.Fail(domainResult.Message!);
            }

            this._logger.LogInformation("Successfully extracted player details for {PlayerName} from {Url}", 
                domainResult.Data!.FullName, url);
                
            return Result<PlayerDetails>.Ok(domainResult.Data!);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error extracting player details from {Url}: {Message}", url, ex.Message);
            return Result<PlayerDetails>.Fail($"Error extracting player details: {ex.Message}");
        }
    }

    public async Task<Result<string>> GetRawHtmlAsync(string url)
    {
        try
        {
            this._logger.LogInformation("Fetching raw HTML from {Url}", url);
            
            var domainResult = await this._crawlerService.GetRawHtmlAsync(url);
            if (!domainResult.Success)
            {
                return Result<string>.Fail(domainResult.Message!);
            }

            this._logger.LogInformation("Successfully fetched HTML from {Url} ({Length} characters)", 
                url, domainResult.Data!.Length);
                
            return Result<string>.Ok(domainResult.Data!);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error fetching HTML from {Url}: {Message}", url, ex.Message);
            return Result<string>.Fail($"Error fetching HTML: {ex.Message}");
        }
    }

    public async Task<Result<CompleteTeamDataDto>> ExtractAllDataAsync(string url, string id)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            string errorMsg = "URL cannot be null or empty.";
            this._logger?.LogWarning(errorMsg);
            return Result<CompleteTeamDataDto>.Fail(errorMsg);
        }

        try
        {
            this._logger?.LogInformation("Starting extraction of all team data from {Url}", url);

            Result<CompleteTeamData> result = await this._crawlerService.ExtractAllDataFromUrlAsync(url, id);
            if (!result.Success)
            {
                this._logger?.LogWarning("Failed to extract data from {Url}: {Message}", url, result.Message);
                return Result<CompleteTeamDataDto>.Fail(result.Message ?? "Unknown extraction error.");
            }

            if (result.Data == null)
            {
                string errorMsg = "Extracted data is null.";
                this._logger?.LogWarning(errorMsg);
                return Result<CompleteTeamDataDto>.Fail(errorMsg);
            }

            // Map domain entity to DTO using the mapping service
            CompleteTeamDataDto dto = _mappingService.ToCompleteTeamDataDto(result.Data);

            this._logger?.LogInformation("Successfully extracted all team data from {Url}: {PlayerCount} players, {GoalkeepingCount} goalkeeping records, {ShootingCount} shooting records, {MatchLogCount} match logs",
                url, dto.Players.Count, dto.Goalkeeping.Count, dto.Shooting.Count, dto.MatchLogs.Count);

            return Result<CompleteTeamDataDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            string errorMsg = $"Unexpected error extracting data from {url}: {ex.Message}";
            this._logger?.LogError(ex, errorMsg);
            return Result<CompleteTeamDataDto>.Fail(errorMsg);
        }
    }
}