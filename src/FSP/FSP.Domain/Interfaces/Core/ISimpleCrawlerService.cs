using FSP.Domain.Entities;
using FSP.Domain.Entities.Core;

namespace FSP.Domain.Interfaces.Core;

public class CompleteTeamData
{
    public List<Player> Players { get; set; } = new();
    public List<Goalkeeping> Goalkeeping { get; set; } = new();
    public List<Shooting> Shooting { get; set; } = new();
    public List<MatchLog> MatchLogs { get; set; } = new();
    public string RawHtml { get; set; } = string.Empty;
    public string TeamId { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public string SourceUrl { get; set; } = string.Empty;
    public DateTime ExtractedAt { get; set; } = DateTime.UtcNow;
}

public interface ISimpleCrawlerService
{
    Task<Result<List<Player>>> ExtractPlayersFromUrlAsync(string url, string selector);
    Task<Result<List<Goalkeeping>>> ExtractGoalkeepingFromUrlAsync(string url, string selector);
    Task<Result<List<Shooting>>> ExtractShootingFromUrlAsync(string url, string selector);
    Task<Result<List<MatchLog>>> ExtractMatchLogFromUrlAsync(string url, string selector);
    Task<Result<PlayerDetails>> ExtractPlayerDetailsFromUrlAsync(string url, string selector, string clubName = null!);
    Task<Result<string>> GetRawHtmlAsync(string url);
    Task<Result<CompleteTeamData>> ExtractAllDataFromUrlAsync(string url, string id);
}
