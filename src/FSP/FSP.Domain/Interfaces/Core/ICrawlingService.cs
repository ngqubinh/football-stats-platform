using FSP.Domain.Entities;
using FSP.Domain.Entities.Core;

namespace FSP.Domain.Interfaces.Core;

public interface ICrawlingService
{
    Task<Result<bool>> IsServerAliveAsync();
    Task<Result<List<URLInformation>>> CrawlPremierLeagueAsync();
    Task<Result<List<URLInformation>>> CrawlRomaniaLiga1Async();
}
