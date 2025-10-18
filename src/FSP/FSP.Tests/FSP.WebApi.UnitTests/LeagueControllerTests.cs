using FluentAssertions;
using FSP.Application.DTOs.Football;
using FSP.Application.Services;
using FSP.Domain.Entities;
using FSP.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace FSP.Tests.FSP.WebApi.UnitTests
{
    public class LeagueControllerTests
    {
        private readonly Mock<ILeagueAppService> _mockLeagueService;
        private readonly Mock<ILogger<LeagueController>> _mockLogger;
        private readonly LeagueController _controller;

        public LeagueControllerTests()
        {
            _mockLeagueService = new Mock<ILeagueAppService>();
            _mockLogger = new Mock<ILogger<LeagueController>>();
            _controller = new LeagueController(_mockLeagueService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetLeagues_WhenSuccessWithData_ReturnsOkWithData()
        {
            // Arrange
            var expectedData = new List<LeagueDto>
            {
                new LeagueDto { LeagueId = 1,  LeagueName = "Premier League", Nation = "England" },
            };
            var serviceResult = Result<IEnumerable<LeagueDto>>.Ok(expectedData);
            _mockLeagueService.Setup(s => s.GetLeaguesAsync()).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetLeagues();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedData);
            _mockLeagueService.Verify(s => s.GetLeaguesAsync(), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Information, "Fetching leagues", Times.Once());
            _mockLogger.VerifyLog(LogLevel.Information, $"Successfully retrieved {expectedData.Count} leagues", Times.Once());
        }

        [Fact]
        public async Task GetLeagues_WhenSuccessWithEmptyData_ReturnsOkWithEmptyList()
        {
            // Arrange
            var expectedData = new List<LeagueDto>();
            var serviceResult = Result<IEnumerable<LeagueDto>>.Ok(expectedData);
            _mockLeagueService.Setup(s => s.GetLeaguesAsync()).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetLeagues();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedData);
            _mockLogger.VerifyLog(LogLevel.Information, "Fetching leagues", Times.Once());
            _mockLogger.VerifyLog(LogLevel.Information, "Successfully retrieved 0 leagues", Times.Once());
        }

        [Fact]
        public async Task GetLeagues_WhenServiceFails_ReturnsBadRequest()
        {
            // Arrange
            var errorMessage = "Failed to fetch leagues";
            var serviceResult = Result<IEnumerable<LeagueDto>>.Fail(errorMessage);
            _mockLeagueService.Setup(s => s.GetLeaguesAsync()).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetLeagues();

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult!.Value.Should().Be(errorMessage);
            _mockLogger.VerifyLog(LogLevel.Warning, $"Failed to fetch leagues: {errorMessage}", Times.Once());
        }

        [Fact]
        public async Task GetLeagues_WhenServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var exceptionMessage = "Service error";
            _mockLeagueService.Setup(s => s.GetLeaguesAsync())
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetLeagues();

            // Assert
            result.Result.Should().BeOfType<ObjectResult>();
            var objectResult = result.Result as ObjectResult;
            objectResult!.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Internal server error.");
            _mockLogger.VerifyLog(LogLevel.Error, $"Error fetching leagues: {exceptionMessage}", Times.Once());
        }
    }

    // Helper extension for verifying logger calls
    public static class LoggerExtensions
    {
        public static void VerifyLog<T>(this Mock<ILogger<T>> logger, LogLevel level, string message, Times times)
        {
            logger.Verify(
                x => x.Log(
                    level,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(message)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()), times);
        }
    }
}