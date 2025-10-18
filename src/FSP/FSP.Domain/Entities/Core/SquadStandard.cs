namespace FSP.Domain.Entities.Core;

public class SquadStandard
{
    public string Squad { get; set; } = string.Empty;
    public int NumberOfPlayer { get; set; }
    public float AverageAge { get; set; }
    public float Possession { get; set; }
    public int MatchesPlayed { get; set; }
    public int Starts { get; set; }
    public int Minutes { get; set; }
    public float Nineties { get; set; }
    public int Goals { get; set; }
    public int Assists { get; set; }
    public int GoalsPlusAssists { get; set; }
    public int GoalsMinusPenaltyKicks { get; set; }
    public int PenaltyKicks { get; set; }
    public int PenaltyKickAttempts { get; set; }
    public int YellowCards { get; set; }
    public int RedCards { get; set; }
    public float ExpectedGoals { get; set; }
    public float NonPenaltyExpectedGoals { get; set; }
    public float ExpectedAssistedGoals { get; set; }
    public float NonPenaltyExpectedGoalsPlusAssistedGoals { get; set; }
    public int ProgressiveCarries { get; set; }
    public int ProgressivePasses { get; set; }
    public float GoalsPer90 { get; set; }
    public float AssistsPer90 { get; set; }
    public float GoalsPlusAssistsPer90 { get; set; }
    public float GoalsMinusPenaltyKicksPer90 { get; set; }
    public float GoalsPlusAssistsMinusPenaltyKicksPer90 { get; set; }
    public float ExpectedGoalsPer90 { get; set; }
    public float ExpectedAssistedGoalsPer90 { get; set; }
    public float ExpectedGoalsPlusAssistedGoalsPer90 { get; set; }
    public float NonPenaltyExpectedGoalsPer90 { get; set; }
    public float NonPenaltyExpectedGoalsPlusAssistedGoalsPer90 { get; set; }
}
