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

    public async Task<Result<IEnumerable<Player>>> GetCorePlayersByClubAsync(int clubId)
    {
        string correlationId = Guid.NewGuid().ToString();
        using IDisposable? scope = this._logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Operation"] = nameof(GetCorePlayersByClubAsync),
        });

        try
        {
            // Get current season
            string? currentSeason = PremierLeagueURLS.Urls
                .SelectMany(c => c.SeasonUrls)
                .Select(s => s.Season)
                .OrderByDescending(season => season)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(currentSeason))
            {
                _logger.LogWarning("Current season not found.");
                return Result<IEnumerable<Player>>.Fail("Current season not found.");
            }

            // Get all players for the club in current season
            IEnumerable<Player> clubPlayers = await _unitOfWork.Players.GetAllAsync(
                predicate: p => p.ClubId == clubId && p.Season == currentSeason,
                include: query => query.Include(p => p.Club)
            );

            if (!clubPlayers.Any())
            {
                this._logger.LogWarning("No players found for club ID {ClubId} in season {Season}.", clubId, currentSeason);
                return Result<IEnumerable<Player>>.Fail($"No players found for club ID {clubId} in current season.");
            }

            // Filter players with sufficient playing time (at least 5 full matches)
            var eligiblePlayers = clubPlayers.Where(p => p.Minutes >= 450).ToList();

            if (eligiblePlayers.Count < 11)
            {
                this._logger.LogWarning("Only {Count} eligible players found for club ID {ClubId}. Need at least 11 for core team.",
                    eligiblePlayers.Count, clubId);
                // Fallback: take top players by minutes if we don't have enough
                eligiblePlayers = clubPlayers.OrderByDescending(p => p.Minutes).Take(11).ToList();
            }

            // Select core 11 using position-aware selection
            var corePlayers = SelectBalancedCore11(eligiblePlayers);

            this._logger.LogInformation(
                "Selected {Count} core players for club ID {ClubId}. Formation: {Attackers} attackers, {Midfielders} midfielders, {Defenders} defenders, {Goalkeepers} goalkeepers",
                corePlayers.Count, clubId,
                corePlayers.Count(p => IsAttackingPosition(p.Position)),
                corePlayers.Count(p => IsMidfieldPosition(p.Position)),
                corePlayers.Count(p => IsDefensivePosition(p.Position)),
                corePlayers.Count(p => IsGoalkeeperPosition(p.Position))
            );

            return Result<IEnumerable<Player>>.Ok(corePlayers);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error selecting core players for club ID {ClubId}: {Message}", clubId, ex.Message);
            return Result<IEnumerable<Player>>.Fail($"Error selecting core players for club ID {clubId}.");
        }
    }

    public enum PrePlayerPoint
    {
        Nothing, // 0
        Normal, // 5
        Solid, // 7
        QuiteSolid, // 10
        VerySolid, // 15
        Impressive, // 20
        QuiteImpressive, // 25
        VeryImpressive, // 30
    }

    public async Task<Result<IEnumerable<PlayerWithScore>>> GetPlayerScoresByClubAsync(int clubId)
    {
        string correlationId = Guid.NewGuid().ToString();
        using IDisposable? scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Operation"] = nameof(GetPlayerScoresByClubAsync)
        });

        try
        {
            // Get current season
            string? currentSeason = PremierLeagueURLS.Urls
                .SelectMany(c => c.SeasonUrls)
                .Select(s => s.Season)
                .OrderByDescending(season => season)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(currentSeason))
            {
                _logger.LogWarning("Current season not found.");
                return Result<IEnumerable<PlayerWithScore>>.Fail("Current season not found.");
            }

            // Get all players for the club in current season
            IEnumerable<Player> clubPlayers = await _unitOfWork.Players.GetAllAsync(
                predicate: p => p.ClubId == clubId && p.Season == currentSeason,
                include: query => query.Include(p => p.Club)
            );

            if (!clubPlayers.Any())
            {
                _logger.LogWarning("No players found for club ID {ClubId} in season {Season}.", clubId, currentSeason);
                return Result<IEnumerable<PlayerWithScore>>.Fail($"No players found for club ID {clubId} in current season.");
            }

            // Calculate scores for all players using existing helpers
            var playersWithScores = clubPlayers.Select(player =>
            {
                var primaryPosition = GetPrimaryPosition(player.Position);
                var positionCategory = GetPositionCategory(player.Position);
                var score = CalculatePlayerScore(player, positionCategory);

                return new PlayerWithScore
                {
                    Player = player,
                    Score = score,
                    PositionCategory = positionCategory,
                    PrimaryPosition = primaryPosition
                };
            })
            .OrderByDescending(p => p.Score)
            .ThenByDescending(p => p.Player.Minutes)
            .ToList();

            _logger.LogInformation(
                "Calculated scores for {Count} players in club ID {ClubId}. " +
                "Top scorer: {TopPlayer} with {TopScore} points",
                playersWithScores.Count, clubId,
                playersWithScores.FirstOrDefault()?.Player.PlayerName,
                playersWithScores.FirstOrDefault()?.Score
            );

            return Result<IEnumerable<PlayerWithScore>>.Ok(playersWithScores);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating player scores for club ID {ClubId}: {Message}", clubId, ex.Message);
            return Result<IEnumerable<PlayerWithScore>>.Fail($"Error calculating player scores for club ID {clubId}.");
        }
    }

    public async Task<Result<LineupAnalysis>> AnalyzeLineupAsync(int clubId, LineupInput lineupInput)
    {
        string correlationId = Guid.NewGuid().ToString();
        using IDisposable? scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Operation"] = nameof(AnalyzeLineupAsync),
            ["ClubId"] = clubId
        });

        try
        {
            if (lineupInput?.Players == null || !lineupInput.Players.Any())
            {
                return Result<LineupAnalysis>.Fail("Lineup input is empty or invalid.");
            }

            // Get current season
            string? currentSeason = PremierLeagueURLS.Urls
                .SelectMany(c => c.SeasonUrls)
                .Select(s => s.Season)
                .OrderByDescending(season => season)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(currentSeason))
            {
                _logger.LogWarning("Current season not found.");
                return Result<LineupAnalysis>.Fail("Current season not found.");
            }

            // Get player data for all players in the lineup
            var lineupPlayers = new List<PlayerWithScore>();

            foreach (var playerInput in lineupInput.Players)
            {
                if (string.IsNullOrEmpty(playerInput.PlayerRefId))
                {
                    _logger.LogWarning("Skipping player without Ref ID: {PlayerName}", playerInput.PlayerName);
                    continue;
                }

                // Get player data - ensure player belongs to the specified club
                var player = await _unitOfWork.Players.GetByAsync(
                    predicate: p => p.PlayerRefId == playerInput.PlayerRefId &&
                                   p.Season == currentSeason &&
                                   p.ClubId == clubId, // Ensure player is from the specified club
                    include: query => query.Include(p => p.Club)
                );

                if (player != null)
                {
                    var primaryPosition = GetPrimaryPosition(player.Position);
                    var positionCategory = GetPositionCategory(player.Position);
                    var score = CalculatePlayerScore(player, positionCategory);

                    lineupPlayers.Add(new PlayerWithScore
                    {
                        Player = player,
                        Score = score,
                        PositionCategory = positionCategory,
                        PrimaryPosition = primaryPosition,
                        Rank = 0
                    });
                }
                else
                {
                    _logger.LogWarning("Player not found with Ref ID: {PlayerRefId} in club {ClubId}",
                        playerInput.PlayerRefId, clubId);
                }
            }

            if (!lineupPlayers.Any())
            {
                return Result<LineupAnalysis>.Fail($"No valid players found in the lineup for club ID {clubId}.");
            }

            // Get ALL players from the specified club to compare against
            var allClubPlayersResult = await GetPlayerScoresByClubAsync(clubId);

            if (!allClubPlayersResult.Success)
            {
                return Result<LineupAnalysis>.Fail($"Could not retrieve players for club ID {clubId}.");
            }

            var allClubPlayers = allClubPlayersResult.Data!.ToList();

            // Calculate analysis with comparison to club players
            var analysis = AnalyzeLineupWithComparison(lineupPlayers, allClubPlayers, lineupInput.Formation, clubId);
            
            _logger.LogInformation(
                "Analyzed lineup for club {ClubId} with {Count} players. Total score: {TotalScore}, Average: {AverageScore}, Rating: {Rating}",
                clubId, lineupPlayers.Count, analysis.TotalScore, analysis.AverageScore, analysis.OverallRating
            );

            return Result<LineupAnalysis>.Ok(analysis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing lineup for club {ClubId}: {Message}", clubId, ex.Message);
            return Result<LineupAnalysis>.Fail($"Error analyzing lineup for club {clubId}: {ex.Message}");
        }
    }

    public async Task<Result<TwoTeamComparison>> CompareTwoTeamsAsync(TwoTeamLineupInput twoTeamInput)
    {
        string correlationId = Guid.NewGuid().ToString();
        using IDisposable? scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Operation"] = nameof(CompareTwoTeamsAsync),
            ["TeamAClubId"] = twoTeamInput.TeamA.ClubId,
            ["TeamBClubId"] = twoTeamInput.TeamB.ClubId
        });

        try
        {
            if (twoTeamInput?.TeamA?.Players == null || twoTeamInput?.TeamB?.Players == null)
            {
                return Result<TwoTeamComparison>.Fail("Both team inputs are required.");
            }

            // Analyze both teams
            var teamAResult = await AnalyzeLineupAsync(twoTeamInput.TeamA.ClubId, new LineupInput
            {
                Formation = twoTeamInput.TeamA.Formation,
                Players = twoTeamInput.TeamA.Players
            });

            var teamBResult = await AnalyzeLineupAsync(twoTeamInput.TeamB.ClubId, new LineupInput
            {
                Formation = twoTeamInput.TeamB.Formation,
                Players = twoTeamInput.TeamB.Players
            });

            if (!teamAResult.Success || !teamBResult.Success)
            {
                return Result<TwoTeamComparison>.Fail(
                    $"Team A: {teamAResult.Message}, Team B: {teamBResult.Message}");
            }

            // Compare the two teams
            var comparison = CompareTeams(teamAResult.Data!, teamBResult.Data!, twoTeamInput);

            _logger.LogInformation(
                "Two-team comparison completed. Advantage: {AdvantageTeam} ({AdvantagePercentage}%)",
                comparison.Comparison.AdvantageTeam, comparison.Comparison.AdvantagePercentage
            );

            return Result<TwoTeamComparison>.Ok(comparison);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error comparing two teams: {Message}", ex.Message);
            return Result<TwoTeamComparison>.Fail($"Error comparing two teams: {ex.Message}");
        }
    }

    private TwoTeamComparison CompareTeams(LineupAnalysis teamA, LineupAnalysis teamB, TwoTeamLineupInput input)
    {
        var comparison = new TeamComparison();
        var prediction = new MatchPrediction();
        var keyBattles = new List<PlayerBattle>();

        // Calculate base scores
        float teamAScore = teamA.TotalScore;
        float teamBScore = teamB.TotalScore;

        // Apply extra metrics bonuses if provided
        if (input.TeamAExtraMetrics != null)
        {
            teamAScore = ApplyExtraMetrics(teamAScore, input.TeamAExtraMetrics, isHomeTeam: input.TeamAExtraMetrics.Home);
        }

        if (input.TeamBExtraMetrics != null)
        {
            teamBScore = ApplyExtraMetrics(teamBScore, input.TeamBExtraMetrics, isHomeTeam: input.TeamBExtraMetrics.Home);
        }

        // Calculate overall advantages with extra metrics
        comparison.TotalScoreAdvantage = teamAScore - teamBScore;
        comparison.AverageScoreAdvantage = (teamAScore / teamA.LineupPlayers.Count()) - (teamBScore / teamB.LineupPlayers.Count());

        if (comparison.TotalScoreAdvantage > 0)
        {
            comparison.AdvantageTeam = input.TeamA.ClubName;
            comparison.AdvantagePercentage = (teamAScore / (teamAScore + teamBScore)) * 100;
        }
        else
        {
            comparison.AdvantageTeam = input.TeamB.ClubName;
            comparison.AdvantagePercentage = (teamBScore / (teamAScore + teamBScore)) * 100;
        }

        // Store the adjusted scores for reference
        comparison.AdjustedTeamAScore = teamAScore;
        comparison.AdjustedTeamBScore = teamBScore;
        comparison.MatchType = input.MatchType;

        // Compare positions
        comparison.PositionAdvantages = ComparePositionAdvantages(teamA, teamB, input);

        // Generate match prediction with extra metrics
        prediction = GenerateMatchPrediction(teamA, teamB, input, comparison);

        // Identify key player battles
        keyBattles = IdentifyKeyBattles(teamA, teamB, input);

        return new TwoTeamComparison
        {
            TeamAAnalysis = teamA,
            TeamBAnalysis = teamB,
            Comparison = comparison,
            Prediction = prediction,
            KeyBattles = keyBattles
        };
    }

    private float ApplyExtraMetrics(float baseScore, ExtraMetrics extraMetrics, bool isHomeTeam = false)
    {
        float adjustedScore = baseScore;
        float bonusPoints = 0f;

        // Apply home advantage (+10 points)
        if (extraMetrics.Home)
        {
            bonusPoints += 10f;
        }

        // Apply team level (use as direct points)
        bonusPoints += extraMetrics.TeamLevel;

        // Apply motivation (+10 points)
        if (extraMetrics.Motivation)
        {
            bonusPoints += 10f;
        }

        // Apply recent form (points from last 5 matches)
        bonusPoints += extraMetrics.RecentForm;

        // Apply match type multiplier
        float matchMultiplier = GetMatchTypeMultiplier(extraMetrics.MatchType);
        adjustedScore *= matchMultiplier;

        // Add bonus points to adjusted score
        adjustedScore += bonusPoints;

        return adjustedScore;
    }

    private float GetMatchTypeMultiplier(string matcheType)
    {
        var match = new Domain.Entities.Core.MatchType();

        return matcheType.ToLower() switch
        {
            "champion" => match.Champion / 10f, // 3.0x
            "relegation" => match.Relegation / 10f, // 3.0x
            "derby" => match.Derby / 10f, // 2.0x
            "normaldomesticcup" => match.NormalDomesticCup / 10f, // 0.5x
            "domesticcup" => match.DomesticCup / 10f, // 0.8x
            "normalcontinentalcup" => match.NormalContinentalCup / 10f, // 1.2x
            "continentalcup" => match.ContinentalCup / 10f, // 4.0x
            _ => match.Normal / 10f // 1.0x (default)
        };
    }


    private List<PositionAdvantage> ComparePositionAdvantages(LineupAnalysis teamA, LineupAnalysis teamB, TwoTeamLineupInput input)
    {
        var advantages = new List<PositionAdvantage>();
        var positions = new[] { "GK", "DF", "MF", "FW" };

        foreach (var position in positions)
        {
            var teamAScore = GetPositionAverageScore(teamA, position);
            var teamBScore = GetPositionAverageScore(teamB, position);
            var advantageMargin = teamAScore - teamBScore;

            advantages.Add(new PositionAdvantage
            {
                Position = position,
                AdvantageTeam = advantageMargin > 0 ? input.TeamA.ClubName : input.TeamB.ClubName,
                TeamAScore = teamAScore,
                TeamBScore = teamBScore,
                AdvantageMargin = Math.Abs(advantageMargin)
            });
        }

        return advantages;
    }

    private float GetPositionAverageScore(LineupAnalysis analysis, string position)
    {
        return analysis.LineupPlayers
            .Where(p => p.PositionCategory == position)
            .Select(p => p.Score)
            .DefaultIfEmpty(0)
            .Average();
    }

    // private MatchPrediction GenerateMatchPrediction(LineupAnalysis teamA, LineupAnalysis teamB, TwoTeamLineupInput input, TeamComparison comparison)
    // {
    //     var prediction = new MatchPrediction();
    //     var factors = new List<string>();

    //     // Calculate score difference and determine winner
    //     var scoreDifference = Math.Abs(comparison.TotalScoreAdvantage);
    //     var totalScore = teamA.TotalScore + teamB.TotalScore;
    //     var dominanceRatio = scoreDifference / totalScore;

    //     // Determine confidence level
    //     if (dominanceRatio > 0.3)
    //     {
    //         prediction.Confidence = "High";
    //         factors.Add("Significant quality difference between teams");
    //     }
    //     else if (dominanceRatio > 0.15)
    //     {
    //         prediction.Confidence = "Medium";
    //         factors.Add("Moderate quality difference");
    //     }
    //     else
    //     {
    //         prediction.Confidence = "Low";
    //         factors.Add("Close match expected");
    //     }

    //     // Determine predicted winner
    //     if (comparison.TotalScoreAdvantage > 0)
    //     {
    //         prediction.PredictedWinner = input.TeamA.ClubName;
    //     }
    //     else if (comparison.TotalScoreAdvantage < 0)
    //     {
    //         prediction.PredictedWinner = input.TeamB.ClubName;
    //     }
    //     else
    //     {
    //         prediction.PredictedWinner = "Draw";
    //         prediction.Confidence = "Low";
    //         factors.Add("Teams are evenly matched");
    //     }

    //     // Generate predicted score based on quality difference
    //     var baseScore = 1 + (int)(dominanceRatio * 3); // 1-4 goals
    //     if (prediction.PredictedWinner == "Draw")
    //     {
    //         prediction.PredictedScore = $"{baseScore}-{baseScore}";
    //     }
    //     else if (prediction.PredictedWinner == input.TeamA.ClubName)
    //     {
    //         prediction.PredictedScore = $"{baseScore + 1}-{baseScore}";
    //     }
    //     else
    //     {
    //         prediction.PredictedScore = $"{baseScore}-{baseScore + 1}";
    //     }

    //     // Add specific factors
    //     var attackingAdvantage = comparison.PositionAdvantages.FirstOrDefault(pa => pa.Position == "FW");
    //     if (attackingAdvantage != null && attackingAdvantage.AdvantageMargin > 10)
    //     {
    //         factors.Add($"{attackingAdvantage.AdvantageTeam} has stronger attack");
    //     }

    //     var defensiveAdvantage = comparison.PositionAdvantages.FirstOrDefault(pa => pa.Position == "DF");
    //     if (defensiveAdvantage != null && defensiveAdvantage.AdvantageMargin > 10)
    //     {
    //         factors.Add($"{defensiveAdvantage.AdvantageTeam} has stronger defense");
    //     }

    //     prediction.KeyFactors = factors;
    //     return prediction;
    // }

    private MatchPrediction GenerateMatchPrediction(LineupAnalysis teamA, LineupAnalysis teamB,
    TwoTeamLineupInput input, TeamComparison comparison)
    {
        var prediction = new MatchPrediction();
        var keyFactors = new List<string>();

        // Base prediction on adjusted scores
        float teamAAdj = comparison.AdjustedTeamAScore;
        float teamBAdj = comparison.AdjustedTeamBScore;

        // Determine winner
        if (teamAAdj > teamBAdj)
        {
            prediction.PredictedWinner = input.TeamA.ClubName;
        }
        else if (teamBAdj > teamAAdj)
        {
            prediction.PredictedWinner = input.TeamB.ClubName;
        }
        else
        {
            prediction.PredictedWinner = "Draw";
        }

        // Calculate confidence based on advantage percentage
        if (comparison.AdvantagePercentage > 60)
        {
            prediction.Confidence = "High";
        }
        else if (comparison.AdvantagePercentage > 55)
        {
            prediction.Confidence = "Medium";
        }
        else
        {
            prediction.Confidence = "Low";
        }

        // Generate predicted score based on adjusted scores ratio
        var predictedScore = CalculatePredictedScore(teamAAdj, teamBAdj);
        prediction.PredictedScore = predictedScore;

        // Add key factors based on extra metrics
        if (input.TeamAExtraMetrics?.Home == true)
        {
            keyFactors.Add($"{input.TeamA.ClubName} has home advantage");
        }

        if (input.TeamAExtraMetrics?.Motivation == true)
        {
            keyFactors.Add($"{input.TeamA.ClubName} has high motivation");
        }

        if (input.TeamBExtraMetrics?.Motivation == true)
        {
            keyFactors.Add($"{input.TeamB.ClubName} has high motivation");
        }

        // Add position advantages as key factors
        var significantAdvantages = comparison.PositionAdvantages
            .Where(pa => pa.AdvantageMargin > 5)
            .OrderByDescending(pa => pa.AdvantageMargin);

        foreach (var advantage in significantAdvantages.Take(3))
        {
            keyFactors.Add($"{advantage.AdvantageTeam} dominates in {advantage.Position}");
        }

        prediction.KeyFactors = keyFactors;

        return prediction;
    }


    private string CalculatePredictedScore(float teamAScore, float teamBScore)
    {
        // Simple algorithm to convert score advantage to predicted goals
        float totalScore = teamAScore + teamBScore;
        float teamAGoals = (teamAScore / totalScore) * 3.5f; // Scale to reasonable goal count
        float teamBGoals = (teamBScore / totalScore) * 3.5f;

        // Round to nearest 0.5 and format
        int teamARounded = (int)Math.Round(teamAGoals);
        int teamBRounded = (int)Math.Round(teamBGoals);

        // Ensure at least 1 goal for the winner if significant advantage
        if (teamAScore > teamBScore * 1.2f && teamARounded == 0) teamARounded = 1;
        if (teamBScore > teamAScore * 1.2f && teamBRounded == 0) teamBRounded = 1;

        return $"{teamARounded}-{teamBRounded}";
    }


    private List<PlayerBattle> IdentifyKeyBattles(LineupAnalysis teamA, LineupAnalysis teamB, TwoTeamLineupInput input)
    {
        var battles = new List<PlayerBattle>();

        // Compare goalkeepers
        var teamAGk = teamA.LineupPlayers.FirstOrDefault(p => p.PositionCategory == "GK");
        var teamBGk = teamB.LineupPlayers.FirstOrDefault(p => p.PositionCategory == "GK");
        if (teamAGk != null && teamBGk != null)
        {
            battles.Add(CreatePlayerBattle("Goalkeeper", teamAGk, teamBGk, input));
        }

        // Compare top attackers
        var teamAAttackers = teamA.LineupPlayers.Where(p => p.PositionCategory == "FW").OrderByDescending(p => p.Score).Take(2).ToList();
        var teamBAttackers = teamB.LineupPlayers.Where(p => p.PositionCategory == "FW").OrderByDescending(p => p.Score).Take(2).ToList();

        for (int i = 0; i < Math.Min(teamAAttackers.Count, teamBAttackers.Count); i++)
        {
            battles.Add(CreatePlayerBattle($"Attacker {i + 1}", teamAAttackers[i], teamBAttackers[i], input));
        }

        // Compare midfield maestros
        var teamAMidfielders = teamA.LineupPlayers.Where(p => p.PositionCategory == "MF").OrderByDescending(p => p.Score).FirstOrDefault();
        var teamBMidfielders = teamB.LineupPlayers.Where(p => p.PositionCategory == "MF").OrderByDescending(p => p.Score).FirstOrDefault();
        if (teamAMidfielders != null && teamBMidfielders != null)
        {
            battles.Add(CreatePlayerBattle("Key Midfielder", teamAMidfielders, teamBMidfielders, input));
        }

        return battles;
    }

    private PlayerBattle CreatePlayerBattle(string position, PlayerWithScore teamAPlayer, PlayerWithScore teamBPlayer, TwoTeamLineupInput input)
    {
        var advantageMargin = teamAPlayer.Score - teamBPlayer.Score;
        return new PlayerBattle
        {
            Position = position,
            TeamAPlayer = teamAPlayer,
            TeamBPlayer = teamBPlayer,
            Advantage = advantageMargin > 0 ? input.TeamA.ClubName : input.TeamB.ClubName,
            AdvantageMargin = Math.Abs(advantageMargin)
        };
    }

    private LineupAnalysis AnalyzeLineupWithComparison(List<PlayerWithScore> lineupPlayers, List<PlayerWithScore> allClubPlayers, string formation, int clubId)
    {
        var playersList = lineupPlayers.ToList();
        var allPlayersList = allClubPlayers.ToList();

        // Calculate basic statistics
        var totalScore = playersList.Sum(p => p.Score);
        var averageScore = playersList.Average(p => p.Score);
        var clubAverageScore = allPlayersList.Average(p => p.Score);

        // Analyze position distribution
        var positionBreakdown = new PositionBreakdown
        {
            Goalkeepers = playersList.Count(p => p.PositionCategory == "GK"),
            Defenders = playersList.Count(p => p.PositionCategory == "DF"),
            Midfielders = playersList.Count(p => p.PositionCategory == "MF"),
            Attackers = playersList.Count(p => p.PositionCategory == "FW")
        };

        // Calculate average scores by position
        positionBreakdown.AvgGoalkeeperScore = playersList
            .Where(p => p.PositionCategory == "GK")
            .Select(p => p.Score)
            .DefaultIfEmpty(0)
            .Average();

        positionBreakdown.AvgDefenderScore = playersList
            .Where(p => p.PositionCategory == "DF")
            .Select(p => p.Score)
            .DefaultIfEmpty(0)
            .Average();

        positionBreakdown.AvgMidfielderScore = playersList
            .Where(p => p.PositionCategory == "MF")
            .Select(p => p.Score)
            .DefaultIfEmpty(0)
            .Average();

        positionBreakdown.AvgAttackerScore = playersList
            .Where(p => p.PositionCategory == "FW")
            .Select(p => p.Score)
            .DefaultIfEmpty(0)
            .Average();

        // Club position averages for comparison
        var clubGkAvg = allPlayersList.Where(p => p.PositionCategory == "GK").Select(p => p.Score).DefaultIfEmpty(0).Average();
        var clubDfAvg = allPlayersList.Where(p => p.PositionCategory == "DF").Select(p => p.Score).DefaultIfEmpty(0).Average();
        var clubMfAvg = allPlayersList.Where(p => p.PositionCategory == "MF").Select(p => p.Score).DefaultIfEmpty(0).Average();
        var clubFwAvg = allPlayersList.Where(p => p.PositionCategory == "FW").Select(p => p.Score).DefaultIfEmpty(0).Average();

        // Determine strengths and weaknesses
        var strengths = new List<string>();
        var weaknesses = new List<string>();

        // Lineup balance analysis
        if (positionBreakdown.Goalkeepers == 1)
            strengths.Add("Well-balanced with 1 goalkeeper");
        else if (positionBreakdown.Goalkeepers > 1)
            weaknesses.Add("Too many goalkeepers");
        else
            weaknesses.Add("No goalkeeper in lineup");

        if (positionBreakdown.Defenders >= 3 && positionBreakdown.Defenders <= 5)
            strengths.Add("Solid defensive line");
        else if (positionBreakdown.Defenders < 3)
            weaknesses.Add("Defensive line might be too weak");
        else
            weaknesses.Add("Too defensive, may lack attacking threat");

        // Score comparisons
        if (averageScore > clubAverageScore)
            strengths.Add($"Above club average score (+{averageScore - clubAverageScore:F1})");
        else if (averageScore < clubAverageScore)
            weaknesses.Add($"Below club average score ({clubAverageScore - averageScore:F1})");

        // Position-specific comparisons
        if (positionBreakdown.AvgGoalkeeperScore > clubGkAvg)
            strengths.Add("Strong goalkeeper selection");
        else if (positionBreakdown.AvgGoalkeeperScore < clubGkAvg && positionBreakdown.Goalkeepers > 0)
            weaknesses.Add("Weak goalkeeper selection");

        if (positionBreakdown.AvgDefenderScore > clubDfAvg)
            strengths.Add("Strong defensive line");
        else if (positionBreakdown.AvgDefenderScore < clubDfAvg && positionBreakdown.Defenders > 0)
            weaknesses.Add("Below average defensive line");

        if (positionBreakdown.AvgMidfielderScore > clubMfAvg)
            strengths.Add("Strong midfield");
        else if (positionBreakdown.AvgMidfielderScore < clubMfAvg && positionBreakdown.Midfielders > 0)
            weaknesses.Add("Below average midfield");

        if (positionBreakdown.AvgAttackerScore > clubFwAvg)
            strengths.Add("Strong attacking options");
        else if (positionBreakdown.AvgAttackerScore < clubFwAvg && positionBreakdown.Attackers > 0)
            weaknesses.Add("Below average attacking options");

        // Identify missing top performers
        var topClubPlayers = allPlayersList.OrderByDescending(p => p.Score).Take(5).ToList();
        var missingTopPlayers = topClubPlayers.Where(topPlayer =>
            !playersList.Any(lineupPlayer => lineupPlayer.Player.PlayerRefId == topPlayer.Player.PlayerRefId)
        ).ToList();

        if (missingTopPlayers.Any())
        {
            weaknesses.Add($"Missing top performers: {string.Join(", ", missingTopPlayers.Select(p => p.Player.PlayerName))}");
        }

        // Calculate lineup efficiency compared to optimal lineup
        var optimalLineupScore = CalculateOptimalLineupScore(allPlayersList, formation);
        var lineupEfficiency = optimalLineupScore > 0 ? (totalScore / optimalLineupScore) * 100 : 0;

        if (lineupEfficiency > 90)
            strengths.Add("Highly efficient lineup selection");
        else if (lineupEfficiency < 70)
            weaknesses.Add($"Lineup efficiency only {lineupEfficiency:F1}% of optimal");


        // Determine overall rating
        var overallRating = CalculateOverallRatingWithComparison(averageScore, clubAverageScore, strengths.Count, weaknesses.Count, lineupEfficiency);

        return new LineupAnalysis
        {
            ClubId = clubId,
            LineupPlayers = playersList,
            TotalScore = totalScore,
            AverageScore = averageScore,
            ClubAverageScore = clubAverageScore,
            Formation = formation,
            PositionBreakdown = positionBreakdown,
            Strengths = strengths,
            Weaknesses = weaknesses,
            OverallRating = overallRating,
            LineupEfficiency = lineupEfficiency,
            MissingTopPlayers = missingTopPlayers.Select(p => p.Player.PlayerName).ToList(),
            OptimalLineupScore = optimalLineupScore
        };
    }

    private float CalculateOptimalLineupScore(List<PlayerWithScore> allPlayers, string formation)
    {
        // Simple calculation - sum of top 11 players
        return allPlayers.OrderByDescending(p => p.Score).Take(11).Sum(p => p.Score);
    }

    private string CalculateOverallRatingWithComparison(float averageScore, float clubAverageScore, int strengthCount, int weaknessCount, float efficiency)
    {
        var scoreRatio = averageScore / Math.Max(clubAverageScore, 1);

        if (scoreRatio >= 1.2 && efficiency >= 90 && weaknessCount <= 1)
            return "Excellent";
        if (scoreRatio >= 1.1 && efficiency >= 80 && weaknessCount <= 2)
            return "Very Good";
        if (scoreRatio >= 1.0 && efficiency >= 70 && weaknessCount <= 3)
            return "Good";
        if (scoreRatio >= 0.9 && efficiency >= 60 && weaknessCount <= 4)
            return "Average";
        return "Needs Improvement";
    }

    private string CalculateOverallRating(float averageScore, int strengthCount, int weaknessCount)
    {
        if (averageScore >= 60 && weaknessCount <= 1) return "Excellent";
        if (averageScore >= 45 && weaknessCount <= 2) return "Very Good";
        if (averageScore >= 30 && weaknessCount <= 3) return "Good";
        if (averageScore >= 20 && weaknessCount <= 4) return "Average";
        return "Needs Improvement";
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
            CurrentAppearances = current.MatchPlayed,
            CurrentMinutesPlayed = current.Minutes,

            PreviousGoals = previous.Goals,
            PreviousAssists = previous.Assists,
            PreviousAppearances = previous.MatchPlayed,
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

    private List<Player> SelectBalancedCore11(List<Player> players)
    {
        // Group by position for balanced selection
        var goalkeepers = players.Where(p => IsGoalkeeperPosition(p.Position))
                               .OrderByDescending(p => CalculateGoalkeeperScore(p))
                               .Take(1).ToList();

        var defenders = players.Where(p => IsDefensivePosition(p.Position))
                             .OrderByDescending(p => CalculateDefensiveScore(p))
                             .Take(4).ToList();

        var midfielders = players.Where(p => IsMidfieldPosition(p.Position))
                               .OrderByDescending(p => CalculateMidfieldScore(p))
                               .Take(4).ToList();

        var attackers = players.Where(p => IsAttackingPosition(p.Position))
                             .OrderByDescending(p => CalculateAttackingScore(p))
                             .Take(2).ToList();

        // Combine the selection
        var coreTeam = new List<Player>();
        coreTeam.AddRange(goalkeepers);
        coreTeam.AddRange(defenders);
        coreTeam.AddRange(midfielders);
        coreTeam.AddRange(attackers);

        // If we don't have enough players in certain positions, fill with the next best players
        if (coreTeam.Count < 11)
        {
            var remainingSpots = 11 - coreTeam.Count;
            var remainingPlayers = players.Except(coreTeam)
                                        .OrderByDescending(p => CalculateUniversalScore(p))
                                        .Take(remainingSpots)
                                        .ToList();

            coreTeam.AddRange(remainingPlayers);
        }

        return coreTeam;
    }

    private float CalculateGoalkeeperScore(Player player)
    {
        // For goalkeepers, focus on minutes played and reliability
        float score = player.Minutes * 0.01f; // Playing time importance
        score += player.Starts * 2.0f; // Starting reliability

        // Avoid division by zero
        if (player.MatchPlayed > 0)
        {
            score += (float)player.Starts / player.MatchPlayed * 20f; // Consistency (starts per match)
        }

        // Lower score for disciplinary issues
        score -= player.YellowCards * 0.5f;
        score -= player.RedCards * 3.0f;

        return SanitizeScore(score);
    }

    private float CalculateDefensiveScore(Player player)
    {
        // For defenders, focus on reliability, discipline, and some offensive contribution
        float score = player.Minutes * 0.008f; // Playing time
        score += player.Starts * 1.5f; // Starting reliability

        // Offensive contribution for modern defenders
        score += player.Goals * 3.0f;
        score += player.Assists * 2.5f;
        score += player.ExpectedAssistedGoals * 1.2f;

        // Progressive actions
        score += player.ProgressivePasses * 0.05f;
        score += player.ProgressiveCarries * 0.05f;

        // Discipline (negative for cards)
        score -= player.YellowCards * 0.3f;
        score -= player.RedCards * 2.0f;

        return SanitizeScore(score);
    }

    private float CalculateMidfieldScore(Player player)
    {
        // For midfielders, focus on creativity, distribution, and contribution
        float score = player.Goals * 2.5f;
        score += player.Assists * 3.0f; // Higher weight for assists
        score += player.ExpectedAssistedGoals * 2.0f; // Creative expected contribution
        score += player.ExpectedGoals * 1.5f;

        // Progressive actions are crucial for midfielders
        score += player.ProgressivePasses * 0.08f;
        score += player.ProgressiveCarries * 0.08f;
        score += player.ProgressiveReceptions * 0.06f;

        // Reliability
        score += player.Minutes * 0.006f;
        score += player.Starts * 1.2f;

        // Discipline
        score -= player.YellowCards * 0.2f;
        score -= player.RedCards * 1.5f;

        return SanitizeScore(score);
    }

    private float CalculateAttackingScore(Player player)
    {
        // For attackers, focus on goal contribution and threat
        float score = player.Goals * 4.0f; // Highest weight for goals
        score += player.Assists * 2.5f;
        score += player.NonPenaltyGoals * 3.0f; // Value open play goals
        score += player.GoalsAssists * 2.0f; // Overall contribution

        // Expected metrics
        score += player.ExpectedGoals * 2.5f;
        score += player.NonPenaltyExpectedGoals * 2.0f;
        score += player.ExpectedAssistedGoals * 1.8f;
        score += player.NonPenaltyExpectedGoalsPlusAssistedGoals * 1.5f;

        // Penalty expertise
        score += player.PenaltyKicksMade * 2.0f;

        // Playing time with lower weight (attackers might be rotated more)
        score += player.Minutes * 0.005f;
        score += player.Starts * 1.0f;

        return SanitizeScore(score);
    }

    private float CalculateUniversalScore(Player player)
    {
        // Universal scoring for filling remaining spots
        float score = player.Goals * 2.0f;
        score += player.Assists * 1.8f;
        score += player.NonPenaltyGoals * 1.5f;
        score += player.ExpectedGoals * 1.2f;
        score += player.ExpectedAssistedGoals * 1.0f;
        score += player.Minutes * 0.01f;
        score += player.Starts * 1.5f;
        score -= player.YellowCards * 0.3f;
        score -= player.RedCards * 2.0f;

        return SanitizeScore(score);
    }

    private float SanitizeScore(float score)
    {
        if (float.IsInfinity(score) || float.IsNaN(score))
        {
            _logger.LogWarning("Detected invalid score value: {Score}. Replacing with 0.", score);
            return 0f;
        }

        // Ensure score is within reasonable bounds
        return Math.Max(0, score);
    }


    private bool IsGoalkeeperPosition(string position)
    {
        if (string.IsNullOrEmpty(position)) return false;
        return position.Contains("GK", StringComparison.OrdinalIgnoreCase);
    }

    private bool IsDefensivePosition(string position)
    {
        if (string.IsNullOrEmpty(position)) return false;
        var defensivePositions = new[] { "DF", "CB", "FB", "RB", "LB", "WB", "RWB", "LWB" };
        return defensivePositions.Any(pos => position.Contains(pos, StringComparison.OrdinalIgnoreCase));
    }

    private bool IsMidfieldPosition(string position)
    {
        if (string.IsNullOrEmpty(position)) return false;
        var midfieldPositions = new[] { "MF", "CM", "AM", "DM", "LM", "RM", "CAM", "CDM", "LW", "RW" };
        return midfieldPositions.Any(pos => position.Contains(pos, StringComparison.OrdinalIgnoreCase));
    }

    private bool IsAttackingPosition(string position)
    {
        if (string.IsNullOrEmpty(position)) return false;
        var attackingPositions = new[] { "FW", "ST", "CF", "SS" };
        return attackingPositions.Any(pos => position.Contains(pos, StringComparison.OrdinalIgnoreCase));
    }

    // Helper method to calculate score based on position category
    // Method 1: Original method with 2 parameters
    private float CalculatePlayerScore(Player player, string positionCategory)
    {
        return CalculatePlayerScore(player, positionCategory, "Normal");
    }


    // Method 2: New method with 3 parameters (OVERLOADING)
    private float CalculatePlayerScore(Player player, string positionCategory, string quality)
    {
        float baseScore = positionCategory switch
        {
            "GK" => CalculateGoalkeeperScore(player),
            "DF" => CalculateDefensiveScore(player),
            "MF" => CalculateMidfieldScore(player),
            "FW" => CalculateAttackingScore(player),
            _ => CalculateUniversalScore(player)
        };

        // Apply quality multiplier to the base statistical score
        float qualityMultiplier = GetQualityMultiplier(quality);
        float adjustedScore = baseScore * qualityMultiplier;

         _logger.LogDebug(
        "Player: {PlayerName}, Position: {Position}, BaseScore: {BaseScore}, " +
        "Quality: {Quality}, Multiplier: {Multiplier}, FinalScore: {FinalScore}",
        player.PlayerName, positionCategory, baseScore, quality, qualityMultiplier, adjustedScore
    );

        return adjustedScore;
    }

    private float GetQualityMultiplier(string quality)
    {
        var qualityPoints = new PlayerQualityPoint();

        float multiplier = quality?.ToLower() switch
        {
            "shit" => qualityPoints.Shit / 10f + 0.5f,        // 0.5x multiplier
            "normal" => qualityPoints.Normal / 10f + 0.8f,    // 1.1x multiplier
            "solid" => qualityPoints.Solid / 10f + 0.9f,      // 1.4x multiplier
            "quitesolid" => qualityPoints.QuiteSolid / 10f + 1.0f, // 1.7x multiplier
            "verysolid" => qualityPoints.VerySolid / 10f + 1.2f,   // 2.2x multiplier
            "impressive" => qualityPoints.Impressive / 10f + 1.4f, // 2.6x multiplier
            "quiteimpressive" => qualityPoints.QuiteImpressive / 10f + 1.6f, // 3.1x multiplier
            "veryimpressive" => qualityPoints.VeryImpressive / 10f + 2.0f,   // 4.0x multiplier
            _ => (qualityPoints.Normal / 10f) + 0.8f // Default to 1.1x
        };

        _logger.LogDebug("Quality: {Quality} -> Multiplier: {Multiplier}", quality, multiplier);
        return multiplier;
    }

    // Helper method to categorize position
    private string GetPositionCategory(string position)
    {
        if (IsGoalkeeperPosition(position)) return "GK";
        if (IsDefensivePosition(position)) return "DF";
        if (IsMidfieldPosition(position)) return "MF";
        if (IsAttackingPosition(position)) return "FW";
        return "UN"; // Unknown
    }
    #endregion



}
