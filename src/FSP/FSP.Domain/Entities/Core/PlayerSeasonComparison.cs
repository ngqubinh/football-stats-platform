namespace FSP.Domain.Entities.Core;

public class PlayerSeasonComparison
{
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public string CurrentSeason { get; set; } = string.Empty;
    public string PreviousSeason { get; set; } = string.Empty;

    // Current Season Stats
    public int CurrentGoals { get; set; }
    public int CurrentAssists { get; set; }
    public int CurrentAppearances { get; set; }
    public int CurrentMinutesPlayed { get; set; }

    // Previous Season Stats
    public int PreviousGoals { get; set; }
    public int PreviousAssists { get; set; }
    public int PreviousAppearances { get; set; }
    public int PreviousMinutesPlayed { get; set; }

    // Differences
    public int GoalsDifference { get; set; }
    public int AssistsDifference { get; set; }
    public int AppearancesDifference { get; set; }

    // Percentage Changes
    public double GoalsChangePercentage { get; set; }
    public double AssistsChangePercentage { get; set; }
    public double AppearancesChangePercentage { get; set; }

    // Performance per 90 minutes
    public double CurrentGoalsPer90 { get; set; }
    public double PreviousGoalsPer90 { get; set; }
    public double GoalsPer90Difference { get; set; }

    public string PerformanceTrend { get; set; } = string.Empty;
}

public class ClubTrendDto
{
    public string Season { get; set; } = string.Empty;
    public int TotalGoals { get; set; }
    public int TotalGoalsAgainst { get; set; }
    public int TotalAssists { get; set; }
}