using FluentAssertions;
using FSP.Application.DTOs.Core;
using FSP.Application.DTOs.Football;
using FSP.Application.Services;
using FSP.Domain.Entities;
using FSP.Domain.Entities.Core;
using FSP.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;

namespace FSP.Tests.FSP.WebApi.UnitTests;

public class SimpleCrawlerControllerTests
{
    private readonly Mock<ISimpleCrawlerAppService> _mockCrawlerService;
    private readonly Mock<ILogger<SimpleCrawlerController>> _mockLogger;
    private readonly SimpleCrawlerController _controller;

    public SimpleCrawlerControllerTests()
    {
        _mockCrawlerService = new Mock<ISimpleCrawlerAppService>();
        _mockLogger = new Mock<ILogger<SimpleCrawlerController>>();
        _controller = new SimpleCrawlerController(_mockCrawlerService.Object, _mockLogger.Object);
    }

    #region Test Data Helpers

    private PlayerDto CreateTestPlayer()
    {
        return new PlayerDto
        {
            PlayerId = 1,
            PlayerName = "Test Player",
            Nation = "England",
            Position = "Forward",
            Age = "25",
            MatchPlayed = 10,
            Starts = 8,
            Minutes = 720,
            NinetyMinutes = "8.0",
            Goals = 5,
            Assists = 3,
            GoalsAssists = 8,
            NonPenaltyGoals = 4,
            PenaltyKicksMade = 1,
            PenaltyKickAttempted = 1,
            YellowCards = 2,
            RedCards = 0,
            ExpectedGoals = 4.5f,
            NonPenaltyExpectedGoals = 4.0f,
            ExpectedAssistedGoals = 2.8f,
            NonPenaltyExpectedGoalsPlusAssistedGoals = 6.8f,
            ProgressiveCarries = 45,
            ProgressivePasses = 32,
            ProgressiveReceptions = 28,
            GoalsPer90s = "0.63",
            AssistsPer90s = "0.38",
            GoalsAssistsPer90s = "1.00",
            NonPenaltyGoalsPer90s = "0.50",
            NonPenaltyGoalsAssistsPer90s = "0.88",
            ExpectedGoalsPer90 = "0.56",
            ExpectedAssistedGoalsPer90 = "0.35",
            ExpectedGoalsPlusAssistedGoalsPer90 = "0.91",
            NonPenaltyExpectedGoalsPer90 = "0.50",
            NonPenaltyExpectedGoalsPlusAssistedGoalsPer90 = "0.85",
            ClubId = 123,
            ClubName = "Test Club",
            PlayerRefId = "player123",
            Season = "2023/2024"
        };
    }

    private GoalkeepingDto CreateTestGoalkeeping()
    {
        return new GoalkeepingDto
        {
            GoalkeepingId = 1,
            PlayerName = "Test Goalkeeper",
            Nation = "Spain",
            Position = "Goalkeeper",
            Age = "28",
            MatchPlayed = 5,
            Starts = 5,
            Minutes = "450",
            NineteenMinutes = "5.0",
            GoalsAgainst = 3,
            GoalsAssistsPer90s = "0.00",
            ShotsOnTargetAgainst = "15",
            Saves = "12",
            SavePercentage = "80.0",
            Wins = 3,
            Draws = 1,
            Losses = 1,
            CleanSheets = 2,
            CleanSheetsPercentage = "40.0",
            PenaltyKicksAttempted = "1",
            PenaltyKicksAllowed = "0",
            PenaltyKicksSaved = "1",
            PenaltyKicksMissed = "0",
            PenaltyKicksSavedPercentage = "100.0",
            Season = "2023/2024",
            PlayerId = 2,
            PlayerRefId = "gk123"
        };
    }

    private ShootingDto CreateTestShooting()
    {
        return new ShootingDto
        {
            ShootingId = 1,
            PlayerName = "Test Shooter",
            Nation = "Brazil",
            Position = "Forward",
            Age = "24",
            NineteenMinutes = "7.5",
            Goals = 6,
            ShotsTotal = 25,
            ShotsOnTarget = 12,
            ShotsOnTargetPercentage = "48.0",
            ShotsTotalPer90 = "3.33",
            ShotsOnTargetPer90 = "1.60",
            GoalsShots = "24.0",
            GoalsShotsOnTarget = "50.0",
            AverageShotDistance = "16.5",
            PenaltyKicksMade = 0,
            PenaltyKicksAttempted = 0,
            Season = "2023/2024",
            PlayerId = 3,
            PlayerRefId = "shooter123"
        };
    }

