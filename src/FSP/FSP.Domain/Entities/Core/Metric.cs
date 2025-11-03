namespace FSP.Domain.Entities.Core;

public class ExtraMetrics
{
    public bool Home { get; set; }
    public int TeamLevel { get; set; } // Point (eg.. 74 32)
    public bool Motivation { get; set; }
    public int RecentForm { get; set; }
    public string MatchType { get; set; } = "Normal";
}

public static class FormCalculator
{
    public static int CalculateRecentForm(string[] lastFiveMatches)
    {
        // W = Win (10 points), D = Draw (5 points), L = Loss (-1 point)
        int totalPoints = 0;

        foreach (string result in lastFiveMatches)
        {
            switch (result.ToUpper())
            {
                case "W":
                    totalPoints += 10;
                    break;
                case "D":
                    totalPoints += 5;
                    break;
                case "L":
                    totalPoints -= 1;
                    break;
                default:
                    break;
            }
        }

        return totalPoints;
    }
}

public class PlayerQualityPoint
{
    public int Shit => 0;
    public int Normal => 3;
    public int Solid => 5;
    public int QuiteSolid => 7;
    public int VerySolid => 10;
    public int Impressive => 12;
    public int QuiteImpressive => 15;
    public int VeryImpressive => 20;
}

public class MatchType
{
    public int Normal => 10;
    public int Champion => 30;
    public int Relegation => 30;
    public int Derby => 20;
    public int NormalDomesticCup => 5;
    public int DomesticCup => 8;
    public int NormalContinentalCup => 12;
    public int ContinentalCup => 40;
}

public class TwoTeamLineupInput
{
    public TeamLineupInput TeamA { get; set; } = new();
    public TeamLineupInput TeamB { get; set; } = new();
    public string? MatchDate { get; set; }
    public string? Venue { get; set; }

    public ExtraMetrics? TeamAExtraMetrics { get; set; }
    public ExtraMetrics? TeamBExtraMetrics { get; set; }

    public string MatchType { get; set; } = "Normal"; // New property
}

public class TeamLineupInput
{
    public int ClubId { get; set; }
    public string ClubName { get; set; } = string.Empty;
    public string Formation { get; set; } = string.Empty;
    public List<LineupPlayerInput> Players { get; set; } = new();
}

public class TwoTeamComparison
{
    public LineupAnalysis TeamAAnalysis { get; set; } = new();
    public LineupAnalysis TeamBAnalysis { get; set; } = new();
    public TeamComparison Comparison { get; set; } = new();
    public MatchPrediction Prediction { get; set; } = new();
    public List<PlayerBattle> KeyBattles { get; set; } = new();
}

public class TeamComparison
{
    public float TotalScoreAdvantage { get; set; }
    public float AverageScoreAdvantage { get; set; }
    public string AdvantageTeam { get; set; } = string.Empty;
    public float AdvantagePercentage { get; set; }
    public List<PositionAdvantage> PositionAdvantages { get; set; } = new();

    public float AdjustedTeamAScore { get; set; }
    public float AdjustedTeamBScore { get; set; }
    public bool ExtraMetricsApplied { get; set; }

    public string MatchType { get; set; } = "Normal";
}

public class PositionAdvantage
{
    public string Position { get; set; } = string.Empty;
    public string AdvantageTeam { get; set; } = string.Empty;
    public float TeamAScore { get; set; }
    public float TeamBScore { get; set; }
    public float AdvantageMargin { get; set; }
}

public class MatchPrediction
{
    public string PredictedWinner { get; set; } = string.Empty;
    public string Confidence { get; set; } = string.Empty;
    public string PredictedScore { get; set; } = string.Empty;
    public List<string> KeyFactors { get; set; } = new();
}

public class PlayerBattle
{
    public string Position { get; set; } = string.Empty;
    public PlayerWithScore? TeamAPlayer { get; set; }
    public PlayerWithScore? TeamBPlayer { get; set; }
    public string Advantage { get; set; } = string.Empty;
    public float AdvantageMargin { get; set; }
}

public class LineupInput
{
    public List<LineupPlayerInput> Players { get; set; } = new();
    public string Formation { get; set; } = string.Empty;
    public string? MatchDate { get; set; }
}

public class LineupPlayerInput
{
    public string PlayerRefId { get; set; } = string.Empty;
    public string PlayerName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public bool IsStarting { get; set; } = true;
    public string Quality { get; set; } = "Normal";
}

public class LineupAnalysis
{
    public int ClubId { get; set; }
    public IEnumerable<PlayerWithScore> LineupPlayers { get; set; } = new List<PlayerWithScore>();
    public float TotalScore { get; set; }
    public float AverageScore { get; set; }
    public float ClubAverageScore { get; set; }
    public float OptimalLineupScore { get; set; }
    public string Formation { get; set; } = string.Empty;
    public PositionBreakdown PositionBreakdown { get; set; } = new();
    public List<string> Strengths { get; set; } = new();
    public List<string> Weaknesses { get; set; } = new();
    public string OverallRating { get; set; } = string.Empty;
    public float LineupEfficiency { get; set; }
    public List<string> MissingTopPlayers { get; set; } = new();
}

public class PositionBreakdown
{
    public int Goalkeepers { get; set; }
    public int Defenders { get; set; }
    public int Midfielders { get; set; }
    public int Attackers { get; set; }
    public float AvgGoalkeeperScore { get; set; }
    public float AvgDefenderScore { get; set; }
    public float AvgMidfielderScore { get; set; }
    public float AvgAttackerScore { get; set; }
}
