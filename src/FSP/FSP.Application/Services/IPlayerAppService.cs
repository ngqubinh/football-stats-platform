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
    Task<Result<IEnumerable<PlayerSeasonComparison>>> ComparePlayerWithPreviousSeasonAsync(string playerRefId);
    Task<Result<PlayerSeasonComparison>> GetPlayerCurrentVsPreviousSeasonAsync(string playerRefId);
    Task<Result<GoalkeepingDto>> GetCurrentGoalkeepingByPlayerAsync(string playerRefId);
    Task<Result<ShootingDto>> GetCurrentShootingByPlayerAsync(string playerRefId);
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
}