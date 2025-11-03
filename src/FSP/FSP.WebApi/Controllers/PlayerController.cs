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

    [HttpGet("club/{clubId}/players/core")]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetCorePlayersByClub(int clubId)
    {
        string? correlationId = Guid.NewGuid().ToString();
        using IDisposable? scope = this._logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Endpoint"] = nameof(this.GetCorePlayersByClub)
        });

        this._logger.LogInformation("Fetching core players for club ID {ClubId}", clubId);

        try
        {
            var result = await this._player.GetCorePlayersByClubAsync(clubId);

            if (!result.Success)
            {
                this._logger.LogWarning("Failed to fetch core players for club ID {ClubId}: {Message}", clubId, result.Message);
                return BadRequest(result.Message ?? $"Failed to fetch core players for club ID {clubId}.");
            }

            var corePlayers = result.Data?.ToList() ?? new List<PlayerDto>();

            this._logger.LogInformation(
                "Successfully retrieved {Count} core players for club ID {ClubId}. Formation breakdown: {Attackers} attackers, {Midfielders} midfielders, {Defenders} defenders, {Goalkeepers} goalkeepers",
                corePlayers.Count, clubId,
                corePlayers.Count(p => IsAttackingPosition(p.Position)),
                corePlayers.Count(p => IsMidfieldPosition(p.Position)),
                corePlayers.Count(p => IsDefensivePosition(p.Position)),
                corePlayers.Count(p => IsGoalkeeperPosition(p.Position))
            );

            return Ok(corePlayers);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error fetching core players for club ID {ClubId}: {Message}", clubId, ex.Message);
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpGet("club/{clubId}/players/scores")]
    public async Task<ActionResult<PlayerScoreSummaryDto>> GetPlayerScoresByClub(int clubId)
    {
        string? correlationId = Guid.NewGuid().ToString();
        using IDisposable? scope = this._logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Endpoint"] = nameof(this.GetPlayerScoresByClub)
        });

        this._logger.LogInformation("Fetching player scores for club ID {ClubId}", clubId);

        try
        {
            var result = await this._player.GetPlayerScoresByClubAsync(clubId);

            if (!result.Success)
            {
                this._logger.LogWarning("Failed to fetch player scores for club ID {ClubId}: {Message}", clubId, result.Message);
                return BadRequest(result.Message ?? $"Failed to fetch player scores for club ID {clubId}.");
            }

            var playersWithScores = result.Data?.ToList() ?? new List<PlayerWithScoreDto>();

            // Create summary
            var summary = new PlayerScoreSummaryDto
            {
                TotalPlayers = playersWithScores.Count,
                AverageScore = playersWithScores.Any() ? playersWithScores.Average(p => p.Score) : 0,
                TopScorer = playersWithScores.OrderByDescending(p => p.Score).FirstOrDefault(),
                Players = playersWithScores
            };

            this._logger.LogInformation(
                "Successfully retrieved scores for {Count} players in club ID {ClubId}. " +
                "Top scorer: {TopPlayer} with {TopScore} points",
                playersWithScores.Count, clubId,
                summary.TopScorer?.Player.PlayerName,
                summary.TopScorer?.Score
            );

            return Ok(summary);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error fetching player scores for club ID {ClubId}: {Message}", clubId, ex.Message);
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpPost("club/{clubId}/lineup/analyze")]
    public async Task<ActionResult<LineupAnalysisDto>> AnalyzeLineup(int clubId, [FromBody] LineupInputDto lineupInput)
    {
        string? correlationId = Guid.NewGuid().ToString();
        using IDisposable? scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Endpoint"] = nameof(AnalyzeLineup),
            ["ClubId"] = clubId
        });

        _logger.LogInformation("Analyzing lineup for club {ClubId} with {Count} players",
            clubId, lineupInput?.Players?.Count ?? 0);

        try
        {
            var result = await _player.AnalyzeLineupAsync(clubId, lineupInput!);

            if (!result.Success)
            {
                _logger.LogWarning("Failed to analyze lineup for club {ClubId}: {Message}", clubId, result.Message);
                return BadRequest(result.Message ?? $"Failed to analyze lineup for club {clubId}.");
            }

            _logger.LogInformation(
                "Lineup analysis completed for club {ClubId}. Total score: {TotalScore}, Rating: {Rating}",
                clubId, result.Data?.TotalScore, result.Data?.OverallRating
            );

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing lineup for club {ClubId}: {Message}", clubId, ex.Message);
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpPost("lineup/compare-teams")]
    public async Task<ActionResult<TwoTeamComparisonDto>> CompareTwoTeams([FromBody] TwoTeamLineupInputDto twoTeamInput)
    {
        string? correlationId = Guid.NewGuid().ToString();
        using IDisposable? scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Endpoint"] = nameof(CompareTwoTeams),
            ["TeamAClubId"] = twoTeamInput?.TeamA?.ClubId ?? 0,
            ["TeamBClubId"] = twoTeamInput?.TeamB?.ClubId ?? 0
        });

        _logger.LogInformation("Comparing two teams: {TeamA} vs {TeamB}",
            twoTeamInput?.TeamA?.ClubName, twoTeamInput?.TeamB?.ClubName);

        try
        {
            var result = await _player.CompareTwoTeamsAsync(twoTeamInput!);

            if (!result.Success)
            {
                _logger.LogWarning("Failed to compare two teams: {Message}", result.Message);
                return BadRequest(result.Message ?? "Failed to compare two teams.");
            }

            _logger.LogInformation(
                "Two-team comparison completed. Predicted winner: {Winner} with {Confidence} confidence",
                result.Data?.Prediction?.PredictedWinner, result.Data?.Prediction?.Confidence
            );

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error comparing two teams: {Message}", ex.Message);
            return StatusCode(500, "Internal server error.");
        }
    }

    #region Helper Methods for Position Analysis
    private bool IsGoalkeeperPosition(string position)
    {
        if (string.IsNullOrEmpty(position)) return false;
        return position.Contains("GK", StringComparison.OrdinalIgnoreCase);
    }

    private bool IsDefensivePosition(string position)
    {
        if (string.IsNullOrEmpty(position)) return false;
        var defensivePositions = new[] { "DF", "CB", "FB", "RB", "LB", "WB", "RWB", "LWB" };
        return defensivePositions.Any(pos => position.Contains(pos, StringComparison.OrdinalIgnoreCase));
    }

    private bool IsMidfieldPosition(string position)
    {
        if (string.IsNullOrEmpty(position)) return false;
        var midfieldPositions = new[] { "MF", "CM", "AM", "DM", "LM", "RM", "CAM", "CDM", "LW", "RW" };
        return midfieldPositions.Any(pos => position.Contains(pos, StringComparison.OrdinalIgnoreCase));
    }

    private bool IsAttackingPosition(string position)
    {
        if (string.IsNullOrEmpty(position)) return false;
        var attackingPositions = new[] { "FW", "ST", "CF", "SS" };
        return attackingPositions.Any(pos => position.Contains(pos, StringComparison.OrdinalIgnoreCase));
    }
    #endregion
}
