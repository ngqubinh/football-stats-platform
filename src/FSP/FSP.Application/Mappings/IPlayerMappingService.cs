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
    PlayerWithScoreDto ToPlayerWithScoreDto(PlayerWithScore playerWithScore);
    IEnumerable<PlayerWithScoreDto> ToPlayerWithScoreDtos(IEnumerable<PlayerWithScore> playerWithScores);
    LineupAnalysisDto ToLineupAnalysisDto(LineupAnalysis lineupAnalysis);
    TwoTeamComparisonDto ToTwoTeamComparisonDto(TwoTeamComparison twoTeamComparison);
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

    public PlayerWithScoreDto ToPlayerWithScoreDto(PlayerWithScore playerWithScore)
    {
        if (playerWithScore == null) return new PlayerWithScoreDto();

        return new PlayerWithScoreDto
        {
            Player = this.ToPlayerDto(playerWithScore.Player),
            Score = playerWithScore.Score,
            PositionCategory = playerWithScore.PositionCategory,
            PrimaryPosition = playerWithScore.PrimaryPosition,
            Rank = playerWithScore.Rank
        };
    }

    public IEnumerable<PlayerWithScoreDto> ToPlayerWithScoreDtos(IEnumerable<PlayerWithScore> playerWithScores)
    {
        if (playerWithScores == null || !playerWithScores.Any())
        {
            return Enumerable.Empty<PlayerWithScoreDto>();
        }

        return playerWithScores.Select(ToPlayerWithScoreDto);
    }

    public LineupAnalysisDto ToLineupAnalysisDto(LineupAnalysis lineupAnalysis)
    {
        if (lineupAnalysis == null) return new LineupAnalysisDto();

        return new LineupAnalysisDto
        {
            LineupPlayers = this.ToPlayerWithScoreDtos(lineupAnalysis.LineupPlayers),
            TotalScore = lineupAnalysis.TotalScore,
            AverageScore = lineupAnalysis.AverageScore,
            Formation = lineupAnalysis.Formation,
            PositionBreakdown = new PositionBreakdownDto
            {
                Goalkeepers = lineupAnalysis.PositionBreakdown.Goalkeepers,
                Defenders = lineupAnalysis.PositionBreakdown.Defenders,
                Midfielders = lineupAnalysis.PositionBreakdown.Midfielders,
                Attackers = lineupAnalysis.PositionBreakdown.Attackers,
                AvgGoalkeeperScore = lineupAnalysis.PositionBreakdown.AvgGoalkeeperScore,
                AvgDefenderScore = lineupAnalysis.PositionBreakdown.AvgDefenderScore,
                AvgMidfielderScore = lineupAnalysis.PositionBreakdown.AvgMidfielderScore,
                AvgAttackerScore = lineupAnalysis.PositionBreakdown.AvgAttackerScore
            },
            Strengths = lineupAnalysis.Strengths,
            Weaknesses = lineupAnalysis.Weaknesses,
            OverallRating = lineupAnalysis.OverallRating
        };
    }

    public TwoTeamComparisonDto ToTwoTeamComparisonDto(TwoTeamComparison twoTeamComparison)
    {
        if (twoTeamComparison == null) return new TwoTeamComparisonDto();

        return new TwoTeamComparisonDto
        {
            TeamAAnalysis = this.ToLineupAnalysisDto(twoTeamComparison.TeamAAnalysis),
            TeamBAnalysis = this.ToLineupAnalysisDto(twoTeamComparison.TeamBAnalysis),
            Comparison = new TeamComparisonDto
            {
                TotalScoreAdvantage = twoTeamComparison.Comparison.TotalScoreAdvantage,
                AverageScoreAdvantage = twoTeamComparison.Comparison.AverageScoreAdvantage,
                AdvantageTeam = twoTeamComparison.Comparison.AdvantageTeam,
                AdvantagePercentage = twoTeamComparison.Comparison.AdvantagePercentage,
                PositionAdvantages = twoTeamComparison.Comparison.PositionAdvantages.Select(pa => new PositionAdvantageDto
                {
                    Position = pa.Position,
                    AdvantageTeam = pa.AdvantageTeam,
                    TeamAScore = pa.TeamAScore,
                    TeamBScore = pa.TeamBScore,
                    AdvantageMargin = pa.AdvantageMargin
                }).ToList()
            },
            Prediction = new MatchPredictionDto
            {
                PredictedWinner = twoTeamComparison.Prediction.PredictedWinner,
                Confidence = twoTeamComparison.Prediction.Confidence,
                PredictedScore = twoTeamComparison.Prediction.PredictedScore,
                KeyFactors = twoTeamComparison.Prediction.KeyFactors
            },
            KeyBattles = twoTeamComparison.KeyBattles.Select(b => new PlayerBattleDto
            {
                Position = b.Position,
                TeamAPlayer = b.TeamAPlayer != null ? this.ToPlayerWithScoreDto(b.TeamAPlayer) : null,
                TeamBPlayer = b.TeamBPlayer != null ? this.ToPlayerWithScoreDto(b.TeamBPlayer) : null,
                Advantage = b.Advantage,
                AdvantageMargin = b.AdvantageMargin
            }).ToList()
        };
    }
}
