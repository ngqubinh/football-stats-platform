namespace FSP.Domain.Entities.Core;

public class Club
{
    public int ClubId { get; set; }
    public string ClubName { get; set; } = string.Empty;
    public string Nation { get; set; } = string.Empty;
    public int LeagueId { get; set; }
    public League League { get; set; } = null!;
    public ICollection<Player> Players { get; set; } = new List<Player>();
}