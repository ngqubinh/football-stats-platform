using FSP.Domain.Entities.Core;

namespace FSP.Infrastructure.Data;

public enum StaticNation
{
    England, Romania, EU,
    Turkey, Chinese, Germany,
    Italy, France, Spain,
    Netherlands, Australia, Sweden,
    Argentina, USA, Brazil,
    Ecuador, Colombia, Chile,
    SaudiArabia, Mexico, Korea
}

public static class StaticLeague
{
    public const string BrazilSerieABetano = "Brazil SerieA Betano";
    public const string PremierLeague = "Premier League";
    public const string EnglandChampionship = "England Championship";
    public const string Liga1 = "Liga 1";
    public const string SuperLig = "Turkey Super Lig";
    public const string ChineseSuperLeague = "Chinese Super League";
    public const string GermanyBundeslia1 = "Germany Bundeslia 1";
    public const string ItalySerieA = "Italy Serie A";
    public const string FranceLigue1 = "France Ligue 1";
    public const string FranceLigue2 = "France League 2";
    public const string SpainLaliga1 = "Spain Laliga 1";
    public const string NetherlandsEredivisie = "Netherlands Eredivisie";
    public const string AustraliaALeague = "Australia A League";
    public const string SwedenAllsvenskan = "Sweden Allsvenskan";
    public const string ArgentinaTorneoBetano = "Argentina Torneo Betano";
    public const string USAMLS = "USA MLS";
    public const string EcuadorLigaPro = "Ecuador Liga Pro";
    public const string ColombiPrimeraA = "Colombia Primera";
    public const string ChileLigaDePrimera = "Chile Liga De Primera";
    public const string SaudiArabiaProfessionalLeague = "Saudi Arabia Professional League";
    public const string MexicoLigaMX = "Mexico Liga MX";
    public const string KoreaKLeague1 = "Korea K League 1";

