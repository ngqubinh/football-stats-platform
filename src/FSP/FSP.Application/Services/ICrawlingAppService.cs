using FSP.Application.DTOs.Core;
using FSP.Application.Mappings;
using FSP.Domain.Entities;
using FSP.Domain.Entities.Core;
using FSP.Domain.Interfaces.Core;

namespace FSP.Application.Services;

public interface ICrawlingAppService
{
    Task<Result<List<URLInformationDto>>> CrawlPremierLeagueAsync();
}

public class CrawlingAppService : ICrawlingAppService
{
    private readonly ICrawlingService _crawling;
    private readonly ICoreMappingService _core;

    public CrawlingAppService(ICrawlingService crawling, ICoreMappingService core)
    {
        this._crawling = crawling;
        this._core = core;
    }

    public async Task<Result<List<URLInformationDto>>> CrawlPremierLeagueAsync()
    {
        try
        {
            Result<List<URLInformation>> domainResult = await this._crawling.CrawlPremierLeagueAsync();
            if (!domainResult.Success)
                return Result<List<URLInformationDto>>.Fail(domainResult.Message!);

            List<URLInformationDto> urlInformationDtos = this._core.ToUrlInformationDtos(domainResult.Data!).ToList();
            return Result<List<URLInformationDto>>.Ok(urlInformationDtos);
        }
        catch (Exception ex)
        {
            return Result<List<URLInformationDto>>.Fail($"Error crawling players for: {ex.Message}.");
        }
    }
}
