using FSP.Application.DTOs.Football;
using FSP.Application.Services;
using FSP.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using FSP.Domain.Entities;
using FSP.Domain.Entities.Core;
using FluentAssertions;

namespace FSP.Tests.FSP.WebApi.UnitTests
{
    public class ClubControllerTests
    {
        private readonly Mock<IClubAppService> _mockClubService;
        private readonly Mock<ILogger<ClubController>> _mockLogger;
        private readonly ClubController _controller;

        public ClubControllerTests()
        {
            _mockClubService = new Mock<IClubAppService>();
            _mockLogger = new Mock<ILogger<ClubController>>();
            _controller = new ClubController(_mockClubService.Object, _mockLogger.Object);
        }

        #region GetClubsByLeague Tests

        [Fact]
        public async Task GetClubsByLeague_WhenSuccessWithData_ReturnsOkWithData()
        {
            // Arrange
            int leagueId = 1;
            var expectedData = new List<ClubDto>
            {
                new ClubDto { ClubId = 1, ClubName = "Manchester United", Nation = "England", LeagueId = leagueId, LeagueName = "Premier League" },
                new ClubDto { ClubId = 2, ClubName = "Liverpool", Nation = "England", LeagueId = leagueId, LeagueName = "Premier League" }
            };
            var serviceResult = Result<IEnumerable<ClubDto>>.Ok(expectedData);
            _mockClubService.Setup(s => s.GetClubsByLeagueAsync(leagueId)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetClubsByLeague(leagueId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedData);
            _mockClubService.Verify(s => s.GetClubsByLeagueAsync(leagueId), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Information, $"Fetching clubs for league ID {leagueId}", Times.Once());
            _mockLogger.VerifyLog(LogLevel.Information, $"Successfully retrieved {expectedData.Count} clubs for league ID {leagueId}", Times.Once());
        }

        [Fact]
        public async Task GetClubsByLeague_WhenSuccessWithEmptyData_ReturnsOkWithEmptyList()
        {
            // Arrange
            int leagueId = 1;
            var expectedData = new List<ClubDto>();
            var serviceResult = Result<IEnumerable<ClubDto>>.Ok(expectedData);
            _mockClubService.Setup(s => s.GetClubsByLeagueAsync(leagueId)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetClubsByLeague(leagueId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedData);
            _mockClubService.Verify(s => s.GetClubsByLeagueAsync(leagueId), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Information, $"Fetching clubs for league ID {leagueId}", Times.Once());
            _mockLogger.VerifyLog(LogLevel.Information, $"Successfully retrieved 0 clubs for league ID {leagueId}", Times.Once());
        }

        [Fact]
        public async Task GetClubsByLeague_WhenServiceFails_ReturnsBadRequest()
        {
            // Arrange
            int leagueId = 1;
            var errorMessage = "No clubs found for the specified league";
            var serviceResult = Result<IEnumerable<ClubDto>>.Fail(errorMessage);
            _mockClubService.Setup(s => s.GetClubsByLeagueAsync(leagueId)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetClubsByLeague(leagueId);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult!.Value.Should().Be(errorMessage);
            _mockClubService.Verify(s => s.GetClubsByLeagueAsync(leagueId), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Warning, $"Failed to fetch clubs for league ID {leagueId}: {errorMessage}", Times.Once());
        }

        [Fact]
        public async Task GetClubsByLeague_WhenServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            int leagueId = 1;
            var exceptionMessage = "Service error";
            _mockClubService.Setup(s => s.GetClubsByLeagueAsync(leagueId))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetClubsByLeague(leagueId);

            // Assert
            result.Result.Should().BeOfType<ObjectResult>();
            var objectResult = result.Result as ObjectResult;
            objectResult!.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Internal server error.");
            _mockClubService.Verify(s => s.GetClubsByLeagueAsync(leagueId), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Error, $"Error fetching clubs for league ID {leagueId}: {exceptionMessage}", Times.Once());
        }

        #endregion

        #region GetClubTrend Tests
        [Fact]
    public async Task GetClubTrend_WhenSuccessWithData_ReturnsOkWithData()
    {
        // Arrange
        int clubId = 1;
        int seasons = 5;
        var expectedData = new List<ClubTrendDto>
        {
            new ClubTrendDto
            {
                Season = "2023-2024",
                TotalGoals = 50,
                TotalGoalsAgainst = 30,
                TotalAssists = 40
            }
        };
        var serviceResult = Result<IEnumerable<ClubTrendDto>>.Ok(expectedData);
        _mockClubService.Setup(s => s.GetClubTrendAsync(clubId, seasons)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetClubTrend(clubId, seasons);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedData);
        _mockClubService.Verify(s => s.GetClubTrendAsync(clubId, seasons), Times.Once);
        _mockLogger.VerifyLog(LogLevel.Information, $"Fetching club trend stats for Club ID {clubId}", Times.Once());
        _mockLogger.VerifyLog(LogLevel.Information, $"Successfully retrieved club trend stats for Club ID {clubId}", Times.Once());
    }

    [Fact]
    public async Task GetClubTrend_WhenServiceFails_ReturnsBadRequest()
    {
        // Arrange
        int clubId = 1;
        int seasons = 5;
        var errorMessage = "No trend data found for the specified club";
        var serviceResult = Result<IEnumerable<ClubTrendDto>>.Fail(errorMessage);

        _mockClubService
            .Setup(s => s.GetClubTrendAsync(clubId, seasons))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetClubTrend(clubId, seasons);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be(errorMessage);

        _mockClubService.Verify(s => s.GetClubTrendAsync(clubId, seasons), Times.Once);

        _mockLogger.VerifyLog(
            LogLevel.Warning,
            $"Failed to fetch club trend stats for player {clubId}: {errorMessage}",
            Times.Once());
    }

    [Fact]
    public async Task GetClubTrend_WhenServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        int clubId = 1;
        int seasons = 5;
        var exceptionMessage = "Service error";
        _mockClubService.Setup(s => s.GetClubTrendAsync(clubId, seasons))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _controller.GetClubTrend(clubId, seasons);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Internal server error.");
        _mockClubService.Verify(s => s.GetClubTrendAsync(clubId, seasons), Times.Once);
        _mockLogger.VerifyLog(LogLevel.Error, $"Error fetching club trend stats for club {clubId}: {exceptionMessage}", Times.Once());
    }

    [Fact]
    public async Task GetClubTrend_WithDefaultSeasons_ReturnsOkWithData()
    {
        // Arrange
        int clubId = 1;
        int defaultSeasons = 5;
        var expectedData = new List<ClubTrendDto>
        {
            new ClubTrendDto
            {
                Season = "2023-2024",
                TotalGoals = 50,
                TotalGoalsAgainst = 30,
                TotalAssists = 40
            }
        };
        
        var serviceResult = Result<IEnumerable<ClubTrendDto>>.Ok(expectedData);
        _mockClubService.Setup(s => s.GetClubTrendAsync(clubId, defaultSeasons)).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetClubTrend(clubId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedData);
        _mockClubService.Verify(s => s.GetClubTrendAsync(clubId, defaultSeasons), Times.Once);
        _mockLogger.VerifyLog(LogLevel.Information, $"Fetching club trend stats for Club ID {clubId}", Times.Once());
        _mockLogger.VerifyLog(LogLevel.Information, $"Successfully retrieved club trend stats for Club ID {clubId}", Times.Once());
    }

    #endregion
    }

}