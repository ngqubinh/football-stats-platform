using FSP.Application.DTOs.Football;
using FSP.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FSP.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClubController : ControllerBase
{
    private readonly IClubAppService _club;
    private readonly ILogger<ClubController> _logger;

    public ClubController(IClubAppService club, ILogger<ClubController> logger)
    {
        this._club = club;
        this._logger = logger;
    }

    [HttpGet("league/{leagueId}/clubs")]
    public async Task<ActionResult<IEnumerable<ClubDto>>> GetClubsByLeague(int leagueId)
    {
        string? correlationId = Guid.NewGuid().ToString();
        using IDisposable? scope = this._logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Endpoint"] = nameof(this.GetClubsByLeague)
        });

        this._logger.LogInformation("Fetching clubs for league ID {LeagueId}", leagueId);

        try
        {
            var result = await this._club.GetClubsByLeagueAsync(leagueId);

            if (!result.Success)
            {
                this._logger.LogWarning("Failed to fetch clubs for league ID {LeagueId}: {Message}", leagueId, result.Message);
                return BadRequest(result.Message ?? $"Failed to fetch clubs for league ID {leagueId}.");
            }

            this._logger.LogInformation("Successfully retrieved {Count} clubs for league ID {LeagueId}", result.Data?.Count() ?? 0, leagueId);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error fetching clubs for league ID {LeagueId}: {Message}", leagueId, ex.Message);
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpGet("{clubId}/trends")]
    public async Task<ActionResult<ShootingDto>> GetClubTrend(int clubId, [FromQuery] int seasons = 5)
    {
        string? correlationId = Guid.NewGuid().ToString();
        using IDisposable? scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Endpoint"] = nameof(this.GetClubTrend)
        });
        this._logger.LogInformation("Fetching club trend stats for Club ID {ClubID}", clubId);
        try
        {
            var result = await this._club.GetClubTrendAsync(clubId, seasons);
            if (!result.Success)
            {
                this._logger.LogWarning("Failed to fetch club trend stats for player {ClubID}: {Message}", clubId, result.Message);
                return BadRequest(result.Message ?? $"Failed to fetch club trend stats for club {clubId}.");
            }
            this._logger.LogInformation("Successfully retrieved club trend stats for Club ID {ClubID}", clubId);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error fetching club trend stats for club {ClubID}: {Message}", clubId, ex.Message);
            return StatusCode(500, "Internal server error.");
        }
    }
}