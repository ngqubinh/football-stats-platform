using System.Text.Json.Serialization;
using FSP.Application.DTOs.Football;

namespace FSP.Application.DTOs.Core;

public class URLInformationDto
{
    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;
    [JsonPropertyName("url")]
    public string URL { get; set; } = string.Empty;
    [JsonPropertyName("league")]
    public LeagueDto League { get; set; } = new LeagueDto();
    [JsonPropertyName("status_code")]
    public int StatusCode { get; set; }
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    [JsonPropertyName("season")]
    public string Season { get; set; } = string.Empty;
}
