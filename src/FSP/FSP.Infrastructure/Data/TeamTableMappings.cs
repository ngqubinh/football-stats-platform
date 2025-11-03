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
    public static readonly TeamTableIds DefaultEnglandChampionship = new TeamTableIds("10", "10", "10", "10");
    public static readonly TeamTableIds DefaultEuropaLeague = new TeamTableIds("50", "50", "50", "50");
    public static readonly TeamTableIds DefaultRomaniaLiga1 = new TeamTableIds("47", "47", "47", "47");
    public static readonly TeamTableIds DefaultTurkeySuperLig = new TeamTableIds("26", "26", "26", "26");
    public static readonly TeamTableIds DefaultChineseSuperLeague = new TeamTableIds("62", "62", "62", "62");
    public static readonly TeamTableIds DefaultGermanyBundeslia1 = new TeamTableIds("20", "20", "20", "20");
    public static readonly TeamTableIds DefaultItalySerieA = new TeamTableIds("11", "11", "11", "11");
    public static readonly TeamTableIds DefaultFranceLigue1 = new TeamTableIds("13", "13", "13", "13");
    public static readonly TeamTableIds DefaultFranceLigue2 = new TeamTableIds("60", "60", "60", "60");
    public static readonly TeamTableIds DefaultSpainLaliga1 = new TeamTableIds("12", "12", "12", "12");
    public static readonly TeamTableIds DefaultNetherlandsEredivisie = new TeamTableIds("23", "23", "23", "23");
    public static readonly TeamTableIds DefaultAustraliaALeague = new TeamTableIds("65", "65", "65", "65");
    public static readonly TeamTableIds DefaultSwedenAllsvenskan = new TeamTableIds("29", "29", "29", "29");
    public static readonly TeamTableIds DefaultArgentinaTorneoBetano = new TeamTableIds("21", "21", "21", "21");
    public static readonly TeamTableIds DefaultUSAMLS = new TeamTableIds("22", "22", "22", "22");
    public static readonly TeamTableIds DefaultBrazilSerieABetano = new TeamTableIds("24", "24", "24", "24");
    public static readonly TeamTableIds DefaultEcuadorLigaPro = new TeamTableIds("58", "58", "58", "58");
    public static readonly TeamTableIds DefaultColombiaPrimeraA = new TeamTableIds("41", "41", "41", "41");
    public static readonly TeamTableIds DefaultChileLigaDePrimera = new TeamTableIds("35", "35", "35", "35");
    public static readonly TeamTableIds DefaultSaudiArabiaProfessionalLeague = new TeamTableIds("70", "70", "70", "70");
    public static readonly TeamTableIds DefaultMexicoLigaMX = new TeamTableIds("31", "31", "31", "31");
    public static readonly TeamTableIds DefaultKoreaKLeague1 = new TeamTableIds("55", "55", "55", "55");
}

public static class TeamTableMappings
{
    public static TeamTableIds GetTableIds(string teamLabel, string leagueName)
    {
        return leagueName switch
        {
            "Premier League" => new TeamTableIds("9", "9", "9", "9"),
            "Liga 1" => new TeamTableIds("47", "47", "47", "47"),
            "Europa League" => new TeamTableIds("50", "50", "50", "50"),
            _ => new TeamTableIds("0", "0", "0", "0")
        };
    }
}
