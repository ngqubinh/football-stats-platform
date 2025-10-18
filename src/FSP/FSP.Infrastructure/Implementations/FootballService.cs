using FSP.Domain.Entities;
using FSP.Domain.Entities.Core;
using FSP.Domain.Interfaces.Core;
using FSP.Domain.Interfaces.RepositoryPattern;
using FSP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FSP.Infrastructure.Implementations;


public enum PlayerPosition
{
    GK, DF, MF, FW
}

public class FootballService : IFootballService
{
    private readonly ILogger<FootballService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public FootballService(ILogger<FootballService> logger, IUnitOfWork unitOfWork)
    {
        this._logger = logger;
        this._unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<PlayerSeasonComparison>>> ComparePlayerWithPrevisousSeasonsAsync(string playerRefId)
    {
        string correlationId = Guid.NewGuid().ToString();
        using IDisposable? scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Operation"] = nameof(this.ComparePlayerWithPrevisousSeasonsAsync)
        });

        try
        {
            var playerSeasons = await this._unitOfWork.Players.GetAllAsync(predicate: p => p.PlayerRefId == playerRefId);
            if (!playerSeasons.Any())
            {
                _logger.LogWarning("No player found with Ref ID {PlayerRefId}.", playerRefId);
                return Result<IEnumerable<PlayerSeasonComparison>>.Fail($"No player found with Ref ID {playerRefId}.");
            }

            var orderedSeasons = playerSeasons.OrderByDescending(p => p.Season).ToList();
            if (orderedSeasons.Count < 2)
            {
                _logger.LogWarning("Player {PlayerRefId} has only one season of data. Cannot compare.", playerRefId);
                return Result<IEnumerable<PlayerSeasonComparison>>.Ok(Enumerable.Empty<PlayerSeasonComparison>());
            }

            var comparisons = new List<PlayerSeasonComparison>();

            for (int i = 0; i < orderedSeasons.Count() - 1; i++)
            {
                var currentSeason = orderedSeasons[i];
                var previousSeason = orderedSeasons[i + 1];
                var comparison = this.CreateSeasonComparison(currentSeason, previousSeason);
                comparisons.Add(comparison);
            }

            this._logger.LogInformation("Created {Count} season comparisons for player Ref ID {PlayerId}", comparisons.Count, playerRefId);
            return Result<IEnumerable<PlayerSeasonComparison>>.Ok(comparisons);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error comparing player {PlayerId} with previous season: {Message}", playerRefId, ex.Message);
            return Result<IEnumerable<PlayerSeasonComparison>>.Fail($"Error comparing player {playerRefId} with previous season.");
        }
    }

    public async Task<Result<PlayerSeasonComparison>> GetPlayerCurrentVsPreviousSeasonAsync(string playerRefId)
    {
        string correlationId = Guid.NewGuid().ToString();
        using IDisposable? scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Operation"] = nameof(GetPlayerCurrentVsPreviousSeasonAsync)
        });

        try
        {
            // Get current season
            string currentSeason = PremierLeagueURLS.Urls
                .SelectMany(c => c.SeasonUrls)
                .Select(s => s.Season)
                .OrderByDescending(season => season)
                .FirstOrDefault() ?? "2024-2025";

            string previousSeason = GetPreviousSeason(currentSeason);

            // Get player's current and previous season data
            var playerSeasons = await _unitOfWork.Players.GetAllAsync(
                predicate: p => p.PlayerRefId == playerRefId &&
                            (p.Season == currentSeason || p.Season == previousSeason)
            );

            var seasons = playerSeasons.OrderByDescending(p => p.Season).ToList();

            if (seasons.Count < 2)
            {
                return Result<PlayerSeasonComparison>.Fail("Not enough season data for comparison.");
            }

            // Ensure we have both current and previous season data
            var currentSeasonData = seasons.FirstOrDefault(p => p.Season == currentSeason);
            var previousSeasonData = seasons.FirstOrDefault(p => p.Season == previousSeason);

            if (currentSeasonData == null || previousSeasonData == null)
            {
                return Result<PlayerSeasonComparison>.Fail("Missing current or previous season data.");
            }

            var comparison = CreateSeasonComparison(currentSeasonData, previousSeasonData);
            return Result<PlayerSeasonComparison>.Ok(comparison);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error comparing current vs previous season for player {PlayerId}: {Message}", playerRefId, ex.Message);
            return Result<PlayerSeasonComparison>.Fail($"Error comparing seasons for player {playerRefId}.");
        }
    }

    public async Task<Result<IEnumerable<League>>> GetAllLeaguesAsync()
    {
        string? correlationId = Guid.NewGuid().ToString();
        using IDisposable? scope = this._logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Operation"] = nameof(this.GetAllLeaguesAsync)
        });

        try
        {
            IEnumerable<League> leagues = await this._unitOfWork.Leagues.GetAllAsync();
            if (!leagues.Any())
            {
                this._logger.LogWarning("No leagues found.");
                return Result<IEnumerable<League>>.Ok(new List<League>());
            }

            this._logger.LogInformation("Retrieved {Count} leagues", leagues.Count());
            return Result<IEnumerable<League>>.Ok(leagues);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error fetching leagues: {Message}", ex.Message);
            return Result<IEnumerable<League>>.Fail("Error fetching leagues.");
        }
    }

    public async Task<Result<IEnumerable<Club>>> GetClubsByLeagueAsync(int leagueId)
    {
        string? correlationId = Guid.NewGuid().ToString();
        using IDisposable? scope = this._logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Operation"] = nameof(this.GetClubsByLeagueAsync)
        });

        try
        {
            IEnumerable<Club> clubs = await this._unitOfWork.Clubs.GetAllAsync(c => c.LeagueId == leagueId);
            if (!clubs.Any())
            {
                this._logger.LogWarning("No clubs found for league ID {LeagueId}.", leagueId);
                return Result<IEnumerable<Club>>.Ok(new List<Club>());
            }

            this._logger.LogInformation("Retrieved {Count} clubs for league ID {LeagueId}", clubs.Count(), leagueId);
            return Result<IEnumerable<Club>>.Ok(clubs);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error fetching clubs for league ID {LeagueId}: {Message}", leagueId, ex.Message);
            return Result<IEnumerable<Club>>.Fail($"Error fetching clubs for league ID {leagueId}.");
        }
    }

    public async Task<Result<IEnumerable<Player>>> GetCurrentPlayersByClubAsync(int clubId)
    {
        string? correlationId = Guid.NewGuid().ToString();
        using IDisposable? scope = this._logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Operation"] = nameof(this.GetCurrentPlayersByClubAsync)
        });

        try
        {
            string? currentSeason = PremierLeagueURLS.Urls
                    .SelectMany(c => c.SeasonUrls)
                    .Select(s => s.Season)
                    .OrderByDescending(season => season)
                    .FirstOrDefault();

            IEnumerable<Player> players = await this._unitOfWork.Players.GetAllAsync(c => c.ClubId == clubId && c.Season == currentSeason);
            if (!players.Any())
            {
                this._logger.LogWarning("No players found for club ID {ClubId}.", clubId);
                return Result<IEnumerable<Player>>.Ok(new List<Player>());
            }

            this._logger.LogInformation("Retrieved {Count} current players for club ID {ClubId}", players.Count(), clubId);
            return Result<IEnumerable<Player>>.Ok(players);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error fetching current players for club ID {ClubId}: {Message}", clubId, ex.Message);
            return Result<IEnumerable<Player>>.Fail($"Error fetching current players for club ID {clubId}.");
        }
    }

    public async Task<Result<IEnumerable<Player>>> GetPlayersByClubAsync(int clubId)
    {
        string? correlationId = Guid.NewGuid().ToString();
        using IDisposable? scope = this._logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Operation"] = nameof(this.GetPlayersByClubAsync)
        });

        try
        {
            IEnumerable<Player> players = await this._unitOfWork.Players.GetAllAsync(c => c.ClubId == clubId);
            if (!players.Any())
            {
                this._logger.LogWarning("No players found for club ID {ClubId}.", clubId);
                return Result<IEnumerable<Player>>.Ok(new List<Player>());
            }

            this._logger.LogInformation("Retrieved {Count} players for club ID {ClubId}", players.Count(), clubId);
            return Result<IEnumerable<Player>>.Ok(players);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error fetching players for club ID {ClubId}: {Message}", clubId, ex.Message);
            return Result<IEnumerable<Player>>.Fail($"Error fetching players for club ID {clubId}.");
        }
    }

    public async Task<Result<Goalkeeping>> GetCurrentGoalkeepingByPlayerAsync(string playerRefId)
    {
        string correlationId = Guid.NewGuid().ToString();
        using IDisposable? scope = this._logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Operation"] = nameof(this.GetCurrentGoalkeepingByPlayerAsync)
        });

        try
        {
            string? currentSeason = PremierLeagueURLS.Urls
                    .SelectMany(c => c.SeasonUrls)
                    .Select(s => s.Season)
                    .OrderByDescending(season => season)
                    .FirstOrDefault();
            if (string.IsNullOrEmpty(currentSeason))
            {
                this._logger.LogWarning("Current season not found.");
                return Result<Goalkeeping>.Fail("Current season not found.");
            }

            Player? player = await this._unitOfWork.Players.GetByAsync(
                predicate: p => p.PlayerRefId == playerRefId && p.Season == currentSeason
            );
            if (player == null)
            {
                this._logger.LogWarning("No current season player found with Ref ID {PlayerRefId}.", playerRefId);
                return Result<Goalkeeping>.Fail($"No current season player found with Ref ID {playerRefId}.");
            }

            // Check if player is a goalkeeper
            if (player.Position != PlayerPosition.GK.ToString())
            {
                this._logger.LogWarning("Player {PlayerName} is not a goalkeeper. Position: {Position}",
                    player.PlayerName, player.Position);
                return Result<Goalkeeping>.Fail($"Player {player.PlayerName} is not a goalkeeper.");
            }

            Goalkeeping? goalkeeping = await this._unitOfWork.Goalkeepings.GetByAsync(
                predicate: g => g.PlayerId == player.PlayerId && g.Season == currentSeason
            );
            if (goalkeeping == null)
            {
                this._logger.LogWarning("No goalkeeping data found for player Ref ID {PlayerRefId} in season {Season}.",
                    playerRefId, currentSeason);
                return Result<Goalkeeping>.Fail($"No goalkeeping data found for player {playerRefId} in current season.");
            }
            this._logger.LogInformation("Successfully retrieved goalkeeping data for player Ref ID {PlayerRefId}", playerRefId);
            return Result<Goalkeeping>.Ok(goalkeeping);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error fetching goalkeeping data for player Ref ID {PlayerRefId}: {Message}",
                playerRefId, ex.Message);
            return Result<Goalkeeping>.Fail($"Error fetching goalkeeping data for player {playerRefId}.");
        }
    }

    public async Task<Result<Shooting>> GetCurrentShootingByPlayerAsync(string playerRefId)
    {
        string correlationId = Guid.NewGuid().ToString();
        using IDisposable? scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Operation"] = nameof(this.GetCurrentShootingByPlayerAsync)
        });

        try
        {
            string? currentSeason = PremierLeagueURLS.Urls
                .SelectMany(c => c.SeasonUrls)
                .Select(s => s.Season)
                .OrderByDescending(season => season)
                .FirstOrDefault();

            Player? player = await this._unitOfWork.Players.GetByAsync(
                predicate: p => p.PlayerRefId == playerRefId && p.Season == currentSeason
            );
            if (player == null)
            {
                this._logger.LogWarning("No current season player found with Ref ID {PlayerRefId}.", playerRefId);
                return Result<Shooting>.Fail($"No current season player found with Ref ID {playerRefId}.");
            }

            string primaryPosition = this.GetPrimaryPosition(player.Position);
            if (primaryPosition == PlayerPosition.GK.ToString())
            {
                this._logger.LogWarning("Player {PlayerRefId} is a goalkeeper (primary position: {PrimaryPosition}), no shooting data available.",
                    playerRefId, primaryPosition);
                return Result<Shooting>.Fail($"Player {playerRefId} is a goalkeeper, no shooting data available.");
            }

            Shooting? shooting = await this._unitOfWork.Shootings.GetByAsync(
                predicate: s => s.Player.PlayerId == player.PlayerId && s.Season == currentSeason
            );

            if (shooting == null)
            {
                this._logger.LogWarning("No shooting data found for player Ref ID {PlayerRefId} in season {Season}.",
                    playerRefId, currentSeason);
                return Result<Shooting>.Fail($"No shooting data found for player {playerRefId} in current season.");
            }

            shooting.Position = primaryPosition;
            this._logger.LogInformation("Found shooting data: Goals = {Goals}, ShotsTotal = {ShotsTotal}, Position = {Position}",
                shooting.Goals, shooting.ShotsTotal, shooting.Position);

            //this._logger.LogInformation("Successfully retrieved shooting data for player Ref ID {PlayerRefId}", playerRefId);
            this._logger.LogInformation("Successfully retrieved shooting data for player Ref ID {PlayerRefId} with primary position {PrimaryPosition}",
                playerRefId, primaryPosition);
            return Result<Shooting>.Ok(shooting);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching shooting data for player Ref ID {PlayerRefId}: {Message}",
                playerRefId, ex.Message);
            return Result<Shooting>.Fail($"Error fetching shooting data for player {playerRefId}.");
        }
    }

    public async Task<Result<IEnumerable<ClubTrendDto>>> GetClubTrendAsync(int clubId, int numberOfSeasons = 5)
    {
        string correlationId = Guid.NewGuid().ToString();
        using IDisposable? scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Operation"] = nameof(this.GetClubTrendAsync)
        });

        try
        {
            // Lấy tất cả players của club qua các mùa
            var clubPlayers = await this._unitOfWork.Players.GetAllAsync(
                predicate: p => p.ClubId == clubId,
                include: query => query.Include(p => p.Goalkeeping)
            );

            if (!clubPlayers.Any())
            {
                _logger.LogWarning("No players found for club ID {ClubId}.", clubId);
                return Result<IEnumerable<ClubTrendDto>>.Fail($"No players found for club ID {clubId}.");
            }

            // Nhóm theo season và tính tổng
            var seasonGroups = clubPlayers
                .GroupBy(p => p.Season)
                .Select(g => new ClubTrendDto
                {
                    Season = g.Key,
                    TotalGoals = g.Sum(p => p.Goals),
                    TotalAssists = g.Sum(p => p.Assists),
                    TotalGoalsAgainst = g.Where(p => p.Goalkeeping != null)
                                    .Sum(p => p.Goalkeeping!.GoalsAgainst)
                })
                .OrderByDescending(x => x.Season)
                .Take(numberOfSeasons)
                .OrderBy(x => x.Season) // Sắp xếp tăng dần để timeline đúng
                .ToList();

            _logger.LogInformation("Retrieved club trend data for {Count} seasons for club {ClubId}",
                seasonGroups.Count, clubId);

            return Result<IEnumerable<ClubTrendDto>>.Ok(seasonGroups);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving club trend data for club {ClubId}: {Message}",
                clubId, ex.Message);
            return Result<IEnumerable<ClubTrendDto>>.Fail($"Error retrieving club trend data for club {clubId}.");
        }
    }

    #region helpers
    private PlayerSeasonComparison CreateSeasonComparison(Player current, Player previous)
    {
        int goalsDiff = current.Goals - previous.Goals;
        int assistsDiff = current.Assists - previous.Assists;
        int appearancesDiff = current.MatchPlayed - previous.MatchPlayed;

        // Calculate percentages (handle division by zero)
        double goalsPct = previous.Goals == 0 ?
            (current.Goals == 0 ? 0 : 100) :
            Math.Round((goalsDiff / (double)previous.Goals) * 100, 2);

        double assistsPct = previous.Assists == 0 ?
            (current.Assists == 0 ? 0 : 100) :
            Math.Round((assistsDiff / (double)previous.Assists) * 100, 2);

        // Calculate goals per 90 minutes
        double currentGoalsPer90 = current.Minutes > 0 ?
            Math.Round((current.Goals / (double)current.Minutes) * 90, 2) : 0;

        double previousGoalsPer90 = previous.Minutes > 0 ?
            Math.Round((previous.Goals / (double)previous.Minutes) * 90, 2) : 0;

        // Determine performance trend
        string trend = DeterminePerformanceTrend(goalsDiff, assistsDiff, goalsPct);

        return new PlayerSeasonComparison
        {
            PlayerId = current.PlayerId,
            PlayerName = current.PlayerName,
            CurrentSeason = current.Season,
            PreviousSeason = previous.Season,

            CurrentGoals = current.Goals,
            CurrentAssists = current.Assists,
            CurrentAppearances = current.MatchPlayed, // Using MatchPlayed for appearances
            CurrentMinutesPlayed = current.Minutes,

            PreviousGoals = previous.Goals,
            PreviousAssists = previous.Assists,
            PreviousAppearances = previous.MatchPlayed, // Using MatchPlayed for appearances
            PreviousMinutesPlayed = previous.Minutes,

            GoalsDifference = goalsDiff,
            AssistsDifference = assistsDiff,
            AppearancesDifference = appearancesDiff,

            GoalsChangePercentage = goalsPct,
            AssistsChangePercentage = assistsPct,
            AppearancesChangePercentage = previous.MatchPlayed == 0 ?
                (current.MatchPlayed == 0 ? 0 : 100) :
                Math.Round((appearancesDiff / (double)previous.MatchPlayed) * 100, 2),

            CurrentGoalsPer90 = currentGoalsPer90,
            PreviousGoalsPer90 = previousGoalsPer90,
            GoalsPer90Difference = Math.Round(currentGoalsPer90 - previousGoalsPer90, 2),

            PerformanceTrend = trend
        };
    }

    private string DeterminePerformanceTrend(int goalsDiff, int assistsDiff, double goalsPct)
    {
        if (goalsDiff > 0 && assistsDiff > 0 && goalsPct > 10)
            return "Significantly Improved";
        else if (goalsDiff > 0 || assistsDiff > 0)
            return "Improved";
        else if (goalsDiff == 0 && assistsDiff == 0)
            return "Stable";
        else if (goalsDiff < 0 && assistsDiff < 0 && goalsPct < -10)
            return "Significantly Declined";
        else
            return "Declined";
    }

    private string GetPreviousSeason(string currentSeason)
    {
        if (string.IsNullOrEmpty(currentSeason) || !currentSeason.Contains('-'))
            return "2024-2025";

        var parts = currentSeason.Split('-');
        if (parts.Length == 2 && int.TryParse(parts[0], out int startYear))
        {
            return $"{startYear - 1}-{startYear}";
        }

        return "2024-2025";
    }

    private string GetPrimaryPosition(string position)
    {
        if (string.IsNullOrWhiteSpace(position))
            return string.Empty;

        string[] positions = position.Split(",");
        return positions[0];
    }
    #endregion
}
