using System.Text.Json.Serialization;
using FSP.Application.DTOs.Football;
using FSP.Domain.Entities.Core;

namespace FSP.Application.DTOs.Core;

public class CompleteTeamDataDto
{
    [JsonPropertyName("players")]
    public List<PlayerDto> Players { get; set; } = new();
    [JsonPropertyName("goalkeeping")]
    public List<GoalkeepingDto> Goalkeeping { get; set; } = new();
    [JsonPropertyName("shooting")]
    public List<ShootingDto> Shooting { get; set; } = new();
    [JsonPropertyName("match_logs")]
    public List<MatchLogDto> MatchLogs { get; set; } = new();
    [JsonPropertyName("raw_html")]
    public string RawHtml { get; set; } = string.Empty;
    [JsonPropertyName("team_id")]
    public string TeamId { get; set; } = string.Empty;
    [JsonPropertyName("team_name")]
    public string TeamName { get; set; } = string.Empty;
    [JsonPropertyName("source_url")]
    public string SourceUrl { get; set; } = string.Empty;
    [JsonPropertyName("extracted_at")]
    public DateTime ExtractedAt { get; set; } = DateTime.UtcNow;
}
