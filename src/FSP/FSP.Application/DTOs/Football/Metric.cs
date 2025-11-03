using System;
using System.Text.Json.Serialization;

namespace FSP.Application.DTOs.Football;

public class TwoTeamLineupInputDto
{
    [JsonPropertyName("team_a")]
    public TeamLineupInputDto TeamA { get; set; } = new();

    [JsonPropertyName("team_b")]
    public TeamLineupInputDto TeamB { get; set; } = new();

    [JsonPropertyName("match_date")]
    public string? MatchDate { get; set; }

    [JsonPropertyName("venue")]
    public string? Venue { get; set; }
}

public class TeamLineupInputDto
{
    [JsonPropertyName("club_id")]
    public int ClubId { get; set; }

    [JsonPropertyName("club_name")]
    public string ClubName { get; set; } = string.Empty;

    [JsonPropertyName("formation")]
    public string Formation { get; set; } = string.Empty;

    [JsonPropertyName("players")]
    public List<LineupPlayerInputDto> Players { get; set; } = new();
}

public class TwoTeamComparisonDto
{
    [JsonPropertyName("team_a_analysis")]
    public LineupAnalysisDto TeamAAnalysis { get; set; } = new();

    [JsonPropertyName("team_b_analysis")]
    public LineupAnalysisDto TeamBAnalysis { get; set; } = new();

    [JsonPropertyName("comparison")]
    public TeamComparisonDto Comparison { get; set; } = new();

    [JsonPropertyName("prediction")]
    public MatchPredictionDto Prediction { get; set; } = new();

    [JsonPropertyName("key_battles")]
    public List<PlayerBattleDto> KeyBattles { get; set; } = new();
}

public class TeamComparisonDto
{
    [JsonPropertyName("total_score_advantage")]
    public float TotalScoreAdvantage { get; set; }

    [JsonPropertyName("average_score_advantage")]
    public float AverageScoreAdvantage { get; set; }

    [JsonPropertyName("advantage_team")]
    public string AdvantageTeam { get; set; } = string.Empty;

    [JsonPropertyName("advantage_percentage")]
    public float AdvantagePercentage { get; set; }

    [JsonPropertyName("position_advantages")]
    public List<PositionAdvantageDto> PositionAdvantages { get; set; } = new();
}

public class PositionAdvantageDto
{
    [JsonPropertyName("position")]
    public string Position { get; set; } = string.Empty;

    [JsonPropertyName("advantage_team")]
    public string AdvantageTeam { get; set; } = string.Empty;

    [JsonPropertyName("team_a_score")]
    public float TeamAScore { get; set; }

    [JsonPropertyName("team_b_score")]
    public float TeamBScore { get; set; }

    [JsonPropertyName("advantage_margin")]
    public float AdvantageMargin { get; set; }
}

public class MatchPredictionDto
{
    [JsonPropertyName("predicted_winner")]
    public string PredictedWinner { get; set; } = string.Empty;

    [JsonPropertyName("confidence")]
    public string Confidence { get; set; } = string.Empty;

    [JsonPropertyName("predicted_score")]
    public string PredictedScore { get; set; } = string.Empty;

    [JsonPropertyName("key_factors")]
    public List<string> KeyFactors { get; set; } = new();
}

public class PlayerBattleDto
{
    [JsonPropertyName("position")]
    public string Position { get; set; } = string.Empty;

    [JsonPropertyName("team_a_player")]
    public PlayerWithScoreDto? TeamAPlayer { get; set; }

    [JsonPropertyName("team_b_player")]
    public PlayerWithScoreDto? TeamBPlayer { get; set; }

    [JsonPropertyName("advantage")]
    public string Advantage { get; set; } = string.Empty;

    [JsonPropertyName("advantage_margin")]
    public float AdvantageMargin { get; set; }
}

public class LineupInputDto
{
    [JsonPropertyName("players")]
    public List<LineupPlayerInputDto> Players { get; set; } = new();

    [JsonPropertyName("formation")]
    public string Formation { get; set; } = string.Empty;

    [JsonPropertyName("match_date")]
    public string? MatchDate { get; set; }
}

public class LineupPlayerInputDto
{
    [JsonPropertyName("player_ref_id")]
    public string PlayerRefId { get; set; } = string.Empty;

    [JsonPropertyName("player_name")]
    public string PlayerName { get; set; } = string.Empty;

    [JsonPropertyName("position")]
    public string Position { get; set; } = string.Empty;

    [JsonPropertyName("is_starting")]
    public bool IsStarting { get; set; } = true;
}

public class LineupAnalysisDto
{
    [JsonPropertyName("club_id")]
    public int ClubId { get; set; }

    [JsonPropertyName("lineup_players")]
    public IEnumerable<PlayerWithScoreDto> LineupPlayers { get; set; } = new List<PlayerWithScoreDto>();

    [JsonPropertyName("total_score")]
    public float TotalScore { get; set; }

    [JsonPropertyName("average_score")]
    public float AverageScore { get; set; }

    [JsonPropertyName("club_average_score")]
    public float ClubAverageScore { get; set; }

    [JsonPropertyName("optimal_lineup_score")]
    public float OptimalLineupScore { get; set; }

    [JsonPropertyName("formation")]
    public string Formation { get; set; } = string.Empty;

    [JsonPropertyName("position_breakdown")]
    public PositionBreakdownDto PositionBreakdown { get; set; } = new();

    [JsonPropertyName("strengths")]
    public List<string> Strengths { get; set; } = new();

    [JsonPropertyName("weaknesses")]
    public List<string> Weaknesses { get; set; } = new();

    [JsonPropertyName("overall_rating")]
    public string OverallRating { get; set; } = string.Empty;

    [JsonPropertyName("lineup_efficiency")]
    public float LineupEfficiency { get; set; }

    [JsonPropertyName("missing_top_players")]
    public List<string> MissingTopPlayers { get; set; } = new();
}

public class PositionBreakdownDto
{
    [JsonPropertyName("goalkeepers")]
    public int Goalkeepers { get; set; }

    [JsonPropertyName("defenders")]
    public int Defenders { get; set; }

    [JsonPropertyName("midfielders")]
    public int Midfielders { get; set; }

    [JsonPropertyName("attackers")]
    public int Attackers { get; set; }

    [JsonPropertyName("avg_goalkeeper_score")]
    public float AvgGoalkeeperScore { get; set; }

    [JsonPropertyName("avg_defender_score")]
    public float AvgDefenderScore { get; set; }

    [JsonPropertyName("avg_midfielder_score")]
    public float AvgMidfielderScore { get; set; }

    [JsonPropertyName("avg_attacker_score")]
    public float AvgAttackerScore { get; set; }
}
