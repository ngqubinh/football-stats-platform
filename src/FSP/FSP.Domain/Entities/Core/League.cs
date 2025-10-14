namespace FSP.Domain.Entities.Core;

public class League
{
    public int LeagueId { get; set; }
    public string LeagueName { get; set; } = string.Empty;
    public string Nation { get; set; } = string.Empty;

    public ICollection<Club> Clubs { get; set; } = new List<Club>();
}
