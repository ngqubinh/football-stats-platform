using FSP.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FSP.WebApi.Controllers
{
    [Route("api/leagues")]
    [ApiController]
    public class LeagueController : ControllerBase
    {
        private readonly ILeagueAppService _league;
        private readonly ILogger<LeagueController> _logger;

        public LeagueController(ILeagueAppService league, ILogger<LeagueController> logger)
        {
            this._league = league;
            this._logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetLeagues()
        {
            string? correlationId = Guid.NewGuid().ToString();
            using IDisposable? scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["Endpoint"] = nameof(this.GetLeagues)
            });

            this._logger.LogInformation("Fetching leagues");

            try
            {
                var result = await this._league.GetLeaguesAsync();

                if (!result.Success)
                {
                    this._logger.LogWarning("Failed to fetch leagues: {Message}", result.Message);
                    return BadRequest(result.Message ?? "Failed to fetch leagues.");
                }

                this._logger.LogInformation("Successfully retrieved {Count} leagues", result.Data?.Count() ?? 0);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error fetching leagues: {Message}", ex.Message);
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
