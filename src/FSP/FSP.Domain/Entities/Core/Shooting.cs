namespace FSP.Domain.Entities.Core;

public class Shooting
{
    public int ShootingId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public string Nation { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Age { get; set; } = string.Empty;
    public string NineteenMinutes { get; set; } = string.Empty;
    public int Goals { get; set; }
    public int ShotsTotal { get; set; }
    public int ShotsOnTarget { get; set; }
    public string ShotsOnTargetPercentage { get; set; } = string.Empty;
    public string ShotsTotalPer90 { get; set; } = string.Empty;
    public string ShotsOnTargetPer90 { get; set; } = string.Empty;
    public string GoalsShots { get; set; } = string.Empty;
    public string GoalsShotsOnTarget { get; set; } = string.Empty;
    public string AverageShotDistance { get; set; } = string.Empty;
    public int PenaltyKicksMade { get; set; }
    public int PenaltyKicksAttempted { get; set; }
    public string Season { get; set; } = string.Empty; 
    public int PlayerId { get; set; }
    public virtual Player Player { get; set; } = null!;
    public string PlayerRefId { get; set; } = string.Empty;
}
