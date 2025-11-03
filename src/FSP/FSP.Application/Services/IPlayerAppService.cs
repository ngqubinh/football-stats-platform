using FSP.Application.DTOs.Football;
using FSP.Application.Mappings;
using FSP.Domain.Entities;
using FSP.Domain.Entities.Core;
using FSP.Domain.Interfaces.Core;

namespace FSP.Application.Services;

public interface IPlayerAppService
{
    Task<Result<IEnumerable<PlayerDto>>> GetPlayersByClubAsync(int clubId);
    Task<Result<IEnumerable<PlayerDto>>> GetCurrentPlayersByClubAsync(int clubId);
    Task<Result<IEnumerable<PlayerDto>>> GetCorePlayersByClubAsync(int clubId);
    Task<Result<IEnumerable<PlayerSeasonComparison>>> ComparePlayerWithPreviousSeasonAsync(string playerRefId);
    Task<Result<PlayerSeasonComparison>> GetPlayerCurrentVsPreviousSeasonAsync(string playerRefId);
    Task<Result<GoalkeepingDto>> GetCurrentGoalkeepingByPlayerAsync(string playerRefId);
    Task<Result<ShootingDto>> GetCurrentShootingByPlayerAsync(string playerRefId);
    Task<Result<IEnumerable<PlayerWithScoreDto>>> GetPlayerScoresByClubAsync(int clubId);
    Task<Result<LineupAnalysisDto>> AnalyzeLineupAsync(int clubId, LineupInputDto lineupInput);
    Task<Result<TwoTeamComparisonDto>> CompareTwoTeamsAsync(TwoTeamLineupInputDto twoTeamInput);
}

public class PlayerAppService : IPlayerAppService
{
    private readonly IFootballService _football;
    private readonly IPlayerMappingService _playerMappingService;

    public PlayerAppService(IFootballService football, IPlayerMappingService playerMappingService)
    {
        this._football = football;
        this._playerMappingService = playerMappingService;
    }

