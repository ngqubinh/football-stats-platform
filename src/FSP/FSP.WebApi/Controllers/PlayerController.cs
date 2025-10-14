using FSP.Application.DTOs.Football;
using FSP.Application.Services;
using FSP.Domain.Entities.Core;
using Microsoft.AspNetCore.Mvc;

namespace FSP.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlayerController : ControllerBase
{
    private readonly IPlayerAppService _player;
    private readonly ILogger<PlayerController> _logger;

    public PlayerController(IPlayerAppService player, ILogger<PlayerController> logger)
    {
        this._player = player;
        this._logger = logger;
    }

    [HttpGet("club/{clubId}/players")]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetPlayersByClub(int clubId)
    {
        string? correlationId = Guid.NewGuid().ToString();
        using IDisposable? scope = this._logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Endpoint"] = nameof(this.GetPlayersByClub)
        });

        this._logger.LogInformation("Fetching players for club ID {ClubId}", clubId);

        try
        {
            var result = await this._player.GetPlayersByClubAsync(clubId);

            if (!result.Success)
            {
                this._logger.LogWarning("Failed to fetch players for club ID {ClubId}: {Message}", clubId, result.Message);
                return BadRequest(result.Message ?? $"Failed to fetch players for club ID {clubId}.");
            }

            this._logger.LogInformation("Successfully retrieved {Count} players for club ID {ClubId}", result.Data?.Count() ?? 0, clubId);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error fetching players for club ID {ClubId}: {Message}", clubId, ex.Message);
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpGet("club/{clubId}/players/current")]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetCurrentPlayersByClub(int clubId)
    {
        string? correlationId = Guid.NewGuid().ToString();
        using IDisposable? scope = this._logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Endpoint"] = nameof(this.GetCurrentPlayersByClub)
        });

        this._logger.LogInformation("Fetching current players for club ID {ClubId}:", clubId);

        try
        {
            var result = await this._player.GetCurrentPlayersByClubAsync(clubId);

            if (!result.Success)
            {
                this._logger.LogWarning("Failed to fetch current players for club ID {ClubId}: {Message}",
                    clubId, result.Message);
                return BadRequest(result.Message ?? $"Failed to fetch current players for club ID {clubId}.");
            }

            this._logger.LogInformation("Successfully retrieved {Count} current players for club ID {ClubId}",
                result.Data?.Count() ?? 0, clubId);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error fetching current players for club ID {ClubId}: {Message}",
                clubId, ex.Message);
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpGet("{playerRefId}/season-comparisons")]
    public async Task<ActionResult<IEnumerable<PlayerSeasonComparison>>> ComparePlayerWithPreviousSeasons(string playerRefId)
    {
        string? correlationId = Guid.NewGuid().ToString();
        using IDisposable? scope = this._logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Endpoint"] = nameof(this.ComparePlayerWithPreviousSeasons)
        });

        this._logger.LogInformation("Comparing player {PlayerId} with previous seasons", playerRefId);

        try
        {
            var result = await this._player.ComparePlayerWithPreviousSeasonAsync(playerRefId);

            if (!result.Success)
            {
                this._logger.LogWarning("Failed to compare player {PlayerId} with previous seasons: {Message}",
                    playerRefId, result.Message);
                return BadRequest(result.Message ?? $"Failed to compare player {playerRefId} with previous seasons.");
            }

            this._logger.LogInformation("Successfully retrieved {Count} season comparisons for player Ref ID {PlayerId}",
                result.Data?.Count() ?? 0, playerRefId);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error comparing player {PlayerId} with previous seasons: {Message}",
                playerRefId, ex.Message);
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpGet("{playerRefId}/current-previous-comparison")]
    public async Task<ActionResult<PlayerSeasonComparison>> GetPlayerCurrentVsPreviousSeason(string playerRefId)
    {
        string? correlationId = Guid.NewGuid().ToString();
        using IDisposable? scope = this._logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Endpoint"] = nameof(this.GetPlayerCurrentVsPreviousSeason)
        });

        this._logger.LogInformation("Comparing current vs previous season for player {PlayerId}", playerRefId);

        try
        {
            var result = await this._player.GetPlayerCurrentVsPreviousSeasonAsync(playerRefId);

            if (!result.Success)
            {
                this._logger.LogWarning("Failed to compare current vs previous season for player {PlayerId}: {Message}",
                    playerRefId, result.Message);
                return BadRequest(result.Message ?? $"Failed to compare current vs previous season for player {playerRefId}.");
            }

            this._logger.LogInformation("Successfully retrieved current vs previous season comparison for player ID {PlayerId}",
                playerRefId);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error comparing current vs previous season for player {PlayerId}: {Message}",
                playerRefId, ex.Message);
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpGet("{playerRefId}/goalkeeping")]
    public async Task<ActionResult<GoalkeepingDto>> GetCurrentGoalkeepingByPlayer(string playerRefId)
    {
        string? correlationId = Guid.NewGuid().ToString();
        using IDisposable? scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Endpoint"] = nameof(GetCurrentGoalkeepingByPlayer)
        });
        _logger.LogInformation("Fetching goalkeeping stats for player {PlayerRefId}", playerRefId);
        try
        {
            var result = await _player.GetCurrentGoalkeepingByPlayerAsync(playerRefId);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to fetch goalkeeping stats for player {PlayerRefId}: {Message}", playerRefId, result.Message);
                return BadRequest(result.Message ?? $"Failed to fetch goalkeeping stats for player {playerRefId}.");
            }
            _logger.LogInformation("Successfully retrieved goalkeeping stats for player Ref ID {PlayerRefId}", playerRefId);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching goalkeeping stats for player {PlayerRefId}: {Message}", playerRefId, ex.Message);
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpGet("{playerRefId}/shooting")]
    public async Task<ActionResult<ShootingDto>> GetCurrentShootingByPlayer(string playerRefId)
    {
        string? correlationId = Guid.NewGuid().ToString();
        using IDisposable? scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Endpoint"] = nameof(GetCurrentShootingByPlayer)
        });
        _logger.LogInformation("Fetching shooting stats for player {PlayerRefId}", playerRefId);
        try
        {
            var result = await _player.GetCurrentShootingByPlayerAsync(playerRefId);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to fetch shooting stats for player {PlayerRefId}: {Message}", playerRefId, result.Message);
                return BadRequest(result.Message ?? $"Failed to fetch shooting stats for player {playerRefId}.");
            }
            _logger.LogInformation("Successfully retrieved shooting stats for player Ref ID {PlayerRefId}", playerRefId);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching shooting stats for player {PlayerRefId}: {Message}", playerRefId, ex.Message);
            return StatusCode(500, "Internal server error.");
        }
    }
}