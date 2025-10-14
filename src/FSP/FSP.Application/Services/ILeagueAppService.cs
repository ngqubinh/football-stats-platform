using FSP.Application.DTOs.Football;
using FSP.Application.Mappings;
using FSP.Domain.Entities;
using FSP.Domain.Entities.Core;
using FSP.Domain.Interfaces.Core;

namespace FSP.Application.Services;

public interface ILeagueAppService
{
    Task<Result<IEnumerable<LeagueDto>>> GetLeaguesAsync();
}

public class LeagueAppService : ILeagueAppService
{
    private readonly IFootballService _football;
    private readonly ILeagueMappingService _leagueMappingService;

    public LeagueAppService(IFootballService football, ILeagueMappingService leagueMappingService)
    {
        this._football = football;
        this._leagueMappingService = leagueMappingService;
    }

    public async Task<Result<IEnumerable<LeagueDto>>> GetLeaguesAsync()
    {
        try
        {
            Result<IEnumerable<League>> domainResult = await this._football.GetAllLeaguesAsync();
            if (!domainResult.Success)
                return Result<IEnumerable<LeagueDto>>.Fail(domainResult.Message!);

            IEnumerable<LeagueDto> leagueDtos = this._leagueMappingService.ToLeagueDtos(domainResult.Data!);
            return Result<IEnumerable<LeagueDto>>.Ok(leagueDtos);
        }
        catch (Exception ex)
        {
            return Result < IEnumerable <LeagueDto>>.Fail($"Error fetching leagues: {ex.Message}.");
        }
    }
}
