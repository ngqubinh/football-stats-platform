using System.Text.Json.Serialization;

namespace FSP.Application.DTOs.Football;

public class SquadStandardDto
{
    [JsonPropertyName("squad")]
    public string Squad { get; set; } = string.Empty;

    [JsonPropertyName("number_of_players")]
    public int NumberOfPlayers { get; set; }

    [JsonPropertyName("average_age")]
    public float AverageAge { get; set; }

    [JsonPropertyName("possession")]
    public float Possession { get; set; }

    [JsonPropertyName("matches_played")]
    public int MatchesPlayed { get; set; }

    [JsonPropertyName("starts")]
    public int Starts { get; set; }

    [JsonPropertyName("minutes")]
    public int Minutes { get; set; }

    [JsonPropertyName("nineties")]
    public float Nineties { get; set; }

    [JsonPropertyName("goals")]
    public int Goals { get; set; }

    [JsonPropertyName("assists")]
    public int Assists { get; set; }

    [JsonPropertyName("goals_plus_assists")]
    public int GoalsPlusAssists { get; set; }

    [JsonPropertyName("non_penalty_goals")]
    public int NonPenaltyGoals { get; set; }

    [JsonPropertyName("penalty_kicks_made")]
    public int PenaltyKicksMade { get; set; }

    [JsonPropertyName("penalty_kicks_attempted")]
    public int PenaltyKicksAttempted { get; set; }

    [JsonPropertyName("yellow_cards")]
    public int YellowCards { get; set; }

    [JsonPropertyName("red_cards")]
    public int RedCards { get; set; }

    [JsonPropertyName("expected_goals")]
    public float ExpectedGoals { get; set; }

    [JsonPropertyName("non_penalty_expected_goals")]
    public float NonPenaltyExpectedGoals { get; set; }

    [JsonPropertyName("expected_assisted_goals")]
    public float ExpectedAssistedGoals { get; set; }

    [JsonPropertyName("non_penalty_expected_goals_plus_assisted_goals")]
    public float NonPenaltyExpectedGoalsPlusAssistedGoals { get; set; }

    [JsonPropertyName("progressive_carries")]
    public int ProgressiveCarries { get; set; }

    [JsonPropertyName("progressive_passes")]
    public int ProgressivePasses { get; set; }

    [JsonPropertyName("goals_per_90")]
    public float GoalsPer90 { get; set; }

    [JsonPropertyName("assists_per_90")]
    public float AssistsPer90 { get; set; }

    [JsonPropertyName("goals_plus_assists_per_90")]
    public float GoalsPlusAssistsPer90 { get; set; }

    [JsonPropertyName("non_penalty_goals_per_90")]
    public float NonPenaltyGoalsPer90 { get; set; }

    [JsonPropertyName("goals_plus_assists_minus_pk_per_90")]
    public float GoalsPlusAssistsMinusPkPer90 { get; set; }

    [JsonPropertyName("expected_goals_per_90")]
    public float ExpectedGoalsPer90 { get; set; }

    [JsonPropertyName("expected_assisted_goals_per_90")]
    public float ExpectedAssistedGoalsPer90 { get; set; }

    [JsonPropertyName("expected_goals_plus_assisted_goals_per_90")]
    public float ExpectedGoalsPlusAssistedGoalsPer90 { get; set; }

    [JsonPropertyName("non_penalty_expected_goals_per_90")]
    public float NonPenaltyExpectedGoalsPer90 { get; set; }

    [JsonPropertyName("non_penalty_expected_goals_plus_assisted_goals_per_90")]
    public float NonPenaltyExpectedGoalsPlusAssistedGoalsPer90 { get; set; }
}