    private SquadStandardDto CreateTestSquadStandard()
    {
        return new SquadStandardDto
        {
            Squad = "Test Squad",
            NumberOfPlayers = 25,
            AverageAge = 26.5f,
            Possession = 55.5f,
            MatchesPlayed = 10,
            Starts = 220,
            Minutes = 19800,
            Nineties = 220.0f,
            Goals = 15,
            Assists = 12,
            GoalsPlusAssists = 27,
            NonPenaltyGoals = 13,
            PenaltyKicksMade = 2,
            PenaltyKicksAttempted = 2,
            YellowCards = 18,
            RedCards = 1,
            ExpectedGoals = 14.8f,
            NonPenaltyExpectedGoals = 12.8f,
            ExpectedAssistedGoals = 11.2f,
            NonPenaltyExpectedGoalsPlusAssistedGoals = 24.0f,
            ProgressiveCarries = 320,
            ProgressivePasses = 280,
            GoalsPer90 = 1.5f,
            AssistsPer90 = 1.2f,
            GoalsPlusAssistsPer90 = 2.7f,
            NonPenaltyGoalsPer90 = 1.3f,
            GoalsPlusAssistsMinusPkPer90 = 2.5f,
            ExpectedGoalsPer90 = 1.48f,
            ExpectedAssistedGoalsPer90 = 1.12f,
            ExpectedGoalsPlusAssistedGoalsPer90 = 2.6f,
            NonPenaltyExpectedGoalsPer90 = 1.28f,
            NonPenaltyExpectedGoalsPlusAssistedGoalsPer90 = 2.4f
        };
    }

    private PlayerDetails CreateTestPlayerDetails()
    {
        return new PlayerDetails
        {
            FullName = "Test Player Details",
            OriginalName = "English",
        };
    }

    private CompleteTeamDataDto CreateTestCompleteTeamData()
    {
        return new CompleteTeamDataDto
        {
            TeamName = "Test Team",
            Players = new List<PlayerDto> { CreateTestPlayer() },
            Goalkeeping = new List<GoalkeepingDto> { CreateTestGoalkeeping() },
            Shooting = new List<ShootingDto> { CreateTestShooting() },
            MatchLogs = new List<MatchLogDto>
            {
                new MatchLogDto
                {
                    // Add MatchLogDto properties as needed
                    Time = DateTime.Now.ToString(),
                    Result = "W"
                }
            }
        };
    }

    #endregion

    #region ExtractPlayers Tests

    [Fact]
    public async Task ExtractPlayers_WhenUrlIsEmpty_ReturnsBadRequest()
    {
        // Arrange
        var emptyUrl = "";

        // Act
        var result = await _controller.ExtractPlayers(emptyUrl);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("URL parameter is required");
    }