    public static readonly List<(string LeagueName, string Nation)> SystemLeagues = new()
    {
        (BrazilSerieABetano, StaticNation.Brazil.ToString()),
        (PremierLeague, StaticNation.England.ToString()),
        (EnglandChampionship, StaticNation.England.ToString()),
        (Liga1, StaticNation.Romania.ToString()),
        (SuperLig, StaticNation.Turkey.ToString()),
        (ChineseSuperLeague, StaticNation.Chinese.ToString()),
        (GermanyBundeslia1, StaticNation.Germany.ToString()),
        (ItalySerieA, StaticNation.Italy.ToString()),
        (FranceLigue1, StaticNation.France.ToString()),
        (FranceLigue2, StaticNation.France.ToString()),
        (SpainLaliga1, StaticNation.Spain.ToString()),
        (NetherlandsEredivisie, StaticNation.Netherlands.ToString()),
        (AustraliaALeague, StaticNation.Australia.ToString()),
        (SwedenAllsvenskan, StaticNation.Sweden.ToString()),
        (ArgentinaTorneoBetano, StaticNation.Argentina.ToString()),
        (USAMLS, StaticNation.USA.ToString()),
        (EcuadorLigaPro, StaticNation.Ecuador.ToString()),
        (ColombiPrimeraA, StaticNation.Colombia.ToString()),
        (ChileLigaDePrimera, StaticNation.Chile.ToString()),
        (SaudiArabiaProfessionalLeague, StaticNation.SaudiArabia.ToString()),
        (MexicoLigaMX, StaticNation.Mexico.ToString())
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
        // new FbrefTag
        // {
        //     Label = "Arsenal",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/18bb7c10/Arsenal-Stats" },
        //         // new SeasonUrl { Season = "2024-2025", URL = "https://fbref.com/en/squads/18bb7c10/2024-2025/Arsenal-Stats" },
        //         // new SeasonUrl { Season = "2023-2024", URL = "https://fbref.com/en/squads/18bb7c10/2023-2024/Arsenal-Stats" },
        //         // new SeasonUrl { Season = "2022-2023", URL = "https://fbref.com/en/squads/18bb7c10/2022-2023/Arsenal-Stats" },
        //         // new SeasonUrl { Season = "2021-2022", URL = "https://fbref.com/en/squads/18bb7c10/2021-2022/Arsenal-Stats" }
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.PremierLeague,
        //         Nation = StaticNation.England.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultPremierLeague
        // },

        // new FbrefTag
        // {
        //     Label = "Liverpool",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/822bd0ba/Liverpool-Stats" },
        //         new SeasonUrl { Season = "2024-2025", URL = "https://fbref.com/en/squads/822bd0ba/2024-2025/Liverpool-Stats" },
        //         new SeasonUrl { Season = "2023-2024", URL = "https://fbref.com/en/squads/822bd0ba/2023-2024/Liverpool-Stats" },
        //         new SeasonUrl { Season = "2022-2023", URL = "https://fbref.com/en/squads/822bd0ba/2022-2023/Liverpool-Stats" },
        //         new SeasonUrl { Season = "2021-2022", URL = "https://fbref.com/en/squads/822bd0ba/2021-2022/Liverpool-Stats" }
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.PremierLeague,
        //         Nation = StaticNation.England.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultPremierLeague
        // },

        // new FbrefTag
        // {
        //     Label = "Leeds United",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/5bfb9659/Leeds-United-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.PremierLeague,
        //         Nation = StaticNation.England.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultPremierLeague
        // },

        // new FbrefTag
        // {
        //     Label = "West Ham United",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/7c21e445/West-Ham-United-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.PremierLeague,
        //         Nation = StaticNation.England.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultPremierLeague
        // },

        // new FbrefTag
        // {
        //     Label = "Chelsea",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/cff3d9bb/Chelsea-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.PremierLeague,
        //         Nation = StaticNation.England.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultPremierLeague
        // },

        // new FbrefTag
        // {
        //     Label = "Tottenham Hotspur",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/361ca564/Tottenham-Hotspur-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.PremierLeague,
        //         Nation = StaticNation.England.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultPremierLeague
        // },

        // new FbrefTag
        // {
        //     Label = "Sunderland",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/8ef52968/Sunderland-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.PremierLeague,
        //         Nation = StaticNation.England.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultPremierLeague
        // },

        // new FbrefTag
        // {
        //     Label = "Crystal Palace",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/47c64c55/Crystal-Palace-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.PremierLeague,
        //         Nation = StaticNation.England.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultPremierLeague
        // },

        // new FbrefTag
        // {
        //     Label = "Nottingham Forest",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/e4a775cb/Nottingham-Forest-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.PremierLeague,
        //         Nation = StaticNation.England.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultPremierLeague
        // },

        // new FbrefTag
        // {
        //     Label = "Manchester United",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/19538871/Manchester-United-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.PremierLeague,
        //         Nation = StaticNation.England.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultPremierLeague
        // },

        new FbrefTag
        {
            Label = "Manchester City",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/b8fd03ef/Manchester-City-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.PremierLeague,
                Nation = StaticNation.England.ToString()
            },
            TableIds = TeamTableIds.DefaultPremierLeague
        },

        new FbrefTag
        {
            Label = "Bournemouth",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/4ba7cbea/Bournemouth-Stats" },
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

public static class EnglandChampionshipURLS
{
    public static readonly List<FbrefTag> Urls = new()
    {
        new FbrefTag
        {
            Label = "Wrexham",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/dad7970b/Wrexham-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.EnglandChampionship,
                Nation = StaticNation.England.ToString()
            },
            TableIds = TeamTableIds.DefaultEnglandChampionship
        },

        new FbrefTag
        {
            Label = "Coventry City",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/f7e3dfe9/Coventry-City-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.EnglandChampionship,
                Nation = StaticNation.England.ToString()
            },
            TableIds = TeamTableIds.DefaultEnglandChampionship
        },
    };
}

public static class TurkeySuperLigURLS
{
    public static readonly List<FbrefTag> Urls = new()
    {
        // new FbrefTag
        // {
        //     Label = "Fatih Karagümrük",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/bd61c555/Fatih-Karagumruk-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.SuperLig,
        //         Nation = StaticNation.Turkey.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultTurkeySuperLig
        // },

        // new FbrefTag
        // {
        //     Label = "Kayserispor",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/1f33fbc7/Kayserispor-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.SuperLig,
        //         Nation = StaticNation.Turkey.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultTurkeySuperLig
        // },

        // new FbrefTag
        // {
        //     Label = "Gaziantep FK",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/7c81865f/Gaziantep-FK-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.SuperLig,
        //         Nation = StaticNation.Turkey.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultTurkeySuperLig
        // },

        new FbrefTag
        {
            Label = "Fenerbahçe",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/ae1e2d7d/Fenerbahce-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.SuperLig,
                Nation = StaticNation.Turkey.ToString()
            },
            TableIds = TeamTableIds.DefaultTurkeySuperLig
        },

