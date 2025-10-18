using FSP.Domain.Entities.Core;

namespace FSP.Domain.Interfaces.Core;

public interface IHtmlParserService
{
    Task<List<Player>> ExtractPlayersTableAsync(string html, string selector);
    Task<List<SquadStandard>> ExtractSquadStandardTableAsync(string html, string selector);
    Task<List<Goalkeeping>> ExtractGoalkeepingTableAsync(string html, string selector);
    Task<List<Shooting>> ExtractShootingTableAsync(string html, string selector);
    Task<List<MatchLog>> ExtractMatchLogTableAsync(string html, string selector);
    Task<PlayerDetails> ExtractPlayerDetailsAsync(string html, string selector, string clubName);
}
