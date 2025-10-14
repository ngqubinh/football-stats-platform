using FSP.Domain.Entities.Core;

namespace FSP.Infrastructure.Data;

public enum StaticNation
{
    England, Romania, EU
}

public static class StaticLeague
{
    public const string PremierLeague = "Premier League";
    public const string EuropaLeague = "Europa League";
    public const string Liga1 = "Liga 1";

    public static readonly List<(string LeagueName, string Nation)> SystemLeagues = new()
    {
        (PremierLeague, StaticNation.England.ToString()),
        (EuropaLeague, StaticNation.EU.ToString()),
        (Liga1, StaticNation.Romania.ToString())
    };

    public static bool IsSystemLeague(string leagueName, string nation)
    {
        return SystemLeagues.Any(x => 
            x.LeagueName.Equals(leagueName, StringComparison.OrdinalIgnoreCase) &&
            x.Nation.Equals(nation, StringComparison.OrdinalIgnoreCase));
    }

    public static bool IsSystemLeagueName(string leagueName)
    {
        return SystemLeagues.Any(x => 
            x.LeagueName.Equals(leagueName, StringComparison.OrdinalIgnoreCase));
    }
}

public class SeasonUrl
{
    public string Season { get; set; } = string.Empty;
    public string URL { get; set; } = string.Empty;
}

public class PlayerUrl
{
    public string URL { get; set; } = string.Empty;
}

public class FbrefTag
{
    public string Label { get; set; } = string.Empty;
    public List<SeasonUrl> SeasonUrls { get; set; } = new List<SeasonUrl>();
    public League League { get; set; } = new League();
    public TeamTableIds TableIds { get; set; } = new TeamTableIds();
}

public class FbrefPlayerTag
{
    public List<PlayerUrl> PlayerUrls { get; set; } = new List<PlayerUrl>();
    public PlayerDetails PlayerDetails { get; set; } = new PlayerDetails();
}

public static class PremierLeagueURLS
{
    public static readonly List<FbrefTag> Urls = new()
    {
        new FbrefTag
        {
            Label = "Arsenal",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/18bb7c10/Arsenal-Stats" },
                new SeasonUrl { Season = "2024-2025", URL = "https://fbref.com/en/squads/18bb7c10/2024-2025/Arsenal-Stats" },
                new SeasonUrl { Season = "2023-2024", URL = "https://fbref.com/en/squads/18bb7c10/2023-2024/Arsenal-Stats" },
                new SeasonUrl { Season = "2022-2023", URL = "https://fbref.com/en/squads/18bb7c10/2022-2023/Arsenal-Stats" },
                new SeasonUrl { Season = "2021-2022", URL = "https://fbref.com/en/squads/18bb7c10/2021-2022/Arsenal-Stats" }
            },
            League = new League
            {
                LeagueName = StaticLeague.PremierLeague,
                Nation = StaticNation.England.ToString()
            },
            TableIds = TeamTableIds.DefaultPremierLeague
        },
    };
}

public static class RomaniaLiga1URLS
{
    public static readonly List<FbrefTag> Urls = new()
    {
        new FbrefTag
        {
            Label = "FC Metaloglobus București",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl{ Season = "2025-2026", URL = "https://fbref.com/en/squads/defd54ac/FC-Metaloglobus-Bucuresti-Stats"},
            },
            League = new League
            {
                LeagueName = StaticLeague.Liga1,
                Nation = StaticNation.Romania.ToString()
            },
            TableIds = TeamTableIds.DefaultRomaniaLiga1
        },
        new FbrefTag
        {
            Label = "Oțelul Galați",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl{ Season = "2025-2026", URL = "https://fbref.com/en/squads/86edb46e/Otelul-Galati-Stats"},
                new SeasonUrl{ Season = "2024-2025", URL = "https://fbref.com/en/squads/86edb46e/2024-2025/Otelul-Galati-Stats"},
                new SeasonUrl{ Season = "2023-2024", URL = "https://fbref.com/en/squads/86edb46e/2023-2024/Otelul-Galati-Stats"},
            },
            League = new League
            {
                LeagueName = StaticLeague.Liga1,
                Nation = StaticNation.Romania.ToString()
            },
            TableIds = TeamTableIds.DefaultRomaniaLiga1
        }
    };

    public static readonly List<FbrefPlayerTag> PlayerUrls = new()
    {
        new FbrefPlayerTag
        {
            PlayerUrls = new List<PlayerUrl>
            {
                new PlayerUrl { URL = "https://fbref.com/en/players/bc1f0975/Milen-Zhelev" },
            },
            PlayerDetails = new PlayerDetails { Club = "Oțelul Galați" }
        },
    };
}