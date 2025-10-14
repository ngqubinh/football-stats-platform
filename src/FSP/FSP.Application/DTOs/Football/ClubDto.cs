using System.Text.Json.Serialization;

namespace FSP.Application.DTOs.Football;

public class ClubDto
{
    [JsonPropertyName("club_id")]
    public int ClubId { get; set; }

    [JsonPropertyName("club_name")]
    public string ClubName { get; set; } = string.Empty;

    [JsonPropertyName("nation")]
    public string Nation { get; set; } = string.Empty;

    [JsonPropertyName("league_id")]
    public int LeagueId { get; set; }

    [JsonPropertyName("league_name")]
    public string LeagueName { get; set; } = string.Empty;
}