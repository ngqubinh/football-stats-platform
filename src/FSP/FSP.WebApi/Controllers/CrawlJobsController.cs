using FSP.Application.Services;
using FSP.Domain.Entities.Core;
using Microsoft.AspNetCore.Mvc;

namespace FSP.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrawlJobsController : ControllerBase
    {
        private readonly ICrawlingAppService _crawlingService;
        private readonly ILogger<CrawlJobsController> _logger;

        public CrawlJobsController(ICrawlingAppService crawlingService, ILogger<CrawlJobsController> logger)
        {
            _crawlingService = crawlingService ?? throw new ArgumentNullException(nameof(crawlingService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("premier-league")]
        public async Task<ActionResult<List<URLInformation>>> GetPremierLeagueCrawl()
        {
            string correlationId = Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["Endpoint"] = nameof(GetPremierLeagueCrawl)
            });

            _logger.LogInformation("Starting crawl job for Premier League");

            try
            {
                var result = await _crawlingService.CrawlPremierLeagueAsync();
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to start Premier League crawl job: {Message}", result.Message);
                    return BadRequest(result.Message ?? "Failed to start Premier League crawl job.");
                }

                if (result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning("No data retrieved for Premier League crawl job");
                    return Ok(new List<URLInformation>());
                }

                _logger.LogInformation("Successfully processed Premier League crawl job with {Count} URL statuses", result.Data.Count);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Premier League crawl job: {Message}", ex.Message);
                return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
            }
        }
    }
}
