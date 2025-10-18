using System.IO.Compression;
using System.Text;
using System.Text.Json;
using FSP.Application.DTOs.Core;
using FSP.Application.DTOs.Football;
using FSP.Application.Services;
using FSP.Domain.Entities;
using FSP.Domain.Entities.Core;
using Microsoft.AspNetCore.Mvc;

namespace FSP.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SimpleCrawlerController : ControllerBase
{
    private readonly ISimpleCrawlerAppService _crawlerAppService;
    private readonly ILogger<SimpleCrawlerController> _logger;

    public SimpleCrawlerController(ISimpleCrawlerAppService crawlerAppService, ILogger<SimpleCrawlerController> logger)
    {
        this._crawlerAppService = crawlerAppService;
        this._logger = logger;
    }

    [HttpGet("players")]
    public async Task<ActionResult<List<PlayerDto>>> ExtractPlayers(
        [FromQuery] string url,
        [FromQuery] string selector = "//div[contains(@id, 'div_stats_standard')]//table")
    {
        string correlationId = Guid.NewGuid().ToString();
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Endpoint"] = nameof(ExtractPlayers),
            ["Url"] = url
        });

        _logger.LogInformation("Starting player extraction from URL");

        try
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("URL parameter is required");
            }

            var result = await _crawlerAppService.ExtractPlayersAsync(url, selector);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to extract players: {Message}", result.Message);
                return BadRequest(result.Message ?? "Failed to extract players.");
            }

            _logger.LogInformation("Successfully extracted {Count} players", result.Data?.Count ?? 0);
            return Ok(result.Data ?? new List<PlayerDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting players: {Message}", ex.Message);
            return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
        }
    }

    [HttpGet("goalkeeping")]
    public async Task<ActionResult<List<GoalkeepingDto>>> ExtractGoalkeeping(
        [FromQuery] string url,
        [FromQuery] string selector = "//div[contains(@id, 'div_stats_keeper')]//table")
    {
        string correlationId = Guid.NewGuid().ToString();
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Endpoint"] = nameof(ExtractGoalkeeping),
            ["Url"] = url
        });

        _logger.LogInformation("Starting goalkeeping data extraction from URL");

        try
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("URL parameter is required");
            }

            var result = await _crawlerAppService.ExtractGoalkeepingAsync(url, selector);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to extract goalkeeping data: {Message}", result.Message);
                return BadRequest(result.Message ?? "Failed to extract goalkeeping data.");
            }

            _logger.LogInformation("Successfully extracted {Count} goalkeeping records", result.Data?.Count ?? 0);
            return Ok(result.Data ?? new List<GoalkeepingDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting goalkeeping data: {Message}", ex.Message);
            return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
        }
    }

    [HttpGet("shooting")]
    public async Task<ActionResult<List<ShootingDto>>> ExtractShooting(
        [FromQuery] string url,
        [FromQuery] string selector = "//div[contains(@id, 'div_stats_shooting')]//table")
    {
        string correlationId = Guid.NewGuid().ToString();
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Endpoint"] = nameof(ExtractShooting),
            ["Url"] = url
        });

        _logger.LogInformation("Starting shooting data extraction from URL");

        try
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("URL parameter is required");
            }

            var result = await _crawlerAppService.ExtractShootingAsync(url, selector);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to extract shooting data: {Message}", result.Message);
                return BadRequest(result.Message ?? "Failed to extract shooting data.");
            }

            _logger.LogInformation("Successfully extracted {Count} shooting records", result.Data?.Count ?? 0);
            return Ok(result.Data ?? new List<ShootingDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting shooting data: {Message}", ex.Message);
            return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
        }
    }

    [HttpGet("match-logs")]
    public async Task<ActionResult<List<MatchLogDto>>> ExtractMatchLogs(
        [FromQuery] string url,
        [FromQuery] string selector = "//table[contains(@id, 'matchlogs')]")
    {
        string correlationId = Guid.NewGuid().ToString();
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Endpoint"] = nameof(ExtractMatchLogs),
            ["Url"] = url
        });

        _logger.LogInformation("Starting match logs extraction from URL");

        try
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("URL parameter is required");
            }

            var result = await _crawlerAppService.ExtractMatchLogsAsync(url, selector);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to extract match logs: {Message}", result.Message);
                return BadRequest(result.Message ?? "Failed to extract match logs.");
            }

            _logger.LogInformation("Successfully extracted {Count} match logs", result.Data?.Count ?? 0);
            return Ok(result.Data ?? new List<MatchLogDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting match logs: {Message}", ex.Message);
            return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
        }
    }

    [HttpGet("player-details")]
    public async Task<ActionResult<PlayerDetails>> ExtractPlayerDetails(
        [FromQuery] string url,
        [FromQuery] string selector = "//div[@id='info']",
        [FromQuery] string? clubName = null)
    {
        string correlationId = Guid.NewGuid().ToString();
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Endpoint"] = nameof(ExtractPlayerDetails),
            ["Url"] = url,
            ["ClubName"] = clubName ?? "Unknown"
        });

        _logger.LogInformation("Starting player details extraction");

        try
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("URL parameter is required");
            }

            var result = await _crawlerAppService.ExtractPlayerDetailsAsync(url, selector, clubName!);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to extract player details: {Message}", result.Message);
                return BadRequest(result.Message ?? "Failed to extract player details.");
            }

            _logger.LogInformation("Successfully extracted player details for {PlayerName}",
                result.Data?.FullName ?? "Unknown");

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting player details: {Message}", ex.Message);
            return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
        }
    }

    [HttpGet("raw-html")]
    public async Task<ActionResult<string>> GetRawHtml([FromQuery] string url)
    {
        string correlationId = Guid.NewGuid().ToString();
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Endpoint"] = nameof(GetRawHtml),
            ["Url"] = url
        });

        _logger.LogInformation("Fetching raw HTML");

        try
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("URL parameter is required");
            }

            var result = await _crawlerAppService.GetRawHtmlAsync(url);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to fetch HTML: {Message}", result.Message);
                return BadRequest(result.Message ?? "Failed to fetch HTML.");
            }

            _logger.LogInformation("Successfully fetched HTML ({Length} characters)", result.Data?.Length ?? 0);
            return Content(result.Data ?? string.Empty, "text/html");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching HTML: {Message}", ex.Message);
            return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
        }
    }

    [HttpGet("all-data")]
    public async Task<ActionResult<CompleteTeamDataDto>> ExtractAllData([FromQuery] string url, [FromQuery] string id)
    {
        string correlationId = Guid.NewGuid().ToString();
        using IDisposable? scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Endpoint"] = nameof(ExtractAllData),
            ["Url"] = url
        });

        this._logger.LogInformation("Starting complete data extraction from URL");

        try
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("URL parameter is required");
            }

            Result<CompleteTeamDataDto> result = await _crawlerAppService.ExtractAllDataAsync(url, id);
            if (!result.Success)
            {
                this._logger.LogWarning("Failed to extract all data: {Message}", result.Message);
                return BadRequest(result.Message ?? "Failed to extract all data.");
            }

            // Create enhanced response
            var response = new EnhancedTeamDataResponse
            {
                Data = result.Data!,
                DownloadLinks = new
                {
                    Json = Url.Action(nameof(DownloadJSON), null, new { url, id }, Request.Scheme),
                    Zip = Url.Action(nameof(DownloadZip), null, new { url, id }, Request.Scheme)
                }
            };

            this._logger.LogInformation(
                "Successfully extracted all data: {PlayersCount} players, {GoalkeepingCount} goalkeeping, {ShootingCount} shooting, {MatchLogsCount} match logs",
                result.Data!.Players.Count,
                result.Data.Goalkeeping.Count,
                result.Data.Shooting.Count,
                result.Data.MatchLogs.Count
            );

            //return Ok(result.Data);
            return Ok(response);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error extracting all data: {Message}", ex.Message);
            return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
        }
    }

    [HttpGet("download-json")]
    public async Task<IActionResult> DownloadJSON([FromQuery] string url, [FromQuery] string id)
    {
        try
        {
            var result = await this._crawlerAppService.ExtractAllDataAsync(url, id);
            if (!result.Success)
            {
                return BadRequest(result.Message ?? "Failed to extract data.");
            }

            // Convert to JSON
            string jsonString = JsonSerializer.Serialize(result.Data, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

            byte[] bytes = Encoding.UTF8.GetBytes(jsonString);
            string fileName = $"{SanitizeFileName(result.Data!.TeamName ?? "team")}_{DateTime.UtcNow:yyyyMMddHHmmss}.json";
            return File(bytes, "application/json", fileName);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error generating JSON download");
            return StatusCode(500, "Error generating download file");
        }
    }

    [HttpGet("download-zip")]
    public async Task<IActionResult> DownloadZip([FromQuery] string url, [FromQuery] string id)
    {
        try
        {
            var result = await _crawlerAppService.ExtractAllDataAsync(url, id);

            if (!result.Success)
            {
                return BadRequest(result.Message ?? "Failed to extract data.");
            }

            using var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                // Main data file
                var mainEntry = archive.CreateEntry($"{result.Data!.TeamName}_complete_data.json");
                using (var entryStream = mainEntry.Open())
                using (var streamWriter = new StreamWriter(entryStream))
                {
                    var json = JsonSerializer.Serialize(result.Data, new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    await streamWriter.WriteAsync(json);
                }

                // Individual data files
                await AddDataToZip(archive, result.Data.Players, $"{result.Data.TeamName}_players.json");
                await AddDataToZip(archive, result.Data.Goalkeeping, $"{result.Data.TeamName}_goalkeeping.json");
                await AddDataToZip(archive, result.Data.Shooting, $"{result.Data.TeamName}_shooting.json");
                await AddDataToZip(archive, result.Data.MatchLogs, $"{result.Data.TeamName}_matchlogs.json");
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            var fileName = $"{SanitizeFileName(result.Data.TeamName ?? "team")}_complete_{DateTime.UtcNow:yyyyMMddHHmmss}.zip";

            return File(memoryStream.ToArray(), "application/zip", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating ZIP file");
            return StatusCode(500, "Error creating download file");
        }
    }

    [HttpGet("squad-standard")]
    public async Task<ActionResult<EnhancedSquadResponse>> ExtractSquadStandard(
    [FromQuery] string url,
    [FromQuery] string selector = "//table[@id='stats_squads_standard_for']")
    {
        string correlationId = Guid.NewGuid().ToString();
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Endpoint"] = nameof(ExtractSquadStandard),
            ["Url"] = url
        });

        _logger.LogInformation("Starting squad standard extraction from URL");

        try
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("URL parameter is required");
            }

            var result = await _crawlerAppService.ExtractSquadStandardFromUrlAsync(url, selector);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to extract squad standard data: {Message}", result.Message);
                return BadRequest(result.Message ?? "Failed to extract squad standard data.");
            }

            if (result.Data == null)
            {
                return BadRequest("No squad data found.");
            }

            var jsonLink = Url.Action(nameof(DownloadSquadStandardJSON), null, new { url, selector }, Request.Scheme);
            var zipLink = Url.Action(nameof(DownloadSquadStandardZip), null, new { url, selector }, Request.Scheme);

            if (jsonLink == null || zipLink == null)
            {
                return StatusCode(500, "Failed to generate download links.");
            }

            var response = new EnhancedSquadResponse
            {
                Data = result.Data,
                DownloadLinks = new DownloadLinks
                {
                    Json = jsonLink,
                    Zip = zipLink
                }
            };

            _logger.LogInformation("Successfully extracted {Count} squad records", result.Data?.Count ?? 0);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting squad standard data: {Message}", ex.Message);
            return StatusCode(500, new { Message = "Internal server error.", Error = ex.Message });
        }
    }

    [HttpGet("download-squad-standard-json")]
    public async Task<IActionResult> DownloadSquadStandardJSON([FromQuery] string url, [FromQuery] string selector = "//table[@id='stats_squads_standard_for']")
    {
        try
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("URL parameter is required");
            }

            var result = await _crawlerAppService.ExtractSquadStandardFromUrlAsync(url, selector);
            if (!result.Success)
            {
                _logger.LogError("Failed to extract squad standard data: {Message}", result.Message);
                return BadRequest(result.Message ?? "Failed to extract squad standard data.");
            }

            // Convert to JSON
            string jsonString = JsonSerializer.Serialize(result.Data, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            byte[] bytes = Encoding.UTF8.GetBytes(jsonString);
            string fileName = $"squad_standard_{DateTime.UtcNow:yyyyMMddHHmmss}.json";
            return File(bytes, "application/json", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating squad standard JSON download: {Message}", ex.Message);
            return StatusCode(500, "Error generating JSON download file");
        }
    }

    [HttpGet("download-squad-standard-zip")]
    public async Task<IActionResult> DownloadSquadStandardZip([FromQuery] string url, [FromQuery] string selector = "//table[@id='stats_squads_standard_for']")
    {
        try
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("URL parameter is required");
            }

            var result = await _crawlerAppService.ExtractSquadStandardFromUrlAsync(url, selector);
            if (!result.Success)
            {
                _logger.LogError("Failed to extract squad standard data: {Message}", result.Message);
                return BadRequest(result.Message ?? "Failed to extract squad standard data.");
            }

            using var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                // Add squad standard data to ZIP
                var entry = archive.CreateEntry("squad_standard_data.json");
                using (var entryStream = entry.Open())
                using (var streamWriter = new StreamWriter(entryStream))
                {
                    var json = JsonSerializer.Serialize(result.Data, new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    await streamWriter.WriteAsync(json);
                }
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            var fileName = $"squad_standard_{DateTime.UtcNow:yyyyMMddHHmmss}.zip";

            return File(memoryStream.ToArray(), "application/zip", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating squad standard ZIP file: {Message}", ex.Message);
            return StatusCode(500, "Error creating ZIP download file");
        }
    }

    #region helpers
    private object SanitizeFileName(string fileName)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        return string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries))
            .Trim()
            .Replace(" ", "_")
            .ToLower();
    }

    private async Task AddDataToZip<T>(ZipArchive archive, List<T> data, string fileName)
    {
        if (data != null && data.Any())
        {
            var entry = archive.CreateEntry(fileName);
            using var entryStream = entry.Open();
            using var streamWriter = new StreamWriter(entryStream);
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            await streamWriter.WriteAsync(json);
        }
    }
    #endregion
}

public class EnhancedTeamDataResponse
{
    public CompleteTeamDataDto Data { get; set; } = null!;
    public object DownloadLinks { get; set; } = string.Empty;
}

public class EnhancedSquadResponse
{
    public List<SquadStandardDto> Data { get; set; } = null!;
    public DownloadLinks DownloadLinks { get; set; } = null!;
}

public class DownloadLinks
{
    public string Json { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
}
