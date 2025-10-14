using FSP.Domain.Entities.Core;

namespace FSP.Domain.Interfaces.Core;

public interface IDataStorageService
{
    Task<string> SavePlayersToJsonAsync(List<Player> players, string clubName, League league);
    Task<string> SaveGoalkeepersToJsonAsync(List<Goalkeeping> goalkeepers, string clubName, League league);
    Task<string> SaveShootingsToJsonAsync(List<Shooting> shootings, string clubName, League league);
    Task SaveMatchLogsToJsonAsync(List<MatchLog> matchLogs, string clubName, League league);
    Task<string> SavePlayerDetailsToJsonAsync(PlayerDetails playerDetails, string playerName);
}