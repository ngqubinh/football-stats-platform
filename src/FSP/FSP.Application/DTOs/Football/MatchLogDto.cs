using System.Text.Json.Serialization;

namespace FSP.Application.DTOs.Football;

public class MatchLogDto
{
    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    [JsonPropertyName("time")]
    public string Time { get; set; } = string.Empty;

    [JsonPropertyName("competition")]
    public string Competition { get; set; } = string.Empty;

    [JsonPropertyName("round")]
    public string Round { get; set; } = string.Empty;

    [JsonPropertyName("day")]
    public string Day { get; set; } = string.Empty;

    [JsonPropertyName("venue")]
    public string Venue { get; set; } = string.Empty;

    [JsonPropertyName("result")]
    public string Result { get; set; } = string.Empty;

    [JsonPropertyName("goals_for")]
    public string GoalsFor { get; set; } = string.Empty;

    [JsonPropertyName("goals_against")]
    public string GoalsAgainst { get; set; } = string.Empty;

    [JsonPropertyName("opponent")]
    public string Opponent { get; set; } = string.Empty;

    [JsonPropertyName("possession")]
    public string Possession { get; set; } = string.Empty;

    [JsonPropertyName("attendance")]
    public string Attendance { get; set; } = string.Empty;

    [JsonPropertyName("captain")]
    public string Captain { get; set; } = string.Empty;

    [JsonPropertyName("formation")]
    public string Formation { get; set; } = string.Empty;

    [JsonPropertyName("opponent_formation")]
    public string OppFormation { get; set; } = string.Empty;

    [JsonPropertyName("referee")]
    public string Referee { get; set; } = string.Empty;
}