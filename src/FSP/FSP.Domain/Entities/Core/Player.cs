namespace FSP.Domain.Entities.Core;

public class Player
{
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public string Nation { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Age { get; set; } = string.Empty;
    public int MatchPlayed { get; set; }
    public int Starts { get; set; }
    public int Minutes { get; set; }
    public string NineteenMinutes { get; set; } = string.Empty;
    public int Goals { get; set; }
    public int Assists { get; set; }
    public int GoalsAssists { get; set; }
    public int NonPenaltyGoals { get; set; }
    public int PenaltyKicksMade { get; set; }
    public int PenaltyKickAttempted { get; set; }
    public int YellowCards { get; set; }
    public int RedCards { get; set; }
    public string GoalsPer90s { get; set; } = string.Empty;
    public string AssistsPer90s { get; set; } = string.Empty;
    public string GoalsAssistsPer90s { get; set; } = string.Empty;
    public string NonPenaltyGoalsPer90s { get; set; } = string.Empty;
    public string NonPenaltyGoalsAssistsPer90s { get; set; } = string.Empty;

    public string PlayerRefId { get; set; } = string.Empty;
    public string Season { get; set; } = string.Empty;

    public int ClubId { get; set; }
    public Club Club { get; set; } = null!;
    public Goalkeeping? Goalkeeping { get; set; }
    public Shooting? Shooting { get; set; }
    
    public PlayerDetails? PlayerDetails { get; set; }
}

public class PlayerDetails
{
    public int PlayerDetailId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string OriginalName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Born { get; set; } = string.Empty;
    public string Citizenship { get; set; } = string.Empty;
    public string Club { get; set; } = string.Empty;
    public string PlayerRefId { get; set; } = string.Empty;
    public int PlayerId { get; set; }
    public Player Player { get; set; } = null!;
}