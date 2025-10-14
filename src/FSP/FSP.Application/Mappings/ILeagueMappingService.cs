using FSP.Application.DTOs.Football;
using FSP.Domain.Entities.Core;

namespace FSP.Application.Mappings;

public interface ILeagueMappingService
{
    LeagueDto ToLeagueDto(League league);
    IEnumerable<LeagueDto> ToLeagueDtos(IEnumerable<League> leagues);
}

public class LeagueMappingService : ILeagueMappingService
{
    public LeagueDto ToLeagueDto(League league)
    {
        return new LeagueDto
        {
            LeagueId = league.LeagueId,
            LeagueName = league.LeagueName,
            Nation = league.Nation,
        };
    }

    public IEnumerable<LeagueDto> ToLeagueDtos(IEnumerable<League> leagues)
    {
        if (leagues == null || !leagues.Any()) return Enumerable.Empty<LeagueDto>();
        return leagues.Select(this.ToLeagueDto);
    }
}