        // new FbrefTag
        // {
        //     Label = "İstanbul Başakşehi",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/bff39cf5/Istanbul-Basaksehir-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.SuperLig,
        //         Nation = StaticNation.Turkey.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultTurkeySuperLig
        // },

        // new FbrefTag
        // {
        //     Label = "Kocaelispor",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/37fcbb73/Kocaelispor-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.SuperLig,
        //         Nation = StaticNation.Turkey.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultTurkeySuperLig
        // },

        new FbrefTag
        {
            Label = "Beşiktaş",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/0f9294bd/Besiktas-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.SuperLig,
                Nation = StaticNation.Turkey.ToString()
            },
            TableIds = TeamTableIds.DefaultTurkeySuperLig
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

        // new FbrefTag
        // {
        //     Label = "Oțelul Galați",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl{ Season = "2025-2026", URL = "https://fbref.com/en/squads/86edb46e/Otelul-Galati-Stats"},
        //         new SeasonUrl{ Season = "2024-2025", URL = "https://fbref.com/en/squads/86edb46e/2024-2025/Otelul-Galati-Stats"},
        //         new SeasonUrl{ Season = "2023-2024", URL = "https://fbref.com/en/squads/86edb46e/2023-2024/Otelul-Galati-Stats"},
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.Liga1,
        //         Nation = StaticNation.Romania.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultRomaniaLiga1
        // }

        // new FbrefTag
        // {
        //     Label = "FCSB",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl{ Season = "2025-2026", URL = "https://fbref.com/en/squads/aed59852/FCSB-Stats"},
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.Liga1,
        //         Nation = StaticNation.Romania.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultRomaniaLiga1
        // },
        // new FbrefTag
        // {
        //     Label = "UTA Arad",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl{ Season = "2025-2026", URL = "https://fbref.com/en/squads/a862ea1d/UTA-Arad-Stats"},
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.Liga1,
        //         Nation = StaticNation.Romania.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultRomaniaLiga1
        // }

        // new FbrefTag
        // {
        //     Label = "Botoșani",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl{ Season = "2025-2026", URL = "https://fbref.com/en/squads/d921c99f/Botosani-Stats"},
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.Liga1,
        //         Nation = StaticNation.Romania.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultRomaniaLiga1
        // },
        // new FbrefTag
        // {
        //     Label = "Hermannstadt",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl{ Season = "2025-2026", URL = "https://fbref.com/en/squads/d7e82505/Hermannstadt-Stats"},
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.Liga1,
        //         Nation = StaticNation.Romania.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultRomaniaLiga1
        // }

        new FbrefTag
        {
            Label = "Argeș Pitești",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl{ Season = "2025-2026", URL = "https://fbref.com/en/squads/abdef23c/Arges-Pitesti-Stats"},
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
            Label = "Hermannstadt",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl{ Season = "2025-2026", URL = "https://fbref.com/en/squads/d7e82505/Hermannstadt-Stats"},
            },
            League = new League
            {
                LeagueName = StaticLeague.Liga1,
                Nation = StaticNation.Romania.ToString()
            },
            TableIds = TeamTableIds.DefaultRomaniaLiga1
        }
    };
}

public static class ChineseSuperLeagueURLS
{
    public static readonly List<FbrefTag> Urls = new()
    {
        // new FbrefTag
        // {
        //     Label = "Henan",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/b037fc40/Henan-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.ChineseSuperLeague,
        //         Nation = StaticNation.Chinese.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultChineseSuperLeague
        // },

        new FbrefTag
        {
            Label = "Shanghai Port",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/c48512d3/Shanghai-Port-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.ChineseSuperLeague,
                Nation = StaticNation.Chinese.ToString()
            },
            TableIds = TeamTableIds.DefaultChineseSuperLeague
        },

        new FbrefTag
        {
            Label = "Zhejiang",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/8d9bcfa3/Zhejiang-Professional-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.ChineseSuperLeague,
                Nation = StaticNation.Chinese.ToString()
            },
            TableIds = TeamTableIds.DefaultChineseSuperLeague
        },
    };
}

public static class GermanyBundesliga1URLS
{
    public static readonly List<FbrefTag> Urls = new()
    {
        new FbrefTag
        {
            Label = "Dortmund",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/add600ae/Dortmund-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.GermanyBundeslia1,
                Nation = StaticNation.Germany.ToString()
            },
            TableIds = TeamTableIds.DefaultGermanyBundeslia1
        },

