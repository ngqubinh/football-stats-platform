namespace FSP.Domain.Entities.Core;

public class Goalkeeping
{
    public int GoalkeepingId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public string Nation { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Age { get; set; } = string.Empty;
    public int MatchPlayed { get; set; }
    public int Starts { get; set; }
    public string Minutes { get; set; } = string.Empty;
    public string NineteenMinutes { get; set; } = string.Empty;
    public int GoalsAgainst { get; set; }
    public string GoalsAssistsPer90s { get; set; } = string.Empty;
    public string ShotsOnTargetAgainst { get; set; } = string.Empty;
    public string Saves { get; set; } = string.Empty;
    public string SavePercentage { get; set; } = string.Empty;
    public int Wins { get; set; }
    public int Draws { get; set; }
    public int Losses { get; set; }
    public int CleanSheets { get; set; }
    public string CleanSheetsPercentage { get; set; } = string.Empty;
    public string PenaltyKicksAttempted { get; set; } = string.Empty;
    public string PenaltyKicksAllowed { get; set; } = string.Empty;
    public string PenaltyKicksSaved { get; set; } = string.Empty;
    public string PenaltyKicksMissed { get; set; } = string.Empty;
    public string PenaltyKicksSavedPercentage { get; set; } = string.Empty;
    public string Season { get; set; } = string.Empty; 
    public int PlayerId { get; set; }
    public virtual Player Player { get; set; } = null!;
    public string PlayerRefId { get; set; } = string.Empty;
}
