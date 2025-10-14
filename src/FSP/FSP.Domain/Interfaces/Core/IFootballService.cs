using FSP.Domain.Entities;
using FSP.Domain.Entities.Core;

namespace FSP.Domain.Interfaces.Core;

public interface IFootballService
{
    Task<Result<IEnumerable<League>>> GetAllLeaguesAsync();
    Task<Result<IEnumerable<Club>>> GetClubsByLeagueAsync(int leagueId);
    Task<Result<IEnumerable<Player>>> GetPlayersByClubAsync(int clubId);
    Task<Result<IEnumerable<Player>>> GetCurrentPlayersByClubAsync(int clubId);
    Task<Result<IEnumerable<PlayerSeasonComparison>>> ComparePlayerWithPrevisousSeasonsAsync(string playerRefId);
    Task<Result<PlayerSeasonComparison>> GetPlayerCurrentVsPreviousSeasonAsync(string playerRefId);
    Task<Result<Goalkeeping>> GetCurrentGoalkeepingByPlayerAsync(string playerRefId);
    Task<Result<Shooting>> GetCurrentShootingByPlayerAsync(string playerRefId);
    Task<Result<IEnumerable<ClubTrendDto>>> GetClubTrendAsync(int clubId, int numberOfSeasons = 5);
}