    public async Task<Result<IEnumerable<PlayerDto>>> GetCurrentPlayersByClubAsync(int clubId)
    {
        try
        {
            Result<IEnumerable<Player>> domainResult = await this._football.GetCurrentPlayersByClubAsync(clubId);
            if (!domainResult.Success)
                return Result<IEnumerable<PlayerDto>>.Fail(domainResult.Message!);

            IEnumerable<PlayerDto> playerDtos = this._playerMappingService.ToPlayerDtos(domainResult.Data!);
            return Result<IEnumerable<PlayerDto>>.Ok(playerDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<PlayerDto>>.Fail($"Error fetching current players for club ID {clubId}: {ex.Message}.");
        }
    }

    public async Task<Result<IEnumerable<PlayerDto>>> GetPlayersByClubAsync(int clubId)
    {
        try
        {
            Result<IEnumerable<Player>> domainResult = await this._football.GetPlayersByClubAsync(clubId);
            if (!domainResult.Success)
                return Result<IEnumerable<PlayerDto>>.Fail(domainResult.Message!);

            IEnumerable<PlayerDto> playerDtos = this._playerMappingService.ToPlayerDtos(domainResult.Data!);
            return Result<IEnumerable<PlayerDto>>.Ok(playerDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<PlayerDto>>.Fail($"Error fetching players for club ID {clubId}: {ex.Message}.");
        }
    }

    public async Task<Result<IEnumerable<PlayerDto>>> GetCorePlayersByClubAsync(int clubId)
    {
        try
        {
            Result<IEnumerable<Player>> domainResult = await this._football.GetCorePlayersByClubAsync(clubId);
            if (!domainResult.Success)
                return Result<IEnumerable<PlayerDto>>.Fail(domainResult.Message!);

            IEnumerable<PlayerDto> playerDtos = this._playerMappingService.ToPlayerDtos(domainResult.Data!);
            return Result<IEnumerable<PlayerDto>>.Ok(playerDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<PlayerDto>>.Fail($"Error fetching core players for club ID {clubId}: {ex.Message}.");
        }
    }

    public async Task<Result<IEnumerable<PlayerSeasonComparison>>> ComparePlayerWithPreviousSeasonAsync(string playerRefId)
    {
        try
        {
            var comparisonResult = await _football.ComparePlayerWithPrevisousSeasonsAsync(playerRefId);
            if (!comparisonResult.Success)
                return Result<IEnumerable<PlayerSeasonComparison>>.Fail(comparisonResult.Message!);

            return Result<IEnumerable<PlayerSeasonComparison>>.Ok(comparisonResult.Data!);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<PlayerSeasonComparison>>.Fail($"Error comparing player {playerRefId} with previous seasons: {ex.Message}.");
        }
    }

    public async Task<Result<PlayerSeasonComparison>> GetPlayerCurrentVsPreviousSeasonAsync(string playerRefId)
    {
        try
        {
            var comparisonResult = await _football.GetPlayerCurrentVsPreviousSeasonAsync(playerRefId);
            if (!comparisonResult.Success)
                return Result<PlayerSeasonComparison>.Fail(comparisonResult.Message!);

            return Result<PlayerSeasonComparison>.Ok(comparisonResult.Data!);
        }
        catch (Exception ex)
        {
            return Result<PlayerSeasonComparison>.Fail($"Error comparing current vs previous season for player {playerRefId}: {ex.Message}.");
        }
    }

    public async Task<Result<GoalkeepingDto>> GetCurrentGoalkeepingByPlayerAsync(string playerRefId)
    {
        try
        {
            var domainResult = await this._football.GetCurrentGoalkeepingByPlayerAsync(playerRefId);
            if (!domainResult.Success)
                return Result<GoalkeepingDto>.Fail(domainResult.Message!);
            GoalkeepingDto goalkeepingDto = this._playerMappingService.ToGoalkeepingDto(domainResult.Data!);
            return Result<GoalkeepingDto>.Ok(goalkeepingDto);
        }
        catch (Exception ex)
        {
            return Result<GoalkeepingDto>.Fail($"Error fetching goalkeeping for Player Ref ID {playerRefId}: {ex.Message}.");
        }
    }

    public async Task<Result<ShootingDto>> GetCurrentShootingByPlayerAsync(string playerRefId)
    {
        try
        {
            var domainResult = await this._football.GetCurrentShootingByPlayerAsync(playerRefId);
            if (!domainResult.Success)
                return Result<ShootingDto>.Fail(domainResult.Message!);
            ShootingDto shootingDto = this._playerMappingService.ToShootingDto(domainResult.Data!);
            return Result<ShootingDto>.Ok(shootingDto);
        }
        catch (Exception ex)
        {
            return Result<ShootingDto>.Fail($"Error fetching shooting for Player Ref ID {playerRefId}: {ex.Message}.");
        }
    }

    public async Task<Result<IEnumerable<PlayerWithScoreDto>>> GetPlayerScoresByClubAsync(int clubId)
    {
        try
        {
            var domainResult = await this._football.GetPlayerScoresByClubAsync(clubId);
            if (!domainResult.Success)
                return Result<IEnumerable<PlayerWithScoreDto>>.Fail(domainResult.Message!);

            var dto = this._playerMappingService.ToPlayerWithScoreDtos(domainResult.Data!);

            return Result<IEnumerable<PlayerWithScoreDto>>.Ok(dto);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<PlayerWithScoreDto>>.Fail($"Error calculating player scores for club ID {clubId}: {ex.Message}.");
        }
    }

    public async Task<Result<LineupAnalysisDto>> AnalyzeLineupAsync(int clubId, LineupInputDto lineupInput)
    {
        try
        {
            // Map DTO to domain model
            var domainInput = new LineupInput
            {
                Formation = lineupInput.Formation,
                MatchDate = lineupInput.MatchDate,
                Players = lineupInput.Players.Select(p => new LineupPlayerInput
                {
                    PlayerRefId = p.PlayerRefId,
                    PlayerName = p.PlayerName,
                    Position = p.Position,
                    IsStarting = p.IsStarting
                }).ToList()
            };

            var domainResult = await _football.AnalyzeLineupAsync(clubId, domainInput);
            if (!domainResult.Success)
                return Result<LineupAnalysisDto>.Fail(domainResult.Message!);

            // Map domain result to DTO
            var analysisDto = _playerMappingService.ToLineupAnalysisDto(domainResult.Data!);
            return Result<LineupAnalysisDto>.Ok(analysisDto);
        }
        catch (Exception ex)
        {
            return Result<LineupAnalysisDto>.Fail($"Error analyzing lineup for club {clubId}: {ex.Message}");
        }
    }

    public async Task<Result<TwoTeamComparisonDto>> CompareTwoTeamsAsync(TwoTeamLineupInputDto twoTeamInput)
    {
        try
        {
            // Map DTO to domain model
            var domainInput = new TwoTeamLineupInput
            {
                MatchDate = twoTeamInput.MatchDate,
                Venue = twoTeamInput.Venue,
                TeamA = new TeamLineupInput
                {
                    ClubId = twoTeamInput.TeamA.ClubId,
                    ClubName = twoTeamInput.TeamA.ClubName,
                    Formation = twoTeamInput.TeamA.Formation,
                    Players = twoTeamInput.TeamA.Players.Select(p => new LineupPlayerInput
                    {
                        PlayerRefId = p.PlayerRefId,
                        PlayerName = p.PlayerName,
                        Position = p.Position,
                        IsStarting = p.IsStarting
                    }).ToList()
                },
                TeamB = new TeamLineupInput
                {
                    ClubId = twoTeamInput.TeamB.ClubId,
                    ClubName = twoTeamInput.TeamB.ClubName,
                    Formation = twoTeamInput.TeamB.Formation,
                    Players = twoTeamInput.TeamB.Players.Select(p => new LineupPlayerInput
                    {
                        PlayerRefId = p.PlayerRefId,
                        PlayerName = p.PlayerName,
                        Position = p.Position,
                        IsStarting = p.IsStarting
                    }).ToList()
                }
            };

            var domainResult = await _football.CompareTwoTeamsAsync(domainInput);
            if (!domainResult.Success)
                return Result<TwoTeamComparisonDto>.Fail(domainResult.Message!);

            // Map domain result to DTO
            var comparisonDto = MapToTwoTeamComparisonDto(domainResult.Data!);
            return Result<TwoTeamComparisonDto>.Ok(comparisonDto);
        }
        catch (Exception ex)
        {
            return Result<TwoTeamComparisonDto>.Fail($"Error comparing two teams: {ex.Message}");
        }
    }

    private TwoTeamComparisonDto MapToTwoTeamComparisonDto(TwoTeamComparison domain)
    {
        return new TwoTeamComparisonDto
        {
            TeamAAnalysis = _playerMappingService.ToLineupAnalysisDto(domain.TeamAAnalysis),
            TeamBAnalysis = _playerMappingService.ToLineupAnalysisDto(domain.TeamBAnalysis),
            Comparison = new TeamComparisonDto
            {
                TotalScoreAdvantage = domain.Comparison.TotalScoreAdvantage,
                AverageScoreAdvantage = domain.Comparison.AverageScoreAdvantage,
                AdvantageTeam = domain.Comparison.AdvantageTeam,
                AdvantagePercentage = domain.Comparison.AdvantagePercentage,
                PositionAdvantages = domain.Comparison.PositionAdvantages.Select(pa => new PositionAdvantageDto
                {
                    Position = pa.Position,
                    AdvantageTeam = pa.AdvantageTeam,
                    TeamAScore = pa.TeamAScore,
                    TeamBScore = pa.TeamBScore,
                    AdvantageMargin = pa.AdvantageMargin
                }).ToList()
            },
            Prediction = new MatchPredictionDto
            {
                PredictedWinner = domain.Prediction.PredictedWinner,
                Confidence = domain.Prediction.Confidence,
                PredictedScore = domain.Prediction.PredictedScore,
                KeyFactors = domain.Prediction.KeyFactors
            },
            KeyBattles = domain.KeyBattles.Select(b => new PlayerBattleDto
            {
                Position = b.Position,
                TeamAPlayer = b.TeamAPlayer != null ? _playerMappingService.ToPlayerWithScoreDto(b.TeamAPlayer) : null,
                TeamBPlayer = b.TeamBPlayer != null ? _playerMappingService.ToPlayerWithScoreDto(b.TeamBPlayer) : null,
                Advantage = b.Advantage,
                AdvantageMargin = b.AdvantageMargin
            }).ToList()
        };
    }
}
