namespace FSP.Infrastructure.Data;

public class TeamTableIds
{
    public string StandardStats { get; set; } = string.Empty;
    public string Goalkeeping { get; set; } = string.Empty;
    public string Shooting { get; set; } = string.Empty;
    public string MatchLog { get; set; } = string.Empty;

    public TeamTableIds() { }

    public TeamTableIds(string standardStats, string goalkeeping, string shooting, string matchLog)
    {
        this.StandardStats = standardStats;
        this.Goalkeeping = goalkeeping;
        this.Shooting = shooting;
        this.MatchLog = matchLog;
    }

    public static readonly TeamTableIds DefaultPremierLeague = new TeamTableIds("9", "9", "9", "9");
    public static readonly TeamTableIds DefaultEuropaLeague = new TeamTableIds("50", "50", "50", "50");
    public static readonly TeamTableIds DefaultRomaniaLiga1 = new TeamTableIds("47", "47", "47", "47");
}

public static class TeamTableMappings
{
    public static TeamTableIds GetTableIds(string teamLabel, string leagueName)
    {
        return leagueName switch
        {
            "Premier League" => new TeamTableIds("9", "9", "9", "9"),
            "Europa League" => new TeamTableIds("50", "50", "50", "50"),
            _ => new TeamTableIds("0", "0", "0", "0")
        };
    }
}