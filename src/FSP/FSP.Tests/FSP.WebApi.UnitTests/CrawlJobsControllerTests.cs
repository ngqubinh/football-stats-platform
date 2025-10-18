using FluentAssertions;
using FSP.Application.DTOs.Core;
using FSP.Application.Services;
using FSP.Domain.Entities;
using FSP.Domain.Entities.Core;
using FSP.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace FSP.Tests.FSP.WebApi.UnitTests;

public class CrawlJobsControllerTests
{
    private readonly Mock<ICrawlingAppService> _mockCrawlingService;
    private readonly Mock<ILogger<CrawlJobsController>> _mockLogger;
    private readonly CrawlJobsController _controller;

    public CrawlJobsControllerTests()
    {
        _mockCrawlingService = new Mock<ICrawlingAppService>();
        _mockLogger = new Mock<ILogger<CrawlJobsController>>();
        _controller = new CrawlJobsController(_mockCrawlingService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetPremierLeagueCrawl_WhenSuccessWithData_ReturnsOkWithData()
    {
        // Arrange
        var expectedData = new List<URLInformationDto>
        {
            new URLInformationDto { URL = "https://example.com", Status = "Processed" }
        };
        var serviceResult = Result<List<URLInformationDto>>.Ok(expectedData);
        _mockCrawlingService.Setup(s => s.CrawlPremierLeagueAsync()).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetPremierLeagueCrawl();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedData);
        _mockCrawlingService.Verify(s => s.CrawlPremierLeagueAsync(), Times.Once);
    }

    [Fact]
    public async Task GetPremierLeagueCrawl_WhenSuccessWithEmptyData_ReturnsEmptyList()
    {
        // Arrange
        var serviceResult = Result<List<URLInformationDto>>.Ok(new List<URLInformationDto>());
        _mockCrawlingService.Setup(s => s.CrawlPremierLeagueAsync()).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetPremierLeagueCrawl();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(new List<URLInformation>());
    }

    [Fact]
    public async Task GetPremierLeagueCrawl_WhenServiceFails_ReturnsBadRequest()
    {
        // Arrange
        var errorMessage = "Service failed";
        var serviceResult = Result<List<URLInformationDto>>.Fail(errorMessage);
        _mockCrawlingService.Setup(s => s.CrawlPremierLeagueAsync()).ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetPremierLeagueCrawl();

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task GetPremierLeagueCrawl_WhenServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var exceptionMessage = "Service error";
        _mockCrawlingService.Setup(s => s.CrawlPremierLeagueAsync())
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _controller.GetPremierLeagueCrawl();

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().BeEquivalentTo(new { Message = "Internal server error.", Error = exceptionMessage });
    }
}