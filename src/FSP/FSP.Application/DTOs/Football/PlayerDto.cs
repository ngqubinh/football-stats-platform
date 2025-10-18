using System.Text.Json.Serialization;

namespace FSP.Application.DTOs.Football;

public class PlayerDto
{
    [JsonPropertyName("player_id")]
    public int PlayerId { get; set; }

    [JsonPropertyName("player_name")]
    public string PlayerName { get; set; } = string.Empty;

    [JsonPropertyName("nation")]
    public string Nation { get; set; } = string.Empty;

    [JsonPropertyName("position")]
    public string Position { get; set; } = string.Empty;

    [JsonPropertyName("age")]
    public string Age { get; set; } = string.Empty;

    [JsonPropertyName("match_played")]
    public int MatchPlayed { get; set; }

    [JsonPropertyName("starts")]
    public int Starts { get; set; }

    [JsonPropertyName("minutes")]
    public int Minutes { get; set; }

    [JsonPropertyName("ninety_minutes")]
    public string NinetyMinutes { get; set; } = string.Empty;

    [JsonPropertyName("goals")]
    public int Goals { get; set; }

    [JsonPropertyName("assists")]
    public int Assists { get; set; }

    [JsonPropertyName("goals_assists")]
    public int GoalsAssists { get; set; }

    [JsonPropertyName("non_penalty_goals")]
    public int NonPenaltyGoals { get; set; }

    [JsonPropertyName("penalty_kicks_made")]
    public int PenaltyKicksMade { get; set; }

    [JsonPropertyName("penalty_kicks_attempted")]
    public int PenaltyKickAttempted { get; set; }

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

    [JsonPropertyName("progressive_receptions")]
    public int ProgressiveReceptions { get; set; }

    [JsonPropertyName("goals_per_90s")]
    public string GoalsPer90s { get; set; } = string.Empty;

    [JsonPropertyName("assists_per_90s")]
    public string AssistsPer90s { get; set; } = string.Empty;

    [JsonPropertyName("goals_assists_per_90s")]
    public string GoalsAssistsPer90s { get; set; } = string.Empty;

    [JsonPropertyName("non_penalty_goals_per_90s")]
    public string NonPenaltyGoalsPer90s { get; set; } = string.Empty;

    [JsonPropertyName("non_penalty_goals_assists_per_90s")]
    public string NonPenaltyGoalsAssistsPer90s { get; set; } = string.Empty;

    [JsonPropertyName("expected_goals_per_90")]
    public string ExpectedGoalsPer90 { get; set; } = string.Empty;

    [JsonPropertyName("expected_assisted_goals_per_90")]
    public string ExpectedAssistedGoalsPer90 { get; set; } = string.Empty;

    [JsonPropertyName("expected_goals_plus_assisted_goals_per_90")]
    public string ExpectedGoalsPlusAssistedGoalsPer90 { get; set; } = string.Empty;

    [JsonPropertyName("non_penalty_expected_goals_per_90")]
    public string NonPenaltyExpectedGoalsPer90 { get; set; } = string.Empty;

    [JsonPropertyName("non_penalty_expected_goals_plus_assisted_goals_per_90")]
    public string NonPenaltyExpectedGoalsPlusAssistedGoalsPer90 { get; set; } = string.Empty;

    [JsonPropertyName("club_id")]
    public int ClubId { get; set; }

    [JsonPropertyName("club_name")]
    public string ClubName { get; set; } = string.Empty;

    [JsonPropertyName("player_ref_id")]
    public string PlayerRefId { get; set; } = string.Empty;

    [JsonPropertyName("season")]
    public string Season { get; set; } = string.Empty;
}