        // new FbrefTag
        // {
        //     Label = "Köln",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/bc357bf7/Koln-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.GermanyBundeslia1,
        //         Nation = StaticNation.Germany.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultGermanyBundeslia1
        // },

        new FbrefTag
        {
            Label = "Augsburg",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/0cdc4311/Augsburg-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.GermanyBundeslia1,
                Nation = StaticNation.Germany.ToString()
            },
            TableIds = TeamTableIds.DefaultGermanyBundeslia1
        },
    };
}

public static class ItalySerieAURLS
{
    public static readonly List<FbrefTag> Urls = new()
    {
        // new FbrefTag
        // {
        //     Label = "Napoli",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/d48ad4ff/Napoli-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.ItalySerieA,
        //         Nation = StaticNation.Italy.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultItalySerieA
        // },

        // new FbrefTag
        // {
        //     Label = "Internazionale",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/d609edc0/Internazionale-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.ItalySerieA,
        //         Nation = StaticNation.Italy.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultItalySerieA
        // },

        new FbrefTag
        {
            Label = "Roma",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/cf74a709/Roma-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.ItalySerieA,
                Nation = StaticNation.Italy.ToString()
            },
            TableIds = TeamTableIds.DefaultItalySerieA
        },

        // new FbrefTag
        // {
        //     Label = "Sassuolo",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/e2befd26/Sassuolo-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.ItalySerieA,
        //         Nation = StaticNation.Italy.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultItalySerieA
        // },

        // new FbrefTag
        // {
        //     Label = "Lecce",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/ffcbe334/Lecce-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.ItalySerieA,
        //         Nation = StaticNation.Italy.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultItalySerieA
        // },

        // new FbrefTag
        // {
        //     Label = "Atalanta",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/922493f3/Atalanta-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.ItalySerieA,
        //         Nation = StaticNation.Italy.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultItalySerieA
        // },

        new FbrefTag
        {
            Label = "Milan",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/dc56fe14/Milan-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.ItalySerieA,
                Nation = StaticNation.Italy.ToString()
            },
            TableIds = TeamTableIds.DefaultItalySerieA
        },

        new FbrefTag
        {
            Label = "Parma",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/eab4234c/Parma-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.ItalySerieA,
                Nation = StaticNation.Italy.ToString()
            },
            TableIds = TeamTableIds.DefaultItalySerieA
        },

        new FbrefTag
        {
            Label = "Bologna",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/1d8099f8/Bologna-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.ItalySerieA,
                Nation = StaticNation.Italy.ToString()
            },
            TableIds = TeamTableIds.DefaultItalySerieA
        },

        // new FbrefTag
        // {
        //     Label = "Como",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/28c9c3cd/Como-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.ItalySerieA,
        //         Nation = StaticNation.Italy.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultItalySerieA
        // },

        // new FbrefTag
        // {
        //     Label = "Hellas Verona",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/0e72edf2/Hellas-Verona-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.ItalySerieA,
        //         Nation = StaticNation.Italy.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultItalySerieA
        // },

        // new FbrefTag
        // {
        //     Label = "Udinese",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/04eea015/Udinese-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.ItalySerieA,
        //         Nation = StaticNation.Italy.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultItalySerieA
        // },

        // new FbrefTag
        // {
        //     Label = "Cagliari",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/c4260e09/Cagliari-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.ItalySerieA,
        //         Nation = StaticNation.Italy.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultItalySerieA
        // },

        // new FbrefTag
        // {
        //     Label = "Torino",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/105360fe/Torino-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.ItalySerieA,
        //         Nation = StaticNation.Italy.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultItalySerieA
        // },

        // new FbrefTag
        // {
        //     Label = "Pisa",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/4cceedfc/Pisa-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.ItalySerieA,
        //         Nation = StaticNation.Italy.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultItalySerieA
        // },

        // new FbrefTag
        // {
        //     Label = "Lazio",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/7213da33/Lazio-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.ItalySerieA,
        //         Nation = StaticNation.Italy.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultItalySerieA
        // },

        // new FbrefTag
        // {
        //     Label = "Cremonese",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/9aad3a77/Cremonese-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.ItalySerieA,
        //         Nation = StaticNation.Italy.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultItalySerieA
        // },

        // new FbrefTag
        // {
        //     Label = "Juventus",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/e0652b02/Juventus-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.ItalySerieA,
        //         Nation = StaticNation.Italy.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultItalySerieA
        // },
    };
}

