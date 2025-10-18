using FSP.Application.DTOs.Football;
using FSP.Domain.Entities.Core;
using System.Collections.Generic;
using System.Linq;

namespace FSP.Application.Mappings;

public interface ISquadStandardMappingService
{
    SquadStandardDto ToSquadStandardDto(SquadStandard squadStandard);
    IEnumerable<SquadStandardDto> ToSquadStandardDtos(IEnumerable<SquadStandard> squadStandards);
}

public class SquadStandardMappingService : ISquadStandardMappingService
{
    public SquadStandardDto ToSquadStandardDto(SquadStandard squadStandard)
    {
        if (squadStandard == null) return new SquadStandardDto();

        return new SquadStandardDto
        {
            Squad = squadStandard.Squad,
            NumberOfPlayers = squadStandard.NumberOfPlayer,
            AverageAge = squadStandard.AverageAge,
            Possession = squadStandard.Possession,
            MatchesPlayed = squadStandard.MatchesPlayed,
            Starts = squadStandard.Starts,
            Minutes = squadStandard.Minutes,
            Nineties = squadStandard.Nineties,
            Goals = squadStandard.Goals,
            Assists = squadStandard.Assists,
            GoalsPlusAssists = squadStandard.GoalsPlusAssists,
            NonPenaltyGoals = squadStandard.GoalsMinusPenaltyKicks,
            PenaltyKicksMade = squadStandard.PenaltyKicks,
            PenaltyKicksAttempted = squadStandard.PenaltyKickAttempts,
            YellowCards = squadStandard.YellowCards,
            RedCards = squadStandard.RedCards,
            ExpectedGoals = squadStandard.ExpectedGoals,
            NonPenaltyExpectedGoals = squadStandard.NonPenaltyExpectedGoals,
            ExpectedAssistedGoals = squadStandard.ExpectedAssistedGoals,
            NonPenaltyExpectedGoalsPlusAssistedGoals = squadStandard.NonPenaltyExpectedGoalsPlusAssistedGoals,
            ProgressiveCarries = squadStandard.ProgressiveCarries,
            ProgressivePasses = squadStandard.ProgressivePasses,
            GoalsPer90 = squadStandard.GoalsPer90,
            AssistsPer90 = squadStandard.AssistsPer90,
            GoalsPlusAssistsPer90 = squadStandard.GoalsPlusAssistsPer90,
            NonPenaltyGoalsPer90 = squadStandard.GoalsMinusPenaltyKicksPer90,
            GoalsPlusAssistsMinusPkPer90 = squadStandard.GoalsPlusAssistsMinusPenaltyKicksPer90,
            ExpectedGoalsPer90 = squadStandard.ExpectedGoalsPer90,
            ExpectedAssistedGoalsPer90 = squadStandard.ExpectedAssistedGoalsPer90,
            ExpectedGoalsPlusAssistedGoalsPer90 = squadStandard.ExpectedGoalsPlusAssistedGoalsPer90,
            NonPenaltyExpectedGoalsPer90 = squadStandard.NonPenaltyExpectedGoalsPer90,
            NonPenaltyExpectedGoalsPlusAssistedGoalsPer90 = squadStandard.NonPenaltyExpectedGoalsPlusAssistedGoalsPer90,
        };
    }

    public IEnumerable<SquadStandardDto> ToSquadStandardDtos(IEnumerable<SquadStandard> squadStandards)
    {
        if (squadStandards == null || !squadStandards.Any()) return Enumerable.Empty<SquadStandardDto>();
        return squadStandards?.Select(ToSquadStandardDto) ?? Enumerable.Empty<SquadStandardDto>();
    }
}
