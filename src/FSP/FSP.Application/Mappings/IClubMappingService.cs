using FSP.Application.DTOs.Football;
using FSP.Domain.Entities.Core;

namespace FSP.Application.Mappings;

public interface IClubMappingService
{
    ClubDto ToClubDto(Club club);
    IEnumerable<ClubDto> ToClubDtos(IEnumerable<Club> clubs);
}

public class ClubMappingService : IClubMappingService
{
    public ClubDto ToClubDto(Club club)
    {
        return new ClubDto
        {
            ClubId = club.ClubId,
            ClubName = club.ClubName,
            Nation = club.Nation,
            LeagueId = club.LeagueId,
            LeagueName = club.League?.LeagueName ?? string.Empty
        };
    }

    public IEnumerable<ClubDto> ToClubDtos(IEnumerable<Club> clubs)
    {
        if (clubs == null || !clubs.Any()) return Enumerable.Empty<ClubDto>();
        return clubs.Select(this.ToClubDto);
    }
}