public static class FranceLigue1URLS
{
    public static readonly List<FbrefTag> Urls = new()
    {
        // new FbrefTag
        // {
        //     Label = "Lille",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/cb188c0c/Lille-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.FranceLigue1,
        //         Nation = StaticNation.France.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultFranceLigue1
        // },

        // new FbrefTag
        // {
        //     Label = "Metz",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/f83960ae/Metz-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.FranceLigue1,
        //         Nation = StaticNation.France.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultFranceLigue1
        // },

        // new FbrefTag
        // {
        //     Label = "Nice",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/132ebc33/Nice-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.FranceLigue1,
        //         Nation = StaticNation.France.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultFranceLigue1
        // },

        new FbrefTag
        {
            Label = "Auxerre",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/5ae09109/Auxerre-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.FranceLigue1,
                Nation = StaticNation.France.ToString()
            },
            TableIds = TeamTableIds.DefaultFranceLigue1
        },

        new FbrefTag
        {
            Label = "Marseille",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/5725cc7b/Marseille-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.FranceLigue1,
                Nation = StaticNation.France.ToString()
            },
            TableIds = TeamTableIds.DefaultFranceLigue1
        },
    };
}

public static class FranceLigue2URLS
{
    public static readonly List<FbrefTag> Urls = new()
    {
        new FbrefTag
        {
            Label = "Dunkerque",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/1740a29b/Dunkerque-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.FranceLigue1,
                Nation = StaticNation.France.ToString()
            },
            TableIds = TeamTableIds.DefaultFranceLigue2
        },

        // new FbrefTag
        // {
        //     Label = "Red Star",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/e83d13db/Red-Star-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.FranceLigue1,
        //         Nation = StaticNation.France.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultFranceLigue2
        // },

        new FbrefTag
        {
            Label = "Reims",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/7fdd64e0/Reims-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.FranceLigue1,
                Nation = StaticNation.France.ToString()
            },
            TableIds = TeamTableIds.DefaultFranceLigue2
        },
    };
}

public static class SpainLaliga1URLS
{
    public static readonly List<FbrefTag> Urls = new()
    {
        // new FbrefTag
        // {
        //     Label = "Real Madrid",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/53a2f082/Real-Madrid-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.SpainLaliga1,
        //         Nation = StaticNation.Spain.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultSpainLaliga1
        // },

        new FbrefTag
        {
            Label = "Barcelona",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/206d90db/Barcelona-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.SpainLaliga1,
                Nation = StaticNation.Spain.ToString()
            },
            TableIds = TeamTableIds.DefaultSpainLaliga1
        },

        // new FbrefTag
        // {
        //     Label = "Real Betis",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/fc536746/Real-Betis-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.SpainLaliga1,
        //         Nation = StaticNation.Spain.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultSpainLaliga1
        // },

        // new FbrefTag
        // {
        //     Label = "Atlético Madrid",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/db3b9613/Atletico-Madrid-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.SpainLaliga1,
        //         Nation = StaticNation.Spain.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultSpainLaliga1
        // },

        new FbrefTag
        {
            Label = "Elche",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/6c8b07df/Elche-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.SpainLaliga1,
                Nation = StaticNation.Spain.ToString()
            },
            TableIds = TeamTableIds.DefaultSpainLaliga1
        },

        new FbrefTag
        {
            Label = "Levante",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/9800b6a1/Levante-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.SpainLaliga1,
                Nation = StaticNation.Spain.ToString()
            },
            TableIds = TeamTableIds.DefaultSpainLaliga1
        },

        new FbrefTag
        {
            Label = "Celta Vigo",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/f25da7fb/Celta-Vigo-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.SpainLaliga1,
                Nation = StaticNation.Spain.ToString()
            },
            TableIds = TeamTableIds.DefaultSpainLaliga1
        },
    };
}

