using FSP.Application.DTOs.Football;
using FSP.Domain.Entities.Core;

namespace FSP.Application.Mappings;

public interface IPlayerMappingService
{
    PlayerDto ToPlayerDto(Player player);
    IEnumerable<PlayerDto> ToPlayerDtos(IEnumerable<Player> players);
    GoalkeepingDto ToGoalkeepingDto(Goalkeeping goalkeeping);
    IEnumerable<GoalkeepingDto> ToGoalkeepingDtos(IEnumerable<Goalkeeping> goalkeepings);
    ShootingDto ToShootingDto(Shooting shooting);
    IEnumerable<ShootingDto> ToShootingDtos(IEnumerable<Shooting> shootings);
}

public class PlayerMappingService : IPlayerMappingService
{
    public GoalkeepingDto ToGoalkeepingDto(Goalkeeping goalkeeping)
    {
        return new GoalkeepingDto
        {
            GoalkeepingId = goalkeeping.GoalkeepingId,
            PlayerName = goalkeeping.PlayerName,
            Nation = goalkeeping.Nation,
            Position = goalkeeping.Position,
            Age = goalkeeping.Age,
            MatchPlayed = goalkeeping.MatchPlayed,
            Starts = goalkeeping.Starts,
            Minutes = goalkeeping.Minutes,
            NineteenMinutes = goalkeeping.NineteenMinutes,
            GoalsAgainst = goalkeeping.GoalsAgainst,
            GoalsAssistsPer90s = goalkeeping.GoalsAssistsPer90s,
            ShotsOnTargetAgainst = goalkeeping.ShotsOnTargetAgainst,
            Saves = goalkeeping.Saves,
            SavePercentage = goalkeeping.SavePercentage,
            Wins = goalkeeping.Wins,
            Draws = goalkeeping.Draws,
            Losses = goalkeeping.Losses,
            CleanSheets = goalkeeping.CleanSheets,
            CleanSheetsPercentage = goalkeeping.CleanSheetsPercentage,
            PenaltyKicksAttempted = goalkeeping.PenaltyKicksAttempted,
            PenaltyKicksAllowed = goalkeeping.PenaltyKicksAllowed,
            PenaltyKicksSaved = goalkeeping.PenaltyKicksSaved,
            PenaltyKicksMissed = goalkeeping.PenaltyKicksMissed,
            PenaltyKicksSavedPercentage = goalkeeping.PenaltyKicksSavedPercentage,
            Season = goalkeeping.Season,
            PlayerId = goalkeeping.PlayerId,
            Player = this.ToPlayerDto(goalkeeping.Player),
            PlayerRefId = goalkeeping.PlayerRefId,
        };
    }

    public IEnumerable<GoalkeepingDto> ToGoalkeepingDtos(IEnumerable<Goalkeeping> goalkeepings)
    {
        if (goalkeepings == null || !goalkeepings.Any())
        {
            return Enumerable.Empty<GoalkeepingDto>();
        }

        return goalkeepings.Select(ToGoalkeepingDto);
    }

    public PlayerDto ToPlayerDto(Player player)
    {
        if (player == null) return new PlayerDto();

        return new PlayerDto
        {
            PlayerId = player.PlayerId,
            PlayerName = player.PlayerName,
            Nation = player.Nation,
            Position = player.Position,
            Age = player.Age,
            MatchPlayed = player.MatchPlayed,
            Starts = player.Starts,
            Minutes = player.Minutes,
            NinetyMinutes = player.NineteenMinutes,
            Goals = player.Goals,
            Assists = player.Assists,
            GoalsAssists = player.GoalsAssists,
            NonPenaltyGoals = player.NonPenaltyGoals,
            PenaltyKicksMade = player.PenaltyKicksMade,
            PenaltyKickAttempted = player.PenaltyKickAttempted,
            YellowCards = player.YellowCards,
            RedCards = player.RedCards,
            ExpectedGoals = player.ExpectedGoals,
            NonPenaltyExpectedGoals = player.NonPenaltyExpectedGoals,
            ExpectedAssistedGoals = player.ExpectedAssistedGoals,
            NonPenaltyExpectedGoalsPlusAssistedGoals = player.NonPenaltyExpectedGoalsPlusAssistedGoals,
            ProgressiveCarries = player.ProgressiveCarries,
            ProgressivePasses = player.ProgressivePasses,
            ProgressiveReceptions = player.ProgressiveReceptions,
            GoalsPer90s = player.GoalsPer90s,
            AssistsPer90s = player.AssistsPer90s,
            GoalsAssistsPer90s = player.GoalsAssistsPer90s,
            NonPenaltyGoalsPer90s = player.NonPenaltyGoalsPer90s,
            NonPenaltyGoalsAssistsPer90s = player.NonPenaltyGoalsAssistsPer90s,
            ExpectedGoalsPer90 = player.ExpectedGoalsPer90,
            ExpectedAssistedGoalsPer90 = player.ExpectedAssistedGoalsPer90,
            ExpectedGoalsPlusAssistedGoalsPer90 = player.ExpectedGoalsPlusAssistedGoalsPer90,
            NonPenaltyExpectedGoalsPer90 = player.NonPenaltyExpectedGoalsPer90,
            NonPenaltyExpectedGoalsPlusAssistedGoalsPer90 = player.NonPenaltyExpectedGoalsPlusAssistedGoalsPer90,
            ClubId = player.ClubId,
            ClubName = player.Club?.ClubName ?? string.Empty,
            PlayerRefId = player.PlayerRefId,
            Season = player.Season,
        };
    }

    public IEnumerable<PlayerDto> ToPlayerDtos(IEnumerable<Player> players)
    {
        if (players == null || !players.Any()) return Enumerable.Empty<PlayerDto>();
        return players.Select(this.ToPlayerDto);
    }

    public ShootingDto ToShootingDto(Shooting shooting)
    {
        return new ShootingDto
        {
            ShootingId = shooting.ShootingId,
            PlayerName = shooting.PlayerName,
            Nation = shooting.Nation,
            Position = shooting.Position,
            Age = shooting.Age,
            NineteenMinutes = shooting.NineteenMinutes,
            Goals = shooting.Goals,
            ShotsTotal = shooting.ShotsTotal,
            ShotsOnTarget = shooting.ShotsOnTarget,
            ShotsOnTargetPercentage = shooting.ShotsOnTargetPercentage,
            ShotsTotalPer90 = shooting.ShotsTotalPer90,
            ShotsOnTargetPer90 = shooting.ShotsOnTargetPer90,
            GoalsShots = shooting.GoalsShots,
            GoalsShotsOnTarget = shooting.GoalsShotsOnTarget,
            AverageShotDistance = shooting.AverageShotDistance,
            PenaltyKicksMade = shooting.PenaltyKicksMade,
            PenaltyKicksAttempted = shooting.PenaltyKicksAttempted,
            Season = shooting.Season,
            PlayerId = shooting.PlayerId,
            Player = this.ToPlayerDto(shooting.Player),
            PlayerRefId = shooting.PlayerRefId,
        };
    }

    public IEnumerable<ShootingDto> ToShootingDtos(IEnumerable<Shooting> shootings)
    {
        if (shootings == null || !shootings.Any())
        {
            return Enumerable.Empty<ShootingDto>();
        }

        return shootings.Select(ToShootingDto);
    }
}
