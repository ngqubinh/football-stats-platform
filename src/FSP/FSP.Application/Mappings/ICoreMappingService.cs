using FSP.Application.DTOs.Core;
using FSP.Domain.Entities.Core;
using FSP.Domain.Interfaces.Core;

namespace FSP.Application.Mappings;

public interface ICoreMappingService
{
    URLInformationDto ToUrlInformationDto(URLInformation url);
    IEnumerable<URLInformationDto> ToUrlInformationDtos(List<URLInformation> urls);
    CompleteTeamDataDto ToCompleteTeamDataDto(CompleteTeamData teamData);
    IEnumerable<CompleteTeamDataDto> ToCompleteTeamDataDtos(List<CompleteTeamData> teamDatas);
}


public class CoreMappingService : ICoreMappingService
{
    private readonly ILeagueMappingService _league;
    private readonly IPlayerMappingService _player;
    private readonly IMatchLogMappingService _matchLog;

    public CoreMappingService()
    {
        this._league = new LeagueMappingService();
        this._player = new PlayerMappingService();
        this._matchLog = new MatchLogMappingService();
    }

    public CompleteTeamDataDto ToCompleteTeamDataDto(CompleteTeamData teamData)
    {
        if (teamData == null) return new CompleteTeamDataDto();

        return new CompleteTeamDataDto
        {
            Players = this._player.ToPlayerDtos(teamData.Players).ToList(),
            Goalkeeping = this._player.ToGoalkeepingDtos(teamData.Goalkeeping).ToList(),
            Shooting = this._player.ToShootingDtos(teamData.Shooting).ToList(),
            MatchLogs = this._matchLog.ToMatchLogDtos(teamData.MatchLogs).ToList(),
            RawHtml = teamData.RawHtml,
            ExtractedAt = teamData.ExtractedAt,
            TeamId = teamData.TeamId,
            TeamName = teamData.TeamName,
            SourceUrl = teamData.SourceUrl,
        };
    }

    public IEnumerable<CompleteTeamDataDto> ToCompleteTeamDataDtos(List<CompleteTeamData> teamDatas)
    {
        if (teamDatas == null || !teamDatas.Any()) return Enumerable.Empty<CompleteTeamDataDto>();
        return teamDatas.Select(ToCompleteTeamDataDto).ToList();
    }

    public URLInformationDto ToUrlInformationDto(URLInformation url)
    {
        return new URLInformationDto
        {
            Label = url.Label,
            URL = url.URL,
            League = this._league.ToLeagueDto(url.League) ?? new DTOs.Football.LeagueDto(),
            StatusCode = url.StatusCode,
            Status = url.Status,
            Season = url.Season
        };
    }

    public IEnumerable<URLInformationDto> ToUrlInformationDtos(List<URLInformation> urls)
    {
        if (!urls.Any() || urls == null) return Enumerable.Empty<URLInformationDto>();
        return urls.Select(this.ToUrlInformationDto).ToList();
    }
}