public static class NetherlandsEredivisieURLS
{
    public static readonly List<FbrefTag> Urls = new()
    {
        // new FbrefTag
        // {
        //     Label = "Go Ahead Eagles",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/e33d6108/Go-Ahead-Eagles-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.NetherlandsEredivisie,
        //         Nation = StaticNation.Netherlands.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultNetherlandsEredivisie
        // },

        // new FbrefTag
        // {
        //     Label = "Excelsior",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/740cb7d4/Excelsior-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.NetherlandsEredivisie,
        //         Nation = StaticNation.Netherlands.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultNetherlandsEredivisie
        // },

        // new FbrefTag
        // {
        //     Label = "PSV Eindhoven",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/e334d850/PSV-Eindhoven-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.NetherlandsEredivisie,
        //         Nation = StaticNation.Netherlands.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultNetherlandsEredivisie
        // },

        // new FbrefTag
        // {
        //     Label = "Fortuna Sittard",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/bd08295c/Fortuna-Sittard-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.NetherlandsEredivisie,
        //         Nation = StaticNation.Netherlands.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultNetherlandsEredivisie
        // },

        new FbrefTag
        {
            Label = "Heracles Almelo",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/c882b88e/Heracles-Almelo-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.NetherlandsEredivisie,
                Nation = StaticNation.Netherlands.ToString()
            },
            TableIds = TeamTableIds.DefaultNetherlandsEredivisie
        },

        new FbrefTag
        {
            Label = "Zwolle",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/e3db180b/Zwolle-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.NetherlandsEredivisie,
                Nation = StaticNation.Netherlands.ToString()
            },
            TableIds = TeamTableIds.DefaultNetherlandsEredivisie
        },
    };
}

public static class AustraliaALeagueURLS
{
    public static readonly List<FbrefTag> Urls = new()
    {
        // new FbrefTag
        // {
        //     Label = "Macarthur FC",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/f951d655/Macarthur-FC-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.AustraliaALeague,
        //         Nation = StaticNation.Australia.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultAustraliaALeague
        // },

        // new FbrefTag
        // {
        //     Label = "Adelaide United",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/a4302376/Adelaide-United-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.AustraliaALeague,
        //         Nation = StaticNation.Australia.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultAustraliaALeague
        // },

        // new FbrefTag
        // {
        //     Label = "Melbourne City",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/31553c94/Melbourne-City-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.AustraliaALeague,
        //         Nation = StaticNation.Australia.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultAustraliaALeague
        // },

        // new FbrefTag
        // {
        //     Label = "Brisbane Roar",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/3efc42c3/Brisbane-Roar-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.AustraliaALeague,
        //         Nation = StaticNation.Australia.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultAustraliaALeague
        // },

        // new FbrefTag
        // {
        //     Label = "Auckland FC",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/4cb614ef/Auckland-FC-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.AustraliaALeague,
        //         Nation = StaticNation.Australia.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultAustraliaALeague
        // },
        
        new FbrefTag
        {
            Label = "Central Coast Mariners",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/605aca82/Central-Coast-Mariners-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.AustraliaALeague,
                Nation = StaticNation.Australia.ToString()
            },
            TableIds = TeamTableIds.DefaultAustraliaALeague
        },

        new FbrefTag
        {
            Label = "Wellington Phoenix",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/81134e0b/Wellington-Phoenix-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.AustraliaALeague,
                Nation = StaticNation.Australia.ToString()
            },
            TableIds = TeamTableIds.DefaultAustraliaALeague
        },
    };
}

public static class SwedenAllsvenskanURLS
{
    public static readonly List<FbrefTag> Urls = new()
    {
        new FbrefTag
        {
            Label = "Brommapojkarna",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/bb9e11b2/Brommapojkarna-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.SwedenAllsvenskan,
                Nation = StaticNation.Sweden.ToString()
            },
            TableIds = TeamTableIds.DefaultSwedenAllsvenskan
        },

        new FbrefTag
        {
            Label = "GAIS",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/7c2d1adb/GAIS-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.SwedenAllsvenskan,
                Nation = StaticNation.Sweden.ToString()
            },
            TableIds = TeamTableIds.DefaultSwedenAllsvenskan
        },

        new FbrefTag
        {
            Label = "Malmö",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/f3d8c8b9/Malmo-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.SwedenAllsvenskan,
                Nation = StaticNation.Sweden.ToString()
            },
            TableIds = TeamTableIds.DefaultSwedenAllsvenskan
        },

        new FbrefTag
        {
            Label = "Hammarby",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/92bfd7f0/Hammarby-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.SwedenAllsvenskan,
                Nation = StaticNation.Sweden.ToString()
            },
            TableIds = TeamTableIds.DefaultSwedenAllsvenskan
        },
    };
}

