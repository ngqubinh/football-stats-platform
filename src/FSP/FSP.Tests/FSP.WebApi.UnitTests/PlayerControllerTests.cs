using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using FSP.Application.DTOs.Football;
using FSP.Application.Services;
using FSP.Domain.Entities;
using FSP.Domain.Entities.Core;
using FSP.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FSP.Tests.FSP.WebApi.UnitTests
{
    public class PlayerControllerTests
    {
        private readonly Mock<IPlayerAppService> _mockPlayerService;
        private readonly Mock<ILogger<PlayerController>> _mockLogger;
        private readonly PlayerController _controller;

        public PlayerControllerTests()
        {
            _mockPlayerService = new Mock<IPlayerAppService>();
            _mockLogger = new Mock<ILogger<PlayerController>>();
            _controller = new PlayerController(_mockPlayerService.Object, _mockLogger.Object);
        }

        #region GetPlayersByClub Tests

        [Fact]
        public async Task GetPlayersByClub_WhenSuccessWithData_ReturnsOkWithData()
        {
            // Arrange
            int clubId = 1;
            var expectedData = new List<PlayerDto>
            {
                new PlayerDto { PlayerId = 1, PlayerName = "Player One", ClubId = clubId },
                new PlayerDto { PlayerId = 2, PlayerName = "Player Two", ClubId = clubId }
            };
            var serviceResult = Result<IEnumerable<PlayerDto>>.Ok(expectedData);
            _mockPlayerService.Setup(s => s.GetPlayersByClubAsync(clubId)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetPlayersByClub(clubId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedData);
            _mockPlayerService.Verify(s => s.GetPlayersByClubAsync(clubId), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Information, $"Fetching players for club ID {clubId}", Times.Once());
            _mockLogger.VerifyLog(LogLevel.Information, $"Successfully retrieved {expectedData.Count} players for club ID {clubId}", Times.Once());
        }

        [Fact]
        public async Task GetPlayersByClub_WhenSuccessWithEmptyData_ReturnsOkWithEmptyList()
        {
            // Arrange
            int clubId = 1;
            var expectedData = new List<PlayerDto>();
            var serviceResult = Result<IEnumerable<PlayerDto>>.Ok(expectedData);
            _mockPlayerService.Setup(s => s.GetPlayersByClubAsync(clubId)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetPlayersByClub(clubId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedData);
            _mockPlayerService.Verify(s => s.GetPlayersByClubAsync(clubId), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Information, $"Fetching players for club ID {clubId}", Times.Once());
            _mockLogger.VerifyLog(LogLevel.Information, $"Successfully retrieved 0 players for club ID {clubId}", Times.Once());
        }

        [Fact]
        public async Task GetPlayersByClub_WhenServiceFails_ReturnsBadRequest()
        {
            // Arrange
            int clubId = 1;
            var errorMessage = "No players found for the specified club";
            var serviceResult = Result<IEnumerable<PlayerDto>>.Fail(errorMessage);
            _mockPlayerService.Setup(s => s.GetPlayersByClubAsync(clubId)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetPlayersByClub(clubId);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult!.Value.Should().Be(errorMessage);
            _mockPlayerService.Verify(s => s.GetPlayersByClubAsync(clubId), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Warning, $"Failed to fetch players for club ID {clubId}: {errorMessage}", Times.Once());
        }

        [Fact]
        public async Task GetPlayersByClub_WhenServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            int clubId = 1;
            var exceptionMessage = "Service error";
            _mockPlayerService.Setup(s => s.GetPlayersByClubAsync(clubId))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetPlayersByClub(clubId);

            // Assert
            result.Result.Should().BeOfType<ObjectResult>();
            var objectResult = result.Result as ObjectResult;
            objectResult!.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Internal server error.");
            _mockPlayerService.Verify(s => s.GetPlayersByClubAsync(clubId), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Error, $"Error fetching players for club ID {clubId}: {exceptionMessage}", Times.Once());
        }

        #endregion

        #region GetCurrentPlayersByClub Tests

        [Fact]
        public async Task GetCurrentPlayersByClub_WhenSuccessWithData_ReturnsOkWithData()
        {
            // Arrange
            int clubId = 1;
            var expectedData = new List<PlayerDto>
            {
                new PlayerDto { PlayerId = 1, PlayerName = "Player One", ClubId = clubId }
            };
            var serviceResult = Result<IEnumerable<PlayerDto>>.Ok(expectedData);
            _mockPlayerService.Setup(s => s.GetCurrentPlayersByClubAsync(clubId)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetCurrentPlayersByClub(clubId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedData);
            _mockPlayerService.Verify(s => s.GetCurrentPlayersByClubAsync(clubId), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Information, $"Fetching current players for club ID {clubId}", Times.Once());
            _mockLogger.VerifyLog(LogLevel.Information, $"Successfully retrieved {expectedData.Count} current players for club ID {clubId}", Times.Once());
        }

        [Fact]
        public async Task GetCurrentPlayersByClub_WhenServiceFails_ReturnsBadRequest()
        {
            // Arrange
            int clubId = 1;
            var errorMessage = "No current players found";
            var serviceResult = Result<IEnumerable<PlayerDto>>.Fail(errorMessage);
            _mockPlayerService.Setup(s => s.GetCurrentPlayersByClubAsync(clubId)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetCurrentPlayersByClub(clubId);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult!.Value.Should().Be(errorMessage);
            _mockPlayerService.Verify(s => s.GetCurrentPlayersByClubAsync(clubId), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Warning, $"Failed to fetch current players for club ID {clubId}: {errorMessage}", Times.Once());
        }

        #endregion

        #region ComparePlayerWithPreviousSeasons Tests

        [Fact]
        public async Task ComparePlayerWithPreviousSeasons_WhenSuccessWithData_ReturnsOkWithData()
        {
            // Arrange
            string playerRefId = "player1";
            var expectedData = new List<PlayerSeasonComparison>
            {
                new PlayerSeasonComparison
                {
                    PlayerId = 1,
                    PlayerName = "Player One",
                    CurrentSeason = "2023-2024",
                    PreviousSeason = "2022-2023",
                    CurrentGoals = 10,
                    PreviousGoals = 8,
                    GoalsDifference = 2,
                    GoalsChangePercentage = 25.0,
                    CurrentAssists = 5,
                    PreviousAssists = 3,
                    AssistsDifference = 2,
                    AssistsChangePercentage = 66.67,
                    CurrentAppearances = 20,
                    PreviousAppearances = 18,
                    AppearancesDifference = 2,
                    AppearancesChangePercentage = 11.11,
                    CurrentMinutesPlayed = 1800,
                    PreviousMinutesPlayed = 1620,
                    CurrentGoalsPer90 = 0.5,
                    PreviousGoalsPer90 = 0.44,
                    GoalsPer90Difference = 0.06,
                    PerformanceTrend = "Improved"
                }
            };
            var serviceResult = Result<IEnumerable<PlayerSeasonComparison>>.Ok(expectedData);
            _mockPlayerService.Setup(s => s.ComparePlayerWithPreviousSeasonAsync(playerRefId)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.ComparePlayerWithPreviousSeasons(playerRefId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedData);
            _mockPlayerService.Verify(s => s.ComparePlayerWithPreviousSeasonAsync(playerRefId), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Information, $"Comparing player {playerRefId} with previous seasons", Times.Once());
            _mockLogger.VerifyLog(LogLevel.Information, $"Successfully retrieved {expectedData.Count} season comparisons for player Ref ID {playerRefId}", Times.Once());
        }

        [Fact]
        public async Task ComparePlayerWithPreviousSeasons_WhenSuccessWithEmptyData_ReturnsOkWithEmptyList()
        {
            // Arrange
            string playerRefId = "player1";
            var expectedData = new List<PlayerSeasonComparison>();
            var serviceResult = Result<IEnumerable<PlayerSeasonComparison>>.Ok(expectedData);
            _mockPlayerService.Setup(s => s.ComparePlayerWithPreviousSeasonAsync(playerRefId)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.ComparePlayerWithPreviousSeasons(playerRefId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedData);
            _mockPlayerService.Verify(s => s.ComparePlayerWithPreviousSeasonAsync(playerRefId), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Information, $"Comparing player {playerRefId} with previous seasons", Times.Once());
            _mockLogger.VerifyLog(LogLevel.Information, $"Successfully retrieved 0 season comparisons for player Ref ID {playerRefId}", Times.Once());
        }

        [Fact]
        public async Task ComparePlayerWithPreviousSeasons_WhenServiceFails_ReturnsBadRequest()
        {
            // Arrange
            string playerRefId = "player1";
            var errorMessage = "No comparison data found";
            var serviceResult = Result<IEnumerable<PlayerSeasonComparison>>.Fail(errorMessage);
            _mockPlayerService.Setup(s => s.ComparePlayerWithPreviousSeasonAsync(playerRefId)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.ComparePlayerWithPreviousSeasons(playerRefId);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult!.Value.Should().Be(errorMessage);
            _mockPlayerService.Verify(s => s.ComparePlayerWithPreviousSeasonAsync(playerRefId), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Warning, $"Failed to compare player {playerRefId} with previous seasons: {errorMessage}", Times.Once());
        }

        [Fact]
        public async Task ComparePlayerWithPreviousSeasons_WhenServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            string playerRefId = "player1";
            var exceptionMessage = "Service error";
            _mockPlayerService.Setup(s => s.ComparePlayerWithPreviousSeasonAsync(playerRefId))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.ComparePlayerWithPreviousSeasons(playerRefId);

            // Assert
            result.Result.Should().BeOfType<ObjectResult>();
            var objectResult = result.Result as ObjectResult;
            objectResult!.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Internal server error.");
            _mockPlayerService.Verify(s => s.ComparePlayerWithPreviousSeasonAsync(playerRefId), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Error, $"Error comparing player {playerRefId} with previous seasons: {exceptionMessage}", Times.Once());
        }

        #endregion

        #region GetPlayerCurrentVsPreviousSeason Tests

        [Fact]
        public async Task GetPlayerCurrentVsPreviousSeason_WhenSuccessWithData_ReturnsOkWithData()
        {
            // Arrange
            string playerRefId = "player1";
            var expectedData = new PlayerSeasonComparison
            {
                PlayerId = 1,
                PlayerName = "Player One",
                CurrentSeason = "2023-2024",
                PreviousSeason = "2022-2023",
                CurrentGoals = 10,
                PreviousGoals = 8,
                GoalsDifference = 2,
                GoalsChangePercentage = 25.0,
                CurrentAssists = 5,
                PreviousAssists = 3,
                AssistsDifference = 2,
                AssistsChangePercentage = 66.67,
                CurrentAppearances = 20,
                PreviousAppearances = 18,
                AppearancesDifference = 2,
                AppearancesChangePercentage = 11.11,
                CurrentMinutesPlayed = 1800,
                PreviousMinutesPlayed = 1620,
                CurrentGoalsPer90 = 0.5,
                PreviousGoalsPer90 = 0.44,
                GoalsPer90Difference = 0.06,
                PerformanceTrend = "Improved"
            };

            var serviceResult = Result<PlayerSeasonComparison>.Ok(expectedData);
            _mockPlayerService.Setup(s => s.GetPlayerCurrentVsPreviousSeasonAsync(playerRefId)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetPlayerCurrentVsPreviousSeason(playerRefId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedData);
            _mockPlayerService.Verify(s => s.GetPlayerCurrentVsPreviousSeasonAsync(playerRefId), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Information, $"Comparing current vs previous season for player {playerRefId}", Times.Once());
            _mockLogger.VerifyLog(LogLevel.Information, $"Successfully retrieved current vs previous season comparison for player ID {playerRefId}", Times.Once());
        }

        [Fact]
        public async Task GetPlayerCurrentVsPreviousSeason_WhenServiceFails_ReturnsBadRequest()
        {
            // Arrange
            string playerRefId = "player1";
            var errorMessage = "No comparison data found";
            var serviceResult = Result<PlayerSeasonComparison>.Fail(errorMessage);
            _mockPlayerService.Setup(s => s.GetPlayerCurrentVsPreviousSeasonAsync(playerRefId)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetPlayerCurrentVsPreviousSeason(playerRefId);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult!.Value.Should().Be(errorMessage);
            _mockPlayerService.Verify(s => s.GetPlayerCurrentVsPreviousSeasonAsync(playerRefId), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Warning, $"Failed to compare current vs previous season for player {playerRefId}: {errorMessage}", Times.Once());
        }

        #endregion

        #region GetCurrentGoalkeepingByPlayer Tests

        [Fact]
        public async Task GetCurrentGoalkeepingByPlayer_WhenSuccessWithData_ReturnsOkWithData()
        {
            // Arrange
            string playerRefId = "player1";
            var expectedData = new GoalkeepingDto { GoalkeepingId = 1, PlayerName = "Player One", Saves = "10" };
            var serviceResult = Result<GoalkeepingDto>.Ok(expectedData);
            _mockPlayerService.Setup(s => s.GetCurrentGoalkeepingByPlayerAsync(playerRefId)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetCurrentGoalkeepingByPlayer(playerRefId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedData);
            _mockPlayerService.Verify(s => s.GetCurrentGoalkeepingByPlayerAsync(playerRefId), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Information, $"Fetching goalkeeping stats for player {playerRefId}", Times.Once());
            _mockLogger.VerifyLog(LogLevel.Information, $"Successfully retrieved goalkeeping stats for player Ref ID {playerRefId}", Times.Once());
        }

        [Fact]
        public async Task GetCurrentGoalkeepingByPlayer_WhenServiceFails_ReturnsBadRequest()
        {
            // Arrange
            string playerRefId = "player1";
            var errorMessage = "No goalkeeping stats found";
            var serviceResult = Result<GoalkeepingDto>.Fail(errorMessage);
            _mockPlayerService.Setup(s => s.GetCurrentGoalkeepingByPlayerAsync(playerRefId)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetCurrentGoalkeepingByPlayer(playerRefId);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult!.Value.Should().Be(errorMessage);
            _mockPlayerService.Verify(s => s.GetCurrentGoalkeepingByPlayerAsync(playerRefId), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Warning, $"Failed to fetch goalkeeping stats for player {playerRefId}: {errorMessage}", Times.Once());
        }

        #endregion

        #region GetCurrentShootingByPlayer Tests

        [Fact]
        public async Task GetCurrentShootingByPlayer_WhenSuccessWithData_ReturnsOkWithData()
        {
            // Arrange
            string playerRefId = "player1";
            var expectedData = new ShootingDto { ShootingId = 1, PlayerName = "Player One", Goals = 5 };
            var serviceResult = Result<ShootingDto>.Ok(expectedData);
            _mockPlayerService.Setup(s => s.GetCurrentShootingByPlayerAsync(playerRefId)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetCurrentShootingByPlayer(playerRefId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedData);
            _mockPlayerService.Verify(s => s.GetCurrentShootingByPlayerAsync(playerRefId), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Information, $"Fetching shooting stats for player {playerRefId}", Times.Once());
            _mockLogger.VerifyLog(LogLevel.Information, $"Successfully retrieved shooting stats for player Ref ID {playerRefId}", Times.Once());
        }

        [Fact]
        public async Task GetCurrentShootingByPlayer_WhenServiceFails_ReturnsBadRequest()
        {
            // Arrange
            string playerRefId = "player1";
            var errorMessage = "No shooting stats found";
            var serviceResult = Result<ShootingDto>.Fail(errorMessage);
            _mockPlayerService.Setup(s => s.GetCurrentShootingByPlayerAsync(playerRefId)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetCurrentShootingByPlayer(playerRefId);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult!.Value.Should().Be(errorMessage);
            _mockPlayerService.Verify(s => s.GetCurrentShootingByPlayerAsync(playerRefId), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Warning, $"Failed to fetch shooting stats for player {playerRefId}: {errorMessage}", Times.Once());
        }

        #endregion
    }
}