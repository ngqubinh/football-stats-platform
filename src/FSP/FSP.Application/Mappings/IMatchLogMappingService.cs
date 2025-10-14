using FSP.Application.DTOs.Football;
using FSP.Domain.Entities.Core;

namespace FSP.Application.Mappings;

public interface IMatchLogMappingService
{
    MatchLogDto ToMatchLogDto(MatchLog matchLog);
    IEnumerable<MatchLogDto> ToMatchLogDtos(IEnumerable<MatchLog> matchLogs);
}

public class MatchLogMappingService : IMatchLogMappingService
{
    public MatchLogDto ToMatchLogDto(MatchLog matchLog)
    {
        if (matchLog == null) return new MatchLogDto();

        return new MatchLogDto
        {
            Date = matchLog.Date,
            Time = matchLog.Time,
            Competition = matchLog.Competition,
            Round = matchLog.Round,
            Day = matchLog.Day,
            Venue = matchLog.Venue,
            Result = matchLog.Result,
            GoalsFor = matchLog.GoalsFor,
            GoalsAgainst = matchLog.GoalsAgainst,
            Opponent = matchLog.Opponent,
            Possession = matchLog.Possession,
            Attendance = matchLog.Attendance,
            Captain = matchLog.Captain,
            Formation = matchLog.Formation,
            OppFormation = matchLog.OppFormation,
            Referee = matchLog.Referee
        };
    }

    public IEnumerable<MatchLogDto> ToMatchLogDtos(IEnumerable<MatchLog> matchLogs)
    {
        if (matchLogs == null || !matchLogs.Any())
        {
            return Enumerable.Empty<MatchLogDto>();
        }

        return matchLogs.Select(ToMatchLogDto);
    }
}