public static class ArgentinaTorneoBetanoURLS
{
    public static readonly List<FbrefTag> Urls = new()
    {
        // new FbrefTag
        // {
        //     Label = "Barracas Central",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/1d89634b/Barracas-Central-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.ArgentinaTorneoBetano,
        //         Nation = StaticNation.Argentina.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultArgentinaTorneoBetano
        // },

        // new FbrefTag
        // {
        //     Label = "Boca Juniors",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/795ca75e/Boca-Juniors-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.ArgentinaTorneoBetano,
        //         Nation = StaticNation.Argentina.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultArgentinaTorneoBetano
        // },

        new FbrefTag
        {
            Label = "Lanús",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/11b6dba8/Lanus-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.ArgentinaTorneoBetano,
                Nation = StaticNation.Argentina.ToString()
            },
            TableIds = TeamTableIds.DefaultArgentinaTorneoBetano
        },
    };
}

public static class USAMLSURLS
{
    public static readonly List<FbrefTag> Urls = new()
    {
        // new FbrefTag
        // {
        //     Label = "Charlotte FC",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/eb57545a/Charlotte-FC-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.USAMLS,
        //         Nation = StaticNation.USA.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultUSAMLS
        // },

        // new FbrefTag
        // {
        //     Label = "New York City FC",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/64e81410/New-York-City-FC-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.USAMLS,
        //         Nation = StaticNation.USA.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultUSAMLS
        // },

        new FbrefTag
        {
            Label = "FC Dallas",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/15cf8f40/FC-Dallas-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.USAMLS,
                Nation = StaticNation.USA.ToString()
            },
            TableIds = TeamTableIds.DefaultUSAMLS
        },

        new FbrefTag
        {
            Label = "Vancouver Whitecaps FC",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/ab41cb90/Vancouver-Whitecaps-FC-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.USAMLS,
                Nation = StaticNation.USA.ToString()
            },
            TableIds = TeamTableIds.DefaultUSAMLS
        },
    };
}

public static class BrazilSerieABetanoURLS
{
    public static readonly List<FbrefTag> Urls = new()
    {
        // new FbrefTag
        // {
        //     Label = "Atlético Mineiro",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/422bb734/Atletico-Mineiro-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.BrazilSerieABetano,
        //         Nation = StaticNation.Brazil.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultBrazilSerieABetano
        // },

        new FbrefTag
        {
            Label = "Mirassol",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/289e8847/Mirassol-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.BrazilSerieABetano,
                Nation = StaticNation.Brazil.ToString()
            },
            TableIds = TeamTableIds.DefaultBrazilSerieABetano
        },

        new FbrefTag
        {
            Label = "Botafogo (RJ)",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/d9fdd9d9/Botafogo-RJ-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.BrazilSerieABetano,
                Nation = StaticNation.Brazil.ToString()
            },
            TableIds = TeamTableIds.DefaultBrazilSerieABetano
        },
    };
}

public static class EcuadorLigaProURLS
{
    public static readonly List<FbrefTag> Urls = new()
    {
        new FbrefTag
        {
            Label = "Independiente del Valle",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/990519b8/Independiente-del-Valle-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.EcuadorLigaPro,
                Nation = StaticNation.Ecuador.ToString()
            },
            TableIds = TeamTableIds.DefaultEcuadorLigaPro
        },
    };
}

public static class ColombiPrimeraAURLS
{
    public static readonly List<FbrefTag> Urls = new()
    {
        new FbrefTag
        {
            Label = "AD Cali",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/47538775/AD-Cali-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.ColombiPrimeraA,
                Nation = StaticNation.Colombia.ToString()
            },
            TableIds = TeamTableIds.DefaultColombiaPrimeraA
        },

        new FbrefTag
        {
            Label = "Alianza",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/ffc8a1d6/Alianza-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.ColombiPrimeraA,
                Nation = StaticNation.Colombia.ToString()
            },
            TableIds = TeamTableIds.DefaultColombiaPrimeraA
        },

    };
}

public static class ChileLigaDePrimeraURLS
{
    public static readonly List<FbrefTag> Urls = new()
    {
        // new FbrefTag
        // {
        //     Label = "Universidad de Chile",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/d4a88ef6/Universidad-de-Chile-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.ChileLigaDePrimera,
        //         Nation = StaticNation.Chile.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultChileLigaDePrimera
        // },

        new FbrefTag
        {
            Label = "Audax Italiano",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/13b57ed6/Audax-Italiano-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.ChileLigaDePrimera,
                Nation = StaticNation.Chile.ToString()
            },
            TableIds = TeamTableIds.DefaultChileLigaDePrimera
        },

        new FbrefTag
        {
            Label = "Cobresal",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/ed392b02/Cobresal-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.ChileLigaDePrimera,
                Nation = StaticNation.Chile.ToString()
            },
            TableIds = TeamTableIds.DefaultChileLigaDePrimera
        },
    };
}

