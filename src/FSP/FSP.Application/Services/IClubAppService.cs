using FSP.Application.DTOs.Football;
using FSP.Application.Mappings;
using FSP.Domain.Entities;
using FSP.Domain.Entities.Core;
using FSP.Domain.Interfaces.Core;

namespace FSP.Application.Services;

public interface IClubAppService
{
    Task<Result<IEnumerable<ClubDto>>> GetClubsByLeagueAsync(int leagueId);
    Task<Result<IEnumerable<ClubTrendDto>>> GetClubTrendAsync(int clubId, int numberOfSeasons = 5);
}

public class ClubAppService : IClubAppService
{
    private readonly IFootballService _football;
    private readonly IClubMappingService _clubMappingService;

    public ClubAppService(IFootballService football, IClubMappingService clubMappingService)
    {
        this._football = football;
        this._clubMappingService = clubMappingService;
    }

    public async Task<Result<IEnumerable<ClubDto>>> GetClubsByLeagueAsync(int leagueId)
    {
        try
        {
            Result<IEnumerable<Club>> domainResult = await this._football.GetClubsByLeagueAsync(leagueId);
            if (!domainResult.Success)
                return Result<IEnumerable<ClubDto>>.Fail(domainResult.Message!);

            IEnumerable<ClubDto> clubDtos = this._clubMappingService.ToClubDtos(domainResult.Data!);
            return Result<IEnumerable<ClubDto>>.Ok(clubDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<ClubDto>>.Fail($"Error fetching clubs for league ID {leagueId}: {ex.Message}.");
        }
    }

    public async Task<Result<IEnumerable<ClubTrendDto>>> GetClubTrendAsync(int clubId, int numberOfSeasons = 5)
    {
        try
        {
            var domainResult = await this._football.GetClubTrendAsync(clubId, numberOfSeasons);
            if (!domainResult.Success)
                return Result<IEnumerable<ClubTrendDto>>.Fail(domainResult.Message!);
            return Result<IEnumerable<ClubTrendDto>>.Ok(domainResult.Data!);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<ClubTrendDto>>.Fail($"Error fetching club trend for Club ID {clubId}: {ex.Message}.");
        }
    }
}