    [Fact]
    public async Task ExtractPlayers_WhenSuccessWithData_ReturnsOkWithData()
    {
        // Arrange
        var url = "https://example.com/players";
        var expectedData = new List<PlayerDto> { CreateTestPlayer() };
        var serviceResult = Result<List<PlayerDto>>.Ok(expectedData);
        _mockCrawlerService.Setup(s => s.ExtractPlayersAsync(url, It.IsAny<string>()))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.ExtractPlayers(url);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedData);
        _mockCrawlerService.Verify(s => s.ExtractPlayersAsync(url, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ExtractPlayers_WhenSuccessWithEmptyData_ReturnsEmptyList()
    {
        // Arrange
        var url = "https://example.com/players";
        var serviceResult = Result<List<PlayerDto>>.Ok(new List<PlayerDto>());
        _mockCrawlerService.Setup(s => s.ExtractPlayersAsync(url, It.IsAny<string>()))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.ExtractPlayers(url);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(new List<PlayerDto>());
    }

    [Fact]
    public async Task ExtractPlayers_WhenServiceFails_ReturnsBadRequest()
    {
        // Arrange
        var url = "https://example.com/players";
        var errorMessage = "Failed to extract players";
        var serviceResult = Result<List<PlayerDto>>.Fail(errorMessage);
        _mockCrawlerService.Setup(s => s.ExtractPlayersAsync(url, It.IsAny<string>()))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.ExtractPlayers(url);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task ExtractPlayers_WhenServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var url = "https://example.com/players";
        var exceptionMessage = "Service error";
        _mockCrawlerService.Setup(s => s.ExtractPlayersAsync(url, It.IsAny<string>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _controller.ExtractPlayers(url);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().BeEquivalentTo(new { Message = "Internal server error.", Error = exceptionMessage });
    }

    #endregion

    #region ExtractGoalkeeping Tests

    [Fact]
    public async Task ExtractGoalkeeping_WhenUrlIsEmpty_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.ExtractGoalkeeping("");

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ExtractGoalkeeping_WhenSuccessWithData_ReturnsOkWithData()
    {
        // Arrange
        var url = "https://example.com/goalkeeping";
        var expectedData = new List<GoalkeepingDto> { CreateTestGoalkeeping() };
        var serviceResult = Result<List<GoalkeepingDto>>.Ok(expectedData);
        _mockCrawlerService.Setup(s => s.ExtractGoalkeepingAsync(url, It.IsAny<string>()))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.ExtractGoalkeeping(url);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedData);
    }

    [Fact]
    public async Task ExtractGoalkeeping_WhenServiceFails_ReturnsBadRequest()
    {
        // Arrange
        var url = "https://example.com/goalkeeping";
        var errorMessage = "Failed to extract goalkeeping data";
        var serviceResult = Result<List<GoalkeepingDto>>.Fail(errorMessage);
        _mockCrawlerService.Setup(s => s.ExtractGoalkeepingAsync(url, It.IsAny<string>()))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.ExtractGoalkeeping(url);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region ExtractShooting Tests

    [Fact]
    public async Task ExtractShooting_WhenUrlIsEmpty_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.ExtractShooting("");

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ExtractShooting_WhenSuccessWithData_ReturnsOkWithData()
    {
        // Arrange
        var url = "https://example.com/shooting";
        var expectedData = new List<ShootingDto> { CreateTestShooting() };
        var serviceResult = Result<List<ShootingDto>>.Ok(expectedData);
        _mockCrawlerService.Setup(s => s.ExtractShootingAsync(url, It.IsAny<string>()))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.ExtractShooting(url);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedData);
    }

    #endregion

    #region ExtractMatchLogs Tests

    [Fact]
    public async Task ExtractMatchLogs_WhenUrlIsEmpty_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.ExtractMatchLogs("");

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ExtractMatchLogs_WhenSuccessWithData_ReturnsOkWithData()
    {
        // Arrange
        var url = "https://example.com/matchlogs";
        var expectedData = new List<MatchLogDto>
        {
            new MatchLogDto { Time = DateTime.Now.ToString(), Result = "W" }
        };
        var serviceResult = Result<List<MatchLogDto>>.Ok(expectedData);
        _mockCrawlerService.Setup(s => s.ExtractMatchLogsAsync(url, It.IsAny<string>()))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.ExtractMatchLogs(url);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedData);
    }

    #endregion

    #region ExtractPlayerDetails Tests

    [Fact]
    public async Task ExtractPlayerDetails_WhenUrlIsEmpty_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.ExtractPlayerDetails("");

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ExtractPlayerDetails_WhenSuccessWithData_ReturnsOkWithData()
    {
        // Arrange
        var url = "https://example.com/player";
        var expectedData = CreateTestPlayerDetails();
        var serviceResult = Result<PlayerDetails>.Ok(expectedData);
        _mockCrawlerService.Setup(s => s.ExtractPlayerDetailsAsync(url, It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.ExtractPlayerDetails(url);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedData);
    }

    [Fact]
    public async Task ExtractPlayerDetails_WhenServiceFails_ReturnsBadRequest()
    {
        // Arrange
        var url = "https://example.com/player";
        var errorMessage = "Failed to extract player details";
        var serviceResult = Result<PlayerDetails>.Fail(errorMessage);
        _mockCrawlerService.Setup(s => s.ExtractPlayerDetailsAsync(url, It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.ExtractPlayerDetails(url);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be(errorMessage);
    }

    #endregion

    #region GetRawHtml Tests

    [Fact]
    public async Task GetRawHtml_WhenUrlIsEmpty_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.GetRawHtml("");

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetRawHtml_WhenSuccessWithData_ReturnsContentResult()
    {
        // Arrange
        var url = "https://example.com";
        var expectedHtml = "<html><body>Test</body></html>";
        var serviceResult = Result<string>.Ok(expectedHtml);
        _mockCrawlerService.Setup(s => s.GetRawHtmlAsync(url))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.GetRawHtml(url);

        // Assert
        result.Result.Should().BeOfType<ContentResult>();
        var contentResult = result.Result as ContentResult;
        contentResult!.Content.Should().Be(expectedHtml);
        contentResult.ContentType.Should().Be("text/html");
    }

    #endregion

    #region ExtractAllData Tests

    [Fact]
    public async Task ExtractAllData_WhenUrlIsEmpty_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.ExtractAllData("", "test-id");

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ExtractAllData_WhenSuccessWithData_ReturnsEnhancedResponse()
    {
        // Arrange
        var url = "https://example.com/team";
        var id = "team123";
        var expectedData = CreateTestCompleteTeamData();
        var serviceResult = Result<CompleteTeamDataDto>.Ok(expectedData);
        _mockCrawlerService.Setup(s => s.ExtractAllDataAsync(url, id))
            .ReturnsAsync(serviceResult);

        // Setup URL helper for the controller
        var controllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
            {
                Request = { Scheme = "https" }
            }
        };
        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper.Setup(u => u.Action(It.IsAny<UrlActionContext>()))
            .Returns<UrlActionContext>(context => $"/api/simplecrawler/{context.Action}?url={url}&id={id}");
        _controller.Url = mockUrlHelper.Object;
        _controller.ControllerContext = controllerContext;

        // Act
        var result = await _controller.ExtractAllData(url, id);

        // Assert
        var objectResult = result.Result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(200);
        objectResult.Value.Should().BeOfType<EnhancedTeamDataResponse>();

        var response = objectResult.Value as EnhancedTeamDataResponse;
        response!.Data.Should().BeEquivalentTo(expectedData);
        response.DownloadLinks.Should().NotBeNull();
    }

    [Fact]
    public async Task ExtractAllData_WhenServiceFails_ReturnsBadRequest()
    {
        // Arrange
        var url = "https://example.com/team";
        var id = "team123";
        var errorMessage = "Failed to extract all data";
        var serviceResult = Result<CompleteTeamDataDto>.Fail(errorMessage);
        _mockCrawlerService.Setup(s => s.ExtractAllDataAsync(url, id))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.ExtractAllData(url, id);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be(errorMessage);
    }

    #endregion

    #region DownloadJSON Tests

    [Fact]
    public async Task DownloadJSON_WhenServiceFails_ReturnsBadRequest()
    {
        // Arrange
        var url = "https://example.com/team";
        var id = "team123";
        var errorMessage = "Failed to extract data";
        var serviceResult = Result<CompleteTeamDataDto>.Fail(errorMessage);
        _mockCrawlerService.Setup(s => s.ExtractAllDataAsync(url, id))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.DownloadJSON(url, id);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task DownloadJSON_WhenSuccess_ReturnsFileResult()
    {
        // Arrange
        var url = "https://example.com/team";
        var id = "team123";
        var expectedData = CreateTestCompleteTeamData();
        var serviceResult = Result<CompleteTeamDataDto>.Ok(expectedData);
        _mockCrawlerService.Setup(s => s.ExtractAllDataAsync(url, id))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.DownloadJSON(url, id);

        // Assert
        result.Should().BeOfType<FileContentResult>();
        var fileResult = result as FileContentResult;
        fileResult!.ContentType.Should().Be("application/json");
        fileResult.FileDownloadName.Should().NotBeNullOrEmpty();
        fileResult.FileDownloadName.Should().Contain(".json");
    }

    #endregion

    #region ExtractSquadStandard Tests

    [Fact]
    public async Task ExtractSquadStandard_WhenUrlIsEmpty_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.ExtractSquadStandard("");

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

   [Fact]
    public async Task ExtractSquadStandard_WhenSuccessWithData_ReturnsEnhancedResponse()
    {
        // Arrange
        var url = "https://example.com/squad";
        var expectedData = new List<SquadStandardDto> { CreateTestSquadStandard() };
        var serviceResult = Result<List<SquadStandardDto>>.Ok(expectedData);
        _mockCrawlerService.Setup(s => s.ExtractSquadStandardFromUrlAsync(url, It.IsAny<string>()))
            .ReturnsAsync(serviceResult);

        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(httpContext, new RouteData(), new ControllerActionDescriptor());
        _controller.ControllerContext = new ControllerContext(actionContext);

        // Mock UrlHelper so Url.Action returns fake links
        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper
            .Setup(u => u.Action(It.IsAny<UrlActionContext>()))
            .Returns<UrlActionContext>(ctx => $"/fake/{ctx.Action}");
        _controller.Url = mockUrlHelper.Object;

        // Act
        var result = await _controller.ExtractSquadStandard(url);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();

        var response = okResult.Value as EnhancedSquadResponse;
        response!.Data.Should().BeEquivalentTo(expectedData);
        response.DownloadLinks.Should().NotBeNull();

        response.DownloadLinks.Json.Should().Contain("DownloadSquadStandardJSON");
        response.DownloadLinks.Zip.Should().Contain("DownloadSquadStandardZip");

        var json = JsonSerializer.Serialize(response.DownloadLinks);
        json.Should().Contain("DownloadSquadStandardJSON");
        json.Should().Contain("DownloadSquadStandardZip");
        json.Should().Contain("\"Json\"");
        json.Should().Contain("\"Zip\"");
    }


    #endregion

    #region DownloadSquadStandardJSON Tests

    [Fact]
    public async Task DownloadSquadStandardJSON_WhenUrlIsEmpty_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.DownloadSquadStandardJSON("");

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task DownloadSquadStandardJSON_WhenServiceFails_ReturnsBadRequest()
    {
        // Arrange
        var url = "https://example.com/squad";
        var errorMessage = "Failed to extract squad standard data";
        var serviceResult = Result<List<SquadStandardDto>>.Fail(errorMessage);
        _mockCrawlerService.Setup(s => s.ExtractSquadStandardFromUrlAsync(url, It.IsAny<string>()))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.DownloadSquadStandardJSON(url);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task DownloadSquadStandardJSON_WhenSuccess_ReturnsFileResult()
    {
        // Arrange
        var url = "https://example.com/squad";
        var expectedData = new List<SquadStandardDto> { CreateTestSquadStandard() };
        var serviceResult = Result<List<SquadStandardDto>>.Ok(expectedData);
        _mockCrawlerService.Setup(s => s.ExtractSquadStandardFromUrlAsync(url, It.IsAny<string>()))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.DownloadSquadStandardJSON(url);

        // Assert
        result.Should().BeOfType<FileContentResult>();
        var fileResult = result as FileContentResult;
        fileResult!.ContentType.Should().Be("application/json");
        fileResult.FileDownloadName.Should().NotBeNullOrEmpty();
        fileResult.FileDownloadName.Should().Contain("squad_standard");
        fileResult.FileDownloadName.Should().Contain(".json");
    }

    #endregion

    #region Exception Handling Tests

    [Fact]
    public async Task ExtractPlayers_WhenServiceThrowsException_LogsErrorAndReturnsInternalServerError()
    {
        // Arrange
        var url = "https://example.com/players";
        var exceptionMessage = "Database connection failed";
        _mockCrawlerService.Setup(s => s.ExtractPlayersAsync(url, It.IsAny<string>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _controller.ExtractPlayers(url);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().BeEquivalentTo(new { Message = "Internal server error.", Error = exceptionMessage });
    }

    [Fact]
    public async Task DownloadZip_WhenServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var url = "https://example.com/team";
        var id = "team123";
        _mockCrawlerService.Setup(s => s.ExtractAllDataAsync(url, id))
            .ThrowsAsync(new Exception("ZIP creation failed"));

        // Act
        var result = await _controller.DownloadZip(url, id);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Error creating download file");
    }

    #endregion

    #region Custom Selector Tests

    [Fact]
    public async Task ExtractPlayers_WithCustomSelector_UsesCustomSelector()
    {
        // Arrange
        var url = "https://example.com/players";
        var customSelector = "//custom/table/selector";
        var expectedData = new List<PlayerDto> { CreateTestPlayer() };
        var serviceResult = Result<List<PlayerDto>>.Ok(expectedData);
        _mockCrawlerService.Setup(s => s.ExtractPlayersAsync(url, customSelector))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.ExtractPlayers(url, customSelector);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        _mockCrawlerService.Verify(s => s.ExtractPlayersAsync(url, customSelector), Times.Once);
    }

    [Fact]
    public async Task ExtractGoalkeeping_WithCustomSelector_UsesCustomSelector()
    {
        // Arrange
        var url = "https://example.com/goalkeeping";
        var customSelector = "//custom/goalkeeping/selector";
        var expectedData = new List<GoalkeepingDto> { CreateTestGoalkeeping() };
        var serviceResult = Result<List<GoalkeepingDto>>.Ok(expectedData);
        _mockCrawlerService.Setup(s => s.ExtractGoalkeepingAsync(url, customSelector))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.ExtractGoalkeeping(url, customSelector);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        _mockCrawlerService.Verify(s => s.ExtractGoalkeepingAsync(url, customSelector), Times.Once);
    }

    #endregion

    #region Club Name Parameter Tests

    [Fact]
    public async Task ExtractPlayerDetails_WithClubName_IncludesClubNameInServiceCall()
    {
        // Arrange
        var url = "https://example.com/player";
        var clubName = "Test Club";
        var expectedData = CreateTestPlayerDetails();
        var serviceResult = Result<PlayerDetails>.Ok(expectedData);
        _mockCrawlerService.Setup(s => s.ExtractPlayerDetailsAsync(url, It.IsAny<string>(), clubName))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await _controller.ExtractPlayerDetails(url, clubName: clubName);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        _mockCrawlerService.Verify(s => s.ExtractPlayerDetailsAsync(url, It.IsAny<string>(), clubName), Times.Once);
    }

    #endregion
}