public static class SaudiArabiaProfessionalLeagueURLS
{
    public static readonly List<FbrefTag> Urls = new()
    {
        new FbrefTag
        {
            Label = "Al-Hilal",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/972e2539/Al-Hilal-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.SaudiArabiaProfessionalLeague,
                Nation = StaticNation.SaudiArabia.ToString()
            },
            TableIds = TeamTableIds.DefaultSaudiArabiaProfessionalLeague
        },

        new FbrefTag
        {
            Label = "Al-Shabab",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/84bbaea6/Al-Shabab-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.SaudiArabiaProfessionalLeague,
                Nation = StaticNation.SaudiArabia.ToString()
            },
            TableIds = TeamTableIds.DefaultSaudiArabiaProfessionalLeague
        },
    };
}

public static class MexicoLigaMXURLS
{
    public static readonly List<FbrefTag> Urls = new()
    {
        // new FbrefTag
        // {
        //     Label = "Necaxa",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/752db496/Necaxa-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.MexicoLigaMX,
        //         Nation = StaticNation.Mexico.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultMexicoLigaMX
        // },

        // new FbrefTag
        // {
        //     Label = "Santos Laguna",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/03b65ba9/Santos-Laguna-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.MexicoLigaMX,
        //         Nation = StaticNation.Mexico.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultMexicoLigaMX
        // },

        // new FbrefTag
        // {
        //     Label = "Monterrey",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/dd5ca9bd/Monterrey-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.MexicoLigaMX,
        //         Nation = StaticNation.Mexico.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultMexicoLigaMX
        // },

        // new FbrefTag
        // {
        //     Label = "UANL",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/d9e1bd51/UANL-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.MexicoLigaMX,
        //         Nation = StaticNation.Mexico.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultMexicoLigaMX
        // },

        // new FbrefTag
        // {
        //     Label = "América",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/18d3c3a3/America-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.MexicoLigaMX,
        //         Nation = StaticNation.Mexico.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultMexicoLigaMX
        // },

        // new FbrefTag
        // {
        //     Label = "León",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/fd7dad55/Leon-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.MexicoLigaMX,
        //         Nation = StaticNation.Mexico.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultMexicoLigaMX
        // },

        new FbrefTag
        {
            Label = "Pumas UNAM",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/c9d59c6c/Pumas-UNAM-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.MexicoLigaMX,
                Nation = StaticNation.Mexico.ToString()
            },
            TableIds = TeamTableIds.DefaultMexicoLigaMX
        },

        new FbrefTag
        {
            Label = "Tijuana",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/a42ddf2f/Tijuana-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.MexicoLigaMX,
                Nation = StaticNation.Mexico.ToString()
            },
            TableIds = TeamTableIds.DefaultMexicoLigaMX
        },
    };
}

public static class KoreaKLeague1URLS
{
    public static readonly List<FbrefTag> Urls = new()
    {
        // new FbrefTag
        // {
        //     Label = "FC Anyang",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/5f4cdc77/FC-Anyang-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.KoreaKLeague1,
        //         Nation = StaticNation.Korea.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultKoreaKLeague1
        // },

        // new FbrefTag
        // {
        //     Label = "Ulsan HD",
        //     SeasonUrls = new List<SeasonUrl>
        //     {
        //         new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/4372a20b/Ulsan-HD-Stats" },
        //     },
        //     League = new League
        //     {
        //         LeagueName = StaticLeague.KoreaKLeague1,
        //         Nation = StaticNation.Korea.ToString()
        //     },
        //     TableIds = TeamTableIds.DefaultKoreaKLeague1
        // },

        new FbrefTag
        {
            Label = "Gwangju FC",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/ae306ede/Gwangju-FC-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.KoreaKLeague1,
                Nation = StaticNation.Korea.ToString()
            },
            TableIds = TeamTableIds.DefaultKoreaKLeague1
        },

        new FbrefTag
        {
            Label = "Jeju United FC",
            SeasonUrls = new List<SeasonUrl>
            {
                new SeasonUrl { Season = "2025-2026", URL = "https://fbref.com/en/squads/e1e9c597/Jeju-United-FC-Stats" },
            },
            League = new League
            {
                LeagueName = StaticLeague.KoreaKLeague1,
                Nation = StaticNation.Korea.ToString()
            },
            TableIds = TeamTableIds.DefaultKoreaKLeague1
        },
    };    
}