public class GoalkeepingDto
{
    [JsonPropertyName("goal_keeping_id")]
    public int GoalkeepingId { get; set; }
    [JsonPropertyName("player_name")]
    public string PlayerName { get; set; } = string.Empty;
    [JsonPropertyName("nation")]
    public string Nation { get; set; } = string.Empty;
    [JsonPropertyName("position")]
    public string Position { get; set; } = string.Empty;
    [JsonPropertyName("age")]
    public string Age { get; set; } = string.Empty;
    [JsonPropertyName("match_played")]
    public int MatchPlayed { get; set; }
    [JsonPropertyName("starts")]
    public int Starts { get; set; }
    [JsonPropertyName("minutes")]
    public string Minutes { get; set; } = string.Empty;
    [JsonPropertyName("nineteen_minutes")]
    public string NineteenMinutes { get; set; } = string.Empty;
    [JsonPropertyName("goals_against")]
    public int GoalsAgainst { get; set; }
    [JsonPropertyName("goals_assists_per_90s")]
    public string GoalsAssistsPer90s { get; set; } = string.Empty;
    [JsonPropertyName("shots_on_target_against")]
    public string ShotsOnTargetAgainst { get; set; } = string.Empty;
    [JsonPropertyName("saves")]
    public string Saves { get; set; } = string.Empty;
    [JsonPropertyName("save_percentage")]
    public string SavePercentage { get; set; } = string.Empty;
    [JsonPropertyName("wins")]
    public int Wins { get; set; }
    [JsonPropertyName("draws")]
    public int Draws { get; set; }
    [JsonPropertyName("losses")]
    public int Losses { get; set; }
    [JsonPropertyName("clean_sheets")]
    public int CleanSheets { get; set; }
    [JsonPropertyName("clean_sheets_percentage")]
    public string CleanSheetsPercentage { get; set; } = string.Empty;
    [JsonPropertyName("penalty_kicks_attempted")]
    public string PenaltyKicksAttempted { get; set; } = string.Empty;
    [JsonPropertyName("penalty_kicks_allowed")]
    public string PenaltyKicksAllowed { get; set; } = string.Empty;
    [JsonPropertyName("penalty_kicks_saved")]
    public string PenaltyKicksSaved { get; set; } = string.Empty;
    [JsonPropertyName("penalty_kicks_missed")]
    public string PenaltyKicksMissed { get; set; } = string.Empty;
    [JsonPropertyName("penalty_kicks_saved_percentage")]
    public string PenaltyKicksSavedPercentage { get; set; } = string.Empty;
    [JsonPropertyName("season")]
    public string Season { get; set; } = string.Empty;
    [JsonPropertyName("player_id")]
    public int PlayerId { get; set; }
    [JsonPropertyName("player")]
    public PlayerDto Player { get; set; } = null!;
    [JsonPropertyName("player_ref_id")]
    public string PlayerRefId { get; set; } = string.Empty;
}

public class ShootingDto
{
    [JsonPropertyName("shooting_id")]
    public int ShootingId { get; set; }
    [JsonPropertyName("player_name")]
    public string PlayerName { get; set; } = string.Empty;
    [JsonPropertyName("nation")]
    public string Nation { get; set; } = string.Empty;
    [JsonPropertyName("position")]
    public string Position { get; set; } = string.Empty;
    [JsonPropertyName("age")]
    public string Age { get; set; } = string.Empty;
    [JsonPropertyName("nineteen_minutes")]
    public string NineteenMinutes { get; set; } = string.Empty;
    [JsonPropertyName("goals")]
    public int Goals { get; set; }
    [JsonPropertyName("shots_total")]
    public int ShotsTotal { get; set; }
    [JsonPropertyName("shots_on_target")]
    public int ShotsOnTarget { get; set; }
    [JsonPropertyName("shots_on_target_percentage")]
    public string ShotsOnTargetPercentage { get; set; } = string.Empty;
    [JsonPropertyName("shots_total_per_90")]
    public string ShotsTotalPer90 { get; set; } = string.Empty;
    [JsonPropertyName("shots_on_target_90")]
    public string ShotsOnTargetPer90 { get; set; } = string.Empty;
    [JsonPropertyName("goals_shot")]
    public string GoalsShots { get; set; } = string.Empty;
    [JsonPropertyName("goals_shots_on_target")]
    public string GoalsShotsOnTarget { get; set; } = string.Empty;
    [JsonPropertyName("average_shot_distance")]
    public string AverageShotDistance { get; set; } = string.Empty;
    [JsonPropertyName("penalty_kicks_made")]
    public int PenaltyKicksMade { get; set; }
    [JsonPropertyName("penalty_kicks_attempted")]
    public int PenaltyKicksAttempted { get; set; }
    [JsonPropertyName("season")]
    public string Season { get; set; } = string.Empty;
    [JsonPropertyName("player_id")]
    public int PlayerId { get; set; }
    [JsonPropertyName("player")]
    public PlayerDto Player { get; set; } = null!;
    [JsonPropertyName("player_ref_id")]
    public string PlayerRefId { get; set; } = string.